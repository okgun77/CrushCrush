using UnityEngine;

public interface IMovementPattern
{
    Vector3 CalculateMovement(Transform objectTransform, Transform targetTransform, float deltaTime);
    bool IsComplete { get; }
    void Initialize(MovementData data);
}

// 움직임 데이터 구조체
[System.Serializable]
public struct MovementData
{
    public float speed;
    public float amplitude;
    public float frequency;
    public float duration;
} 