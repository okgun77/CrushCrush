using UnityEngine;
using System.Collections;

public class FragmentMovement : MonoBehaviour
{
    // 초기 퍼짐을 위한 힘의 세기
    public float initialSpreadForce = 2.0f;
    // 타겟을 향해 움직이기 전 대기 시간
    public float delayBeforeMove = 0.8f;
    // 타겟으로 이동할 때 최대 속도
    public float maxMoveSpeed = 8.0f;
    // 감속 시작 거리
    public float slowDownRadius = 50f;
    // 감속/가속 시간
    public float transitionDuration = 0.5f;
    // 페이드 아웃 시작 거리
    public float fadeStartDistance = 50f;
    // 급격한 감속을 시작할 거리
    public float hardStopRadius = 20f;
    // 최소 속도 (완전히 멈추지 않도록)
    public float minSpeed = 0.1f;
    // 페이드 아웃 시작까지의 최소 거리
    public float minFadeDistance = 20f;
    // 스크린 좌표상에서의 페이드 거리 설정
    public float fadeStartScreenDistance = 30f;  // 페이드 시작 거리 (픽셀)
    public float fadeEndScreenDistance = 15f;    // 완전히 사라지는 거리 (픽셀)

    private Rigidbody rb;
    private Vector3 targetScreenPosition;
    private bool hasTarget = false;
    private bool isMovingToTarget = false;
    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    private Vector3 initialVelocity;
    private Material[] materials;
    private bool isFading = false;
    private Vector3 initialScreenPosition;
    private float totalScreenDistance;
    private bool hasInitializedDistance = false;
    private float initialZ;
    private float targetZ;
    private Vector3 cameraPosition;
    private Camera mainCamera;
    private float nearPlaneOffset = 2f; // 카메라 near plane으로부터의 오프셋
    private Vector3 targetWorldPosition;
    private float zoomFactor = 0.8f; // 카메라 방향으로 얼마나 빠르게 다가올지 결정

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
        if (rb == null || !hasTarget) return;

        if (isTransitioning)
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
            return;
        }

        if (!isMovingToTarget) return;

        Vector3 fragmentScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        
        if (fragmentScreenPos.z > 0)
        {
            // 파편의 크기를 고려한 거리 계산
            Renderer renderer = GetComponent<Renderer>();
            Vector3 size = renderer != null ? renderer.bounds.size : Vector3.one;
            
            // 스크린상의 파편 크기 계산
            Vector3 screenSize = mainCamera.WorldToScreenPoint(transform.position + size/2) - 
                               mainCamera.WorldToScreenPoint(transform.position - size/2);
            float screenRadius = Mathf.Max(Mathf.Abs(screenSize.x), Mathf.Abs(screenSize.y)) * 0.5f;

            // 스크린상의 2D 거리 계산 (파편 크기 고려)
            float distanceInScreen = Vector2.Distance(
                new Vector2(fragmentScreenPos.x, fragmentScreenPos.y),
                new Vector2(targetScreenPosition.x, targetScreenPosition.y)
            ) - screenRadius; // 파편 크기만큼 거리 감소

            // 페이드 아웃 처리 (수정된 부분)
            if (distanceInScreen < fadeStartScreenDistance)
            {
                float alpha = Mathf.InverseLerp(fadeEndScreenDistance, fadeStartScreenDistance, distanceInScreen);
                SetMaterialsAlpha(alpha);

                if (distanceInScreen <= fadeEndScreenDistance)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
            
            // 목표 위치 계산 (수정된 부분)
            Vector3 screenTarget = new Vector3(targetScreenPosition.x, targetScreenPosition.y, 10f); // 고정된 z값
            Vector3 worldTargetPosition = mainCamera.ScreenToWorldPoint(screenTarget);
            
            // 카메라 방향으로의 보정된 목표 위치 계산
            Vector3 cameraDirection = (mainCamera.transform.position - transform.position).normalized;
            float targetDistance = Mathf.Min(distanceToCamera, 10f); // 카메라로부터의 목표 거리
            Vector3 targetPos = mainCamera.transform.position - cameraDirection * targetDistance;
            
            // 최종 목표 위치는 UI 타겟 위치와 카메라 방향 보정을 혼합
            Vector3 finalTargetPos = Vector3.Lerp(
                worldTargetPosition,
                targetPos,
                Mathf.InverseLerp(50f, 0f, distanceToCamera) // 거리에 따른 보정 강도 조절
            );

            Vector3 directionToTarget = (finalTargetPos - transform.position).normalized;

            // 속도 조절 (수정된 부분)
            float speedMultiplier = 1f;
            if (distanceInScreen < hardStopRadius)
            {
                // 더 급격한 감속
                speedMultiplier = Mathf.Lerp(0.1f, 0.3f, distanceInScreen / hardStopRadius);
            }
            else if (distanceInScreen < slowDownRadius)
            {
                speedMultiplier = Mathf.Lerp(0.3f, 1f, (distanceInScreen - hardStopRadius) / (slowDownRadius - hardStopRadius));
            }

            // 거리에 따른 추가 속도 보정
            float distanceMultiplier = Mathf.Lerp(1.5f, 1f, distanceToCamera / 50f);
            speedMultiplier *= distanceMultiplier;

            // 목표 지점 근처에서 추가 감속
            if (distanceInScreen < fadeStartScreenDistance)
            {
                speedMultiplier *= Mathf.Lerp(0.1f, 1f, distanceInScreen / fadeStartScreenDistance);
            }

            Vector3 targetVelocity = directionToTarget * maxMoveSpeed * speedMultiplier;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);
        }
    }

    private void SetMaterialsAlpha(float alpha)
    {
        if (materials == null) return;

        foreach (Material mat in materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
            else if (mat.HasProperty("_Alpha"))
            {
                mat.SetFloat("_Alpha", alpha);
            }
        }
    }

    public void SetUITarget(RectTransform uiTarget)
    {
        if (uiTarget != null)
        {
            Canvas canvas = uiTarget.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                targetScreenPosition = uiTarget.position;
            }
            else
            {
                targetScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, uiTarget.position);
            }
            hasTarget = true;
        }
    }
}
