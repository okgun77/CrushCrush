using UnityEngine;

public interface IMovementPattern
{
    Vector3 CalculateMovement(Transform _objectTransform, Transform _targetTransform, float _deltaTime);
    bool IsComplete { get; }
    void Initialize(MovementData _data);
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