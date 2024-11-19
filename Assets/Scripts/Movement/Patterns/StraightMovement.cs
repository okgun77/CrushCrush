using UnityEngine;

public class StraightMovement : IMovementPattern
{
    private float speed;
    private float duration;
    private float elapsedTime;

    public bool IsComplete => elapsedTime >= duration;

    public void Initialize(MovementData data)
    {
        speed = data.speed;
        duration = data.duration;
        elapsedTime = 0f;
    }

    public Vector3 CalculateMovement(Transform objectTransform, Transform targetTransform, float deltaTime)
    {
        elapsedTime += deltaTime;
        Vector3 direction = (targetTransform.position - objectTransform.position).normalized;
        return direction * speed * deltaTime;
    }
} 