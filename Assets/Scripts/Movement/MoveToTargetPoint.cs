using UnityEngine;

public class MoveToTargetPoint : MonoBehaviour, IMovable
{
    [SerializeField] private float speed = 1f; // 이동 속도
    [SerializeField] private float margin = 0.1f; // 화면 경계로부터의 마진 (뷰포트 좌표 기준, 0~1 사이)

    private Transform targetPoint; // 타겟 포인트
    private Vector3 currentVelocity; // 현재 속도
    private bool isPaused = false; // 일시정지 상태 플래그

    private void Update()
    {
        if (!isPaused && targetPoint != null)
        {
            // 타겟 포인트로 이동
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            currentVelocity = direction * speed;

            // 속도가 제대로 반영되고 있는지 확인하는 디버그 로그 추가
            // Debug.Log($"Current Speed: {speed}, Velocity: {currentVelocity}");

            Vector3 newPosition = transform.position + currentVelocity * Time.deltaTime;

            // 화면 경계를 넘지 않도록 위치 조정
            newPosition = ClampPositionToScreen(newPosition);
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

        // SetSpeed 호출 시 속도 변경이 즉시 반영되는지 확인하기 위한 로그 추가
        Debug.Log($"Speed set to: {speed}");
    }

    // 일시정지 상태 적용
    public void SetPaused(bool _isPaused)
    {
        isPaused = _isPaused;
    }

    // 화면 경계를 넘지 않도록 위치를 제한하는 함수
    private Vector3 ClampPositionToScreen(Vector3 _position)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(_position);

        // 마진 적용
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, margin, 1 - margin);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, margin, 1 - margin);

        return Camera.main.ViewportToWorldPoint(viewportPosition);
    }
}
