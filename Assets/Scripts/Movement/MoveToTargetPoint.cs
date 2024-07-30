using UnityEngine;

public class MoveToTargetPoint : MonoBehaviour
{
    [SerializeField] private float speed = 3f; // 이동 속도
    [SerializeField] private float margin = 0.1f; // 화면 경계로부터의 마진 (뷰포트 좌표 기준, 0~1 사이)

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

    private Vector3 ClampPositionToScreen(Vector3 _position, Vector3 _direction)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(_position);
        bool isClamped = false;

        // 마진 적용
        float leftMargin = margin;
        float rightMargin = 1 - margin;
        float bottomMargin = margin;
        float topMargin = 1 - margin;

        if (viewportPosition.x < leftMargin || viewportPosition.x > rightMargin)
        {
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, leftMargin, rightMargin);
            isClamped = true;
        }

        if (viewportPosition.y < bottomMargin || viewportPosition.y > topMargin)
        {
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, bottomMargin, topMargin);
            isClamped = true;
        }

        if (isClamped)
        {
            // 반사 벡터 계산
            Vector3 reflectedDirection = Vector3.Reflect(_direction, Vector3.up);
            // 자연스러운 곡선을 위한 Slerp 사용
            _direction = Vector3.Slerp(_direction, reflectedDirection, Time.deltaTime * speed);
            currentVelocity = _direction * speed;
        }

        return Camera.main.ViewportToWorldPoint(viewportPosition);
    }
}
