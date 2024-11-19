using UnityEngine;

public class ZigzagMovement : IMovementPattern
{
    private float speed;
    private float amplitude;
    private float frequency;
    private float duration;
    private float elapsedTime;

    public bool IsComplete => elapsedTime >= duration;

    public void Initialize(MovementData data)
    {
        speed = data.speed;
        amplitude = data.amplitude;
        frequency = data.frequency;
        duration = data.duration;
        elapsedTime = 0f;
    }

    public Vector3 CalculateMovement(Transform objectTransform, Transform targetTransform, float deltaTime)
    {
        elapsedTime += deltaTime;
        Vector3 direction = (targetTransform.position - objectTransform.position).normalized;
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        
        float zigzag = Mathf.Sin(elapsedTime * frequency) * amplitude;
        return (direction * speed + right * zigzag) * deltaTime;
    }
} 