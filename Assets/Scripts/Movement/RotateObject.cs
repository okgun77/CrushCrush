using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(50, 50, 50); // 회전 속도 설정

    private void Update()
    {
        // 프레임마다 지정된 속도로 회전
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    public void SetRotationSpeed(Vector3 _rotationSpeed)
    {
        rotationSpeed = _rotationSpeed;
    }
}