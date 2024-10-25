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

    private Rigidbody rb;
    private Vector3 targetScreenPosition;
    private bool hasTarget = false;
    private bool isMovingToTarget = false;
    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    private Vector3 initialVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 초기 퍼짐 효과
            Vector3 randomDirection = Random.onUnitSphere;
            rb.AddForce(randomDirection * initialSpreadForce, ForceMode.Impulse);
            
            StartCoroutine(StartMovingToTarget());
        }
    }

    private IEnumerator StartMovingToTarget()
    {
        yield return new WaitForSeconds(delayBeforeMove);
        
        if (rb != null)
        {
            // 현재 속도 저장
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
            // 부드러운 전환 처리
            transitionTimer += Time.fixedDeltaTime;
            float t = transitionTimer / transitionDuration;
            
            if (t <= 1.0f)
            {
                // 현재 속도를 서서히 감소
                rb.linearVelocity = Vector3.Lerp(initialVelocity, Vector3.zero, t);
                
                // 회전도 서서히 감소
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

        Vector3 fragmentScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        
        if (fragmentScreenPos.z > 0)
        {
            // 스크린 상의 거리 계산
            float distanceToTarget = Vector2.Distance(
                new Vector2(fragmentScreenPos.x, fragmentScreenPos.y),
                new Vector2(targetScreenPosition.x, targetScreenPosition.y)
            );

            // 타겟의 월드 좌표 계산
            Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(targetScreenPosition.x, targetScreenPosition.y, fragmentScreenPos.z)
            );

            // 타겟으로 향하는 직접적인 방향
            Vector3 directionToTarget = (targetWorldPos - transform.position).normalized;

            // 거리에 따른 속도 조절
            float speedMultiplier = distanceToTarget < slowDownRadius ? 
                Mathf.Lerp(0, 1, distanceToTarget / slowDownRadius) : 1f;

            // 부드러운 가속
            Vector3 targetVelocity = directionToTarget * maxMoveSpeed * speedMultiplier;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);
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
