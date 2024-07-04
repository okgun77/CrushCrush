using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f; // 이동 속도
    private Vector3 velocity;

    private void Update()
    {
        // 정면 방향으로 이동
        velocity = Vector3.back * speed;
        transform.Translate(velocity * Time.deltaTime);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
