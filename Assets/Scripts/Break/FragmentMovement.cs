using UnityEngine;
using System.Collections;

public class FragmentMovement : MonoBehaviour
{
    [Header("Initial Movement Settings")]
    public float initialSpreadForce = 2.0f;      // 초기 퍼짐 힘
    public float cameraMoveMultiplier = 0.4f;    // 카메라 방향 이동 배율
    public float minCameraApproach = 2f;         // 최소 카메라 접근 거리
    public float maxCameraApproach = 15f;        // 최대 카메라 접근 거리
    
    public float delayBeforeMove = 0.8f;         // 타겟을 향해 움직이기 전 대기 시간
    public float maxMoveSpeed = 60.0f;            // 타겟으로 이동할 때 최대 속도
    public float slowDownRadius = 50f;           // 감속 시작 거리
    public float transitionDuration = 0.5f;      // 감속/가속 시간
    public float hardStopRadius = 20f;           // 급격한 감속을 시작할 거리
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
    private Camera mainCamera;                   // 메인 카메라 참조
    private const float MAX_TRANSITION_WAIT_TIME = 0.1f;  // 최대 추가 대기 시간
    private float extraWaitTimer = 0f;                    // 추가 대기 시간 타이머

    // 1. 캐시 추가
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    
    // 2. 벡터 재사용
    private Vector3 cachedVelocity;
    private Vector3 cachedDirection;
    private Vector2 screenPosition;

    /// <summary>
    /// 초기화: 리지드바디 설정, 초기 퍼짐 효과 적용, 머티리얼 설정
    /// </summary>
    private void Start()
    {
        InitializeComponents();
        if (rb != null)
        {
            ApplyInitialForces();
            CacheMaterials();
            StartCoroutine(StartMovingToTarget());
        }
    }

    /// <summary>
    /// 지정된 대기 시간 후 타겟을 향한 이동 시작
    /// </summary>
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

    /// <summary>
    /// 물리 기반 이동 업데이트
    /// </summary>
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

    /// <summary>
    /// 업데이트 가능 상태 확인
    /// </summary>
    /// <returns>리지드바디와 타겟이 유효한 경우 true</returns>
    private bool CanUpdate()
    {
        return rb != null && hasTarget;
    }

    /// <summary>
    /// 파편 이동 로직 처리: 화면상 위치 계산, 페이드 아웃, 이동 처리
    /// </summary>
    private void HandleFragmentMovement()
    {
        Vector3 fragmentScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        if (fragmentScreenPos.z > 0)
        {
            float distanceInScreen = CalculateScreenDistance(fragmentScreenPos);
            
            // 페이드 아웃 크 먼저 수행
            if (HandleFadeOut(distanceInScreen)) return;

            // 이동 처리
            HandleMovement(fragmentScreenPos, distanceInScreen);
        }
    }

