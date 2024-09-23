using UnityEngine;

public class RotateObject : MonoBehaviour, IMovable
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(50, 50, 50); // 회전 속도 설정
    private bool isPaused = false; // 일시정지 상태 플래그

    private void Update()
    {
        if (!isPaused)
        {
            // 프레임마다 지정된 속도로 회전
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    public void SetRotationSpeed(Vector3 _rotationSpeed)
    {
        rotationSpeed = _rotationSpeed;
    }

    // 일시정지 상태 적용
    public void SetPaused(bool _isPaused)
    {
        isPaused = _isPaused;
    }
}
