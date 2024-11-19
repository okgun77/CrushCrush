using UnityEngine;

public class SpiralMovement : IMovementPattern
{
    private float speed;
    private float amplitude;
    private float frequency;
    private float duration;
    private float elapsedTime;
    private float angle;
    private Vector3 initialDirection;

    public bool IsComplete => elapsedTime >= duration;

    public void Initialize(MovementData data)
    {
        speed = data.speed;
        amplitude = data.amplitude;
        frequency = data.frequency;
        duration = data.duration;
        elapsedTime = 0f;
        angle = 0f;
    }

    public Vector3 CalculateMovement(Transform objectTransform, Transform targetTransform, float deltaTime)
    {
        elapsedTime += deltaTime;
        
        // 초기 방향 저장 (처음 한번만)
        if (elapsedTime <= deltaTime)
        {
            initialDirection = (targetTransform.position - objectTransform.position).normalized;
        }

        // 나선형 움직임 계산
        angle += frequency * deltaTime;
        float currentRadius = amplitude * (elapsedTime / duration); // 시간에 따라 반경 증가
        
        // 진행 방향을 기준으로 한 right와 up 벡터 계산
        Vector3 right = Vector3.Cross(initialDirection, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, initialDirection).normalized;
        
        // 나선형 오프셋 계산
        Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * currentRadius;
        
        // 기본 전진 움직임과 나선형 움직임 합성
        Vector3 forwardMovement = initialDirection * speed * deltaTime;
        Vector3 spiralMovement = offset - objectTransform.position;
        
        // 최종 움직임 계산
        return forwardMovement + spiralMovement * deltaTime;
    }
} 