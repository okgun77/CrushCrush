using UnityEngine;
using System.Collections;

public class FragmentMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Transform targetPoint;
    private bool isMovingToTarget = false;

    // 초기 퍼짐을 위한 힘의 세기
    public float initialSpreadForce = 3.0f;

    // 타겟을 향해 움직이기 전 대기 시간
    public float delayBeforeMove = 1.0f;

    // 타겟으로 이동할 때 최대 속도
    public float maxMoveSpeed = 5.0f;

    // 타겟으로 이동할 때 가속도
    public float acceleration = 3.0f;

    // 타겟에 가까워질 때 감속 반경
    public float slowDownRadius = 3.0f;

    // 방향 전환을 얼마나 부드럽게 할지 (1에 가까울수록 부드러움)
    public float turnSmoothness = 0.1f;

    // 현재 속도
    private float currentSpeed = 0.0f;

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

    private IEnumerator MoveAfterDelay()
    {
        // 설정된 시간만큼 대기
        yield return new WaitForSeconds(delayBeforeMove);

        // 타겟으로 이동을 시작하도록 플래그 설정
        isMovingToTarget = true;
    }

    private void FixedUpdate()
    {
        if (isMovingToTarget && targetPoint != null)
        {
            // 타겟까지의 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, targetPoint.position);

            // 현재 파편의 이동 방향
            Vector3 currentDirection = rb.velocity.normalized;

            // 타겟 방향 계산
            Vector3 targetDirection = (targetPoint.position - transform.position).normalized;

            // 방향을 천천히 전환하기 위해 Slerp 사용
            Vector3 newDirection = Vector3.Slerp(currentDirection, targetDirection, turnSmoothness);

            // 타겟에 가까워질수록 속도를 줄이는 감속 적용
            if (distanceToTarget < slowDownRadius)
            {
                // 감속: 타겟에 가까워질수록 속도가 감소
                currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.fixedDeltaTime);
            }
            else
            {
                // 가속: 최대 속도까지 천천히 증가
                currentSpeed = Mathf.Lerp(currentSpeed, maxMoveSpeed, acceleration * Time.fixedDeltaTime);
            }

            // 부드러운 방향 전환에 따라 속도를 설정
            rb.velocity = newDirection * currentSpeed;
        }
    }
}
