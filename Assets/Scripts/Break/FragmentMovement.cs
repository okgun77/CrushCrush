using UnityEngine;
using System.Collections;

public class FragmentMovement : MonoBehaviour
{
    public float initialSpreadForce = 2.0f;      // 초기 퍼짐을 위한 힘의 세기
    public float delayBeforeMove = 0.8f;         // 타겟을 향해 움직이기 전 대기 시간
    public float maxMoveSpeed = 8.0f;            // 타겟으로 이동할 때 최대 속도
    public float slowDownRadius = 50f;           // 감속 시작 거리
    public float transitionDuration = 0.5f;      // 감속/가속 시간
    public float fadeStartDistance = 50f;        // 페이드 아웃 시작 거리
    public float hardStopRadius = 20f;           // 급격한 감속을 시작할 거리
    public float minSpeed = 0.1f;                // 최소 속도 (완전히 멈추지 않도록)
    public float minFadeDistance = 20f;          // 페이드 아웃 시작까지의 최소 거리
    public float fadeStartScreenDistance = 60f;   // 페이드 시작 거리 (픽셀)
    public float fadeEndScreenDistance = 30f;     // 완전히 사라지는 거리 (픽셀)

    private Rigidbody rb;                        // 리지드바디 컴포넌트
    private Vector3 targetScreenPosition;         // UI 타겟의 스크린 좌표
    private bool hasTarget = false;              // 타겟이 설정되었는지 여부
    private bool isMovingToTarget = false;       // 타겟을 향해 이동 중인지 여부
    private bool isTransitioning = false;        // 전환 중인지 여부
    private float transitionTimer = 0f;          // 전환 타이머
    private Vector3 initialVelocity;             // 초기 속도
    private Material[] materials;                // 파편의 머티리얼 배열
    private bool isFading = false;               // 페이드 아웃 중인지 여부
    private Vector3 initialScreenPosition;        // 초기 스크린 좌표
    private float totalScreenDistance;           // 전체 스크린 이동 거리
    private bool hasInitializedDistance = false; // 거리가 초기화되었는지 여부
    private float initialZ;                      // 초기 Z 좌표
    private float targetZ;                       // 목표 Z 좌표
    private Vector3 cameraPosition;              // 카메라 위치
    private Camera mainCamera;                   // 메인 카메라 참조
    private float nearPlaneOffset = 2f;          // 카메라 near plane으로부터의 오프셋
    private Vector3 targetWorldPosition;         // 월드 좌표계에서의 타겟 위치
    private float zoomFactor = 0.8f;             // 카메라 방향으로 얼마나 빠르게 다가올지 결정

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        cameraPosition = mainCamera.transform.position;

        if (rb != null)
        {
            // 초기 퍼짐 효과
            Vector3 randomDirection = Random.onUnitSphere;
            rb.AddForce(randomDirection * initialSpreadForce, ForceMode.Impulse);
            
            // 머티리얼 캐싱
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                materials = renderer.materials;
                SetMaterialsAlpha(1.0f);
            }
            
