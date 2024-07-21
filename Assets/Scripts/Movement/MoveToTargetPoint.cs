using UnityEngine;

public class MoveToTargetPoint : MonoBehaviour
{
    [SerializeField] private float speed = 3f; // 이동 속도

    private Transform targetPoint; // 타겟 포인트
    private Vector3 currentVelocity; // 현재 속도

    private void Update()
    {
        if (targetPoint != null)
        {
            // 타겟 포인트로 이동
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            currentVelocity = direction * speed;
            Vector3 newPosition = transform.position + currentVelocity * Time.deltaTime;

            // 화면 경계를 넘지 않도록 위치 조정
            newPosition = ClampPositionToScreen(newPosition, direction);
            transform.position = newPosition;
        }
    }

    public void SetTarget(Transform _target)
    {
        targetPoint = _target;
    }

    public Vector3 GetCurrentVelocity()
    {
        return currentVelocity;
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    private Vector3 ClampPositionToScreen(Vector3 position, Vector3 direction)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(position);
        bool isClamped = false;

        if (viewportPosition.x < 0.1f || viewportPosition.x > 0.9f)
        {
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.1f, 0.9f);
            isClamped = true;
        }

        if (viewportPosition.y < 0.1f || viewportPosition.y > 0.9f)
        {
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.1f, 0.9f);
            isClamped = true;
        }

        if (isClamped)
        {
            // 반사 벡터 계산
            Vector3 reflectedDirection = Vector3.Reflect(direction, Vector3.up);
            // 자연스러운 곡선을 위한 Slerp 사용
            direction = Vector3.Slerp(direction, reflectedDirection, Time.deltaTime * speed);
            currentVelocity = direction * speed;
        }

        return Camera.main.ViewportToWorldPoint(viewportPosition);
    }
}
