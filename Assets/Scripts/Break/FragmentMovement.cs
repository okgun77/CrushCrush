using UnityEngine;
using System.Collections;

public class FragmentMovement : MonoBehaviour
{
    

    // 초기 퍼짐을 위한 힘의 세기
    public float initialSpreadForce = 2.0f;
    // 타겟을 향해 움직이기 전 대기 시간
    public float delayBeforeMove = 1.0f;
    // 타겟으로 이동할 때 최대 속도
    public float maxMoveSpeed = 8.0f;
    // 타겟으로 이동할 때 가속도
    public float acceleration = 4.0f;
    // 타겟에 가까워질 때 감속 반경
    public float slowDownRadius = 3.0f;
    // 방향 전환을 얼마나 부드럽게 할지 (1에 가까울수록 부드러움)
    public float turnSmoothness = 0.1f;

    private Rigidbody rb;
    private Transform targetPoint;
    private bool isMovingToTarget = false;
    private float currentSpeed = 0.0f;
    private Vector3 targetScreenPosition;
    private bool hasTarget = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 파편이 처음 생성될 때 랜덤한 방향으로 퍼짐
            Vector3 randomDirection = Random.onUnitSphere;
            rb.AddForce(randomDirection * initialSpreadForce, ForceMode.Impulse);

            // 일정 시간 후에 타겟으로 이동 시작
            StartCoroutine(MoveAfterDelay());
        }
    }

    public void MoveToTarget(Transform _target)
    {
        targetPoint = _target;
    }

    public void SetUITarget(RectTransform uiTarget)
    {
        if (uiTarget != null)
        {
            // UI 요소의 스크린 좌표를 올바르게 얻음
            Vector2 screenPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiTarget.parent as RectTransform,
                uiTarget.position,
                Camera.main,
                out screenPoint
            );
            
            // 스크린 좌표로 변환
            Canvas canvas = uiTarget.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // ScreenSpace-Overlay Canvas인 경우
                targetScreenPosition = uiTarget.position;
            }
            else
            {
                // ScreenSpace-Camera 또는 World Space Canvas인 경우
                targetScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, uiTarget.position);
            }
            
            hasTarget = true;
            StartCoroutine(MoveAfterDelay());
        }
    }

    private IEnumerator MoveAfterDelay()
    {
        // 설정된 시간만큼 대기
        yield return new WaitForSeconds(delayBeforeMove);

        // 타겟으로 이동을 시작하도록 플래그 설정
        isMovingToTarget = true;
    }

    private void FixedUpdate()
    {
        if (isMovingToTarget && hasTarget)
        {
            // 현재 파편의 스크린 좌표
            Vector3 fragmentScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            
            if (fragmentScreenPos.z > 0) // 카메라 앞에 있을 때만 방향 계산
            {
                // 스크린 상에서의 방향 계산
                Vector3 directionOnScreen = (targetScreenPosition - fragmentScreenPos).normalized;
                
                // 스크린 좌표를 월드 좌표로 변환
                Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(
                    new Vector3(targetScreenPosition.x, targetScreenPosition.y, fragmentScreenPos.z)
                );
                Vector3 worldDirection = (targetWorldPos - transform.position).normalized;

                // 부드러운 방향 전환
                Vector3 currentDirection = rb.linearVelocity.normalized;
                Vector3 newDirection = Vector3.Slerp(currentDirection, worldDirection, turnSmoothness);

                // 거리 계산 (스크린 좌표 기준)
                float distanceToTarget = Vector2.Distance(
                    new Vector2(fragmentScreenPos.x, fragmentScreenPos.y),
                    new Vector2(targetScreenPosition.x, targetScreenPosition.y)
                );

                // 속도 계산 및 적용
                if (distanceToTarget < slowDownRadius)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.fixedDeltaTime);
                }
                else
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, maxMoveSpeed, acceleration * Time.fixedDeltaTime);
                }

                rb.linearVelocity = newDirection * currentSpeed;
            }
        }
    }
}
