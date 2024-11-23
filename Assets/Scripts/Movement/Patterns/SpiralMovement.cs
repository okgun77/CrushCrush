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
    private float rotationDirection; // 회전 방향 속성 추가

    public bool IsComplete => elapsedTime >= duration;

    public void Initialize(MovementData data)
    {
        speed = data.speed * data.speedMultiplier;
        amplitude = data.amplitude * data.amplitudeMultiplier;
        frequency = data.frequency;
        duration = data.duration;
        elapsedTime = 0f;
        angle = Random.Range(0f, Mathf.PI * 2f); // 랜덤한 초기 각도
        rotationDirection = data.rotationDirection; // 회전 방향 속성 초기화
    }

    public Vector3 CalculateMovement(Transform _objectTransform, Transform _targetTransform, float _deltaTime)
    {
        elapsedTime += _deltaTime;
        
        // 초기 방향 저장 (처음 한번만)
        if (elapsedTime <= _deltaTime)
        {
            initialDirection = (_targetTransform.position - _objectTransform.position).normalized;
        }

        // 나선형 움직임 계산
        float speedVariation = Mathf.Sin(elapsedTime * 0.5f) + 1f; // 0.5배에서 1.5배 사이로 속도 변화
        angle += frequency * speedVariation * _deltaTime * rotationDirection; // 회전 방향 적용
        
        // 반경을 주기적으로 변화시킴
        float radiusVariation = Mathf.Sin(elapsedTime * 0.3f) * 0.3f + 1f; // 0.7배에서 1.3배 사이로 반경 변화
        float currentRadius = amplitude * (0.2f + (elapsedTime / duration)) * radiusVariation;
        
        // 진행 방향을 기준으로 한 right와 up 벡터 계산
        Vector3 right = Vector3.Cross(initialDirection, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, initialDirection).normalized;
        
        // 나선형 오프셋 계산
        Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * currentRadius;
        
        // 기본 전진 움직임과 나선형 움직임 합성
        Vector3 forwardMovement = initialDirection * speed * _deltaTime;
        Vector3 spiralMovement = offset - _objectTransform.position;
        
        // 최종 움직임 계산
        return forwardMovement + spiralMovement * _deltaTime;
    }
} 