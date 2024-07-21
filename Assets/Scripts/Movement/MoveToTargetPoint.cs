using UnityEngine;

public class MoveToTargetPoint : MonoBehaviour
{
    [SerializeField] private float speed; // 이동 속도

    private Transform targetPoint; // 타겟 포인트
    private Vector3 currentVelocity; // 현재 속도

    private void Update()
    {
        if (targetPoint != null)
        {
            // 타겟 포인트로 이동
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            currentVelocity = direction * speed;
            transform.Translate(currentVelocity * Time.deltaTime, Space.World);
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
}