    /// <summary>
    /// 초기 속도에서 정지 상태로의 전환 처리
    /// </summary>
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
            // 속도가 충분히 감속되었거나 최대 대기 시간을 초과한 경우
            if (rb.linearVelocity.magnitude < 0.1f || extraWaitTimer >= MAX_TRANSITION_WAIT_TIME)
            {
                // 안전하게 속도를 0으로 설정
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                
                isTransitioning = false;
                isMovingToTarget = true;
                extraWaitTimer = 0f;  // 타이머 리셋
            }
            else
            {
                // 추가 대기 시간 증가
                extraWaitTimer += Time.fixedDeltaTime;
                // 계속해서 감속 시도
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 10f);
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 10f);
            }
        }
    }

    /// <summary>
    /// 화면상에서의 파편과 타겟 간 거리 계산
    /// </summary>
    /// <param name="_fragmentScreenPos">화면상의 파편 위치</param>
    /// <returns>화면상의 실제 거리 (오브젝트 크기 고려)</returns>
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

    /// <summary>
    /// 거리에 따른 페이드 아웃 처리
    /// </summary>
    /// <param name="_distanceInScreen">화면상의 거리</param>
    /// <returns>오브젝트가 파괴되었으면 true</returns>
    private bool HandleFadeOut(float _distanceInScreen)
    {
        if (_distanceInScreen < fadeStartScreenDistance)
        {
            float alpha = Mathf.InverseLerp(fadeEndScreenDistance, fadeStartScreenDistance, _distanceInScreen);
            SetMaterialsAlpha(alpha);

            if (_distanceInScreen <= fadeEndScreenDistance)
            {
                // 파편이 사라질 때 FragmentCollector에 통지
                var collector = FindAnyObjectByType<FragmentCollector>();
                collector?.OnFragmentArrived();
                
                Destroy(gameObject);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 카메라와의 거리를 고려한 최종 타겟 위치 계산
    /// </summary>
    /// <param name="_distanceToCamera">카메라까지의 거리</param>
    /// <returns>월드 좌표계의 타겟 위치</returns>
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

    /// <summary>
    /// 거리에 따른 속도 계수 계산
    /// </summary>
    /// <param name="_distanceInScreen">화면상의 거리</param>
    /// <param name="_distanceToCamera">카메라까지의 거리</param>
    /// <returns>최종 속도 계수 (0.0 ~ 1.5)</returns>
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

    /// <summary>
    /// 파편의 실제 이동 처리
    /// </summary>
    /// <param name="_fragmentScreenPos">화면상의 파편 위치</param>
    /// <param name="_distanceInScreen">화면상의 거리</param>
    private void HandleMovement(Vector3 _fragmentScreenPos, float _distanceInScreen)
    {
        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        
        // 방향 계산 재사용
        cachedDirection = (CalculateTargetPosition(distanceToCamera) - transform.position).normalized;
        
        float speedMultiplier = CalculateSpeedMultiplier(_distanceInScreen, distanceToCamera);
        cachedVelocity = cachedDirection * maxMoveSpeed * speedMultiplier;
        
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, cachedVelocity, Time.fixedDeltaTime * 5f);
    }

    /// <summary>
    /// 머티리얼의 알파값 설정
    /// </summary>
    /// <param name="_alpha">설정할 알파값 (0.0 ~ 1.0)</param>
    private void SetMaterialsAlpha(float _alpha)
    {
        if (materials == null) return;

        foreach (Material mat in materials)
        {
            if (mat == null) continue;

            if (mat.HasProperty("_BaseColor"))
            {
                Color color = mat.GetColor("_BaseColor");
                color.a = _alpha;
                mat.SetColor("_BaseColor", color);
            }
        }
    }

    /// <summary>
    /// UI 타겟 설정 및 화면 좌표 계산
    /// </summary>
    /// <param name="_uiTarget">타겟 UI 요소의 RectTransform</param>
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

    private float CalculateApproachDistance(float _distanceToCamera)
    {
        float nearPlane = mainCamera.nearClipPlane;
        float normalizedDistance = Mathf.Clamp01((_distanceToCamera - nearPlane * 2) / (50f - nearPlane * 2));
        
        // 거리가 멀수록 더 많이 접근
        return Mathf.Lerp(minCameraApproach, maxCameraApproach, normalizedDistance);
    }

    private Vector3 CalculateSpreadDirection(float _approachDistance)
    {
        Vector3 toCameraDir = (mainCamera.transform.position - transform.position).normalized;
        Vector3 randomDir = Random.onUnitSphere;
        
        // 랜덤 방향과 카메라 방향을 혼합
        Vector3 spreadDir = Vector3.Lerp(randomDir, toCameraDir, cameraMoveMultiplier).normalized;
        
        // 접근 거리를 반영한 최종 방향
        return spreadDir * (1f + _approachDistance / maxCameraApproach);
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void ApplyInitialForces()
    {
        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        float approachDistance = CalculateApproachDistance(distanceToCamera);
        Vector3 randomDirection = CalculateSpreadDirection(approachDistance);
        
        rb.AddForce(randomDirection * initialSpreadForce, ForceMode.Impulse);
        rb.AddTorque(Random.onUnitSphere * initialSpreadForce * 0.2f, ForceMode.Impulse);
    }

    private void CacheMaterials()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materials = renderer.materials;
        }
    }
}