            StartCoroutine(StartMovingToTarget());
        }
    }

    private IEnumerator StartMovingToTarget()
    {
        yield return new WaitForSeconds(delayBeforeMove);
        
        if (rb != null)
        {
            initialVelocity = rb.linearVelocity;
            isTransitioning = true;
            transitionTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!CanUpdate()) return;
        if (isTransitioning)
        {
            HandleTransition();
            return;
        }
        if (!isMovingToTarget) return;

        HandleFragmentMovement();
    }

    private bool CanUpdate()
    {
        return rb != null && hasTarget;
    }

    private void HandleFragmentMovement()
    {
        Vector3 fragmentScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        if (fragmentScreenPos.z > 0)
        {
            float distanceInScreen = CalculateScreenDistance(fragmentScreenPos);
            
            // 페이드 아웃 체크 먼저 수행
            if (HandleFadeOut(distanceInScreen)) return;

            // 이동 처리
            HandleMovement(fragmentScreenPos, distanceInScreen);
        }
    }

    private void HandleTransition()
    {
        transitionTimer += Time.fixedDeltaTime;
        float t = transitionTimer / transitionDuration;
        
        if (t <= 1.0f)
        {
            rb.linearVelocity = Vector3.Lerp(initialVelocity, Vector3.zero, t);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, t);
        }
        else
        {
            isTransitioning = false;
            isMovingToTarget = true;
        }
    }

    private float CalculateScreenDistance(Vector3 _fragmentScreenPos)
    {
        Renderer renderer = GetComponent<Renderer>();
        Vector3 size = renderer != null ? renderer.bounds.size : Vector3.one;
        
        Vector3 screenSize = mainCamera.WorldToScreenPoint(transform.position + size/2) - 
                            mainCamera.WorldToScreenPoint(transform.position - size/2);
        float screenRadius = Mathf.Max(Mathf.Abs(screenSize.x), Mathf.Abs(screenSize.y)) * 0.5f;

        return Vector2.Distance(
            new Vector2(_fragmentScreenPos.x, _fragmentScreenPos.y),
            new Vector2(targetScreenPosition.x, targetScreenPosition.y)
        ) - screenRadius;
    }

    private bool HandleFadeOut(float _distanceInScreen)
    {
        if (_distanceInScreen < fadeStartScreenDistance)
        {
            float alpha = Mathf.InverseLerp(fadeEndScreenDistance, fadeStartScreenDistance, _distanceInScreen);
            SetMaterialsAlpha(alpha);

            if (_distanceInScreen <= fadeEndScreenDistance)
            {
                Destroy(gameObject);
                return true;
            }
        }
        return false;
    }

    private Vector3 CalculateTargetPosition(float _distanceToCamera)
    {
        Vector3 screenTarget = new Vector3(targetScreenPosition.x, targetScreenPosition.y, 10f);
        Vector3 worldTargetPosition = mainCamera.ScreenToWorldPoint(screenTarget);
        
        Vector3 cameraDirection = (mainCamera.transform.position - transform.position).normalized;
        float targetDistance = Mathf.Min(_distanceToCamera, 10f);
        Vector3 targetPos = mainCamera.transform.position - cameraDirection * targetDistance;
        
        return Vector3.Lerp(
            worldTargetPosition,
            targetPos,
            Mathf.InverseLerp(50f, 0f, _distanceToCamera)
        );
    }

    private float CalculateSpeedMultiplier(float _distanceInScreen, float _distanceToCamera)
    {
        float speedMultiplier = 1f;
        
        if (_distanceInScreen < hardStopRadius)
        {
            speedMultiplier = Mathf.Lerp(0.1f, 0.3f, _distanceInScreen / hardStopRadius);
        }
        else if (_distanceInScreen < slowDownRadius)
        {
            speedMultiplier = Mathf.Lerp(0.3f, 1f, (_distanceInScreen - hardStopRadius) / (slowDownRadius - hardStopRadius));
        }

        float distanceMultiplier = Mathf.Lerp(1.5f, 1f, _distanceToCamera / 50f);
        speedMultiplier *= distanceMultiplier;

        if (_distanceInScreen < fadeStartScreenDistance)
        {
            speedMultiplier *= Mathf.Lerp(0.1f, 1f, _distanceInScreen / fadeStartScreenDistance);
        }

        return speedMultiplier;
    }

    private void HandleMovement(Vector3 _fragmentScreenPos, float _distanceInScreen)
    {
        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        Vector3 finalTargetPos = CalculateTargetPosition(distanceToCamera);
        Vector3 directionToTarget = (finalTargetPos - transform.position).normalized;
        
        float speedMultiplier = CalculateSpeedMultiplier(_distanceInScreen, distanceToCamera);
        Vector3 targetVelocity = directionToTarget * maxMoveSpeed * speedMultiplier;
        
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);
    }

    private void SetMaterialsAlpha(float _alpha)
    {
        if (materials == null) return;

        foreach (Material mat in materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = _alpha;
                mat.color = color;
            }
            else if (mat.HasProperty("_Alpha"))
            {
                mat.SetFloat("_Alpha", _alpha);
            }
        }
    }

    public void SetUITarget(RectTransform _uiTarget)
    {
        if (_uiTarget != null)
        {
            Canvas canvas = _uiTarget.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                targetScreenPosition = _uiTarget.position;
            }
            else
            {
                targetScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, _uiTarget.position);
            }
            hasTarget = true;
        }
    }
}
