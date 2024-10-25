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
    public float fadeStartScreenDistance = 50f;  // 페이드 시작 거리 (픽셀)
    public float fadeEndScreenDistance = 30f;    // 완전히 사라지는 거리 (픽셀)

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
                // 초기 투명도 설정
                SetMaterialsAlpha(1.0f);
            }
            
            // 초기 스크린 위치 저장
            initialScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            
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

        // 현재 오브젝트의 스크린 좌표 계산
        Vector3 fragmentScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        
        if (fragmentScreenPos.z > 0) // 카메라 앞에 있을 때만 처리
        {
            // 스크린 상에서의 거리 계산
            float distanceInScreen = Vector2.Distance(
                new Vector2(fragmentScreenPos.x, fragmentScreenPos.y),
                new Vector2(targetScreenPosition.x, targetScreenPosition.y)
            );

            // 월드 좌표로 변환하여 이동 방향 계산
            Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(targetScreenPosition.x, targetScreenPosition.y, fragmentScreenPos.z)
            );
            Vector3 directionToTarget = (targetWorldPos - transform.position).normalized;

            // 거리에 따른 페이드 아웃
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

            // 속도 조절
            float speedMultiplier = 1f;
            if (distanceInScreen < hardStopRadius)
            {
                speedMultiplier = Mathf.Lerp(minSpeed, 0.3f, distanceInScreen / hardStopRadius);
            }
            else if (distanceInScreen < slowDownRadius)
            {
                speedMultiplier = Mathf.Lerp(0.3f, 1f, (distanceInScreen - hardStopRadius) / (slowDownRadius - hardStopRadius));
            }

            // 속도 적용
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
