using UnityEngine;

public class RotateObject : MonoBehaviour, IMovable
{
    [SerializeField] private Vector3 baseRotationSpeed = new Vector3(50, 50, 50);
    private Vector3 currentRotationSpeed;
    private bool isPaused = false;
    
    private void Start()
    {
        currentRotationSpeed = baseRotationSpeed;
    }

    private void Update()
    {
        if (!isPaused)
        {
            transform.Rotate(currentRotationSpeed * Time.deltaTime);
        }
    }

    // 움직임 방향에 따른 회전 속도 조절
    public void UpdateRotation(Vector3 movementDirection, float speedMultiplier)
    {
        // 이동 방향과 속도에 따른 회전 계산
        float speed = movementDirection.magnitude * speedMultiplier;
        
        // 이동 방향에 따른 회전축 결정
        Vector3 rotationAxis = Vector3.Cross(Vector3.forward, movementDirection).normalized;
        
        // 기본 자전과 이동에 따른 회전 조합
        currentRotationSpeed = baseRotationSpeed + (rotationAxis * speed * 50f);
    }

    public void SetBaseRotationSpeed(Vector3 rotationSpeed)
    {
        baseRotationSpeed = rotationSpeed;
        currentRotationSpeed = baseRotationSpeed;
    }

    public void SetPaused(bool _isPaused)
    {
        isPaused = _isPaused;
    }
}
