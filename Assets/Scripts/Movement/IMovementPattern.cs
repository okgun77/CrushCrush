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
    public int rotationDirection;    // 회전 방향 (-1: 왼쪽, 1: 오른쪽)
    public float speedMultiplier;    // 기본 속도에 대한 배율
    public float amplitudeMultiplier; // 기본 반경에 대한 배율

    // 회전 관련 설정 추가
    public Vector3 baseRotationSpeed;    // 기본 회전 속도
    public float rotationSpeedMultiplier; // 회전 속도 배율
    public bool useMovementBasedRotation; // 이동 방향 기반 회전 사용 여부
}