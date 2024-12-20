using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager : MonoBehaviour
{
    [System.Serializable]
    public class DifficultySettings
    {
        public float speedMultiplier = 1f;      // 속도 배율
        public float amplitudeMultiplier = 1f;  // 진폭 배율
        public float frequencyMultiplier = 1f;  // 주파수 배율
    }

    private GameManager gameManager;
    private Transform playerTransform;
    private Dictionary<GameObject, Coroutine> movementCoroutines = new Dictionary<GameObject, Coroutine>();
    private DifficultySettings currentDifficulty = new DifficultySettings();

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure there is an object with 'Player' tag in the scene.");
        }
        ResetDifficulty();
        Debug.Log("MovementManager initialized");
    }

    // 난이도 조절 메서드
    public void UpdateDifficulty(float progressRatio) // 0.0 ~ 1.0
    {
        // 스테이지 진행도에 따라 난이도 상승
        currentDifficulty.speedMultiplier = Mathf.Lerp(1f, 2f, progressRatio);
        currentDifficulty.amplitudeMultiplier = Mathf.Lerp(1f, 1.5f, progressRatio);
        currentDifficulty.frequencyMultiplier = Mathf.Lerp(1f, 1.5f, progressRatio);
    }

    public void ResetDifficulty()
    {
        currentDifficulty = new DifficultySettings();
    }

    public void StartMovement(GameObject obj, MovementType pattern, MovementData data, Vector3 targetPosition)
    {
        if (obj == null) return;

        // 난이도 설정 적용
        MovementData adjustedData = new MovementData
        {
            speed = data.speed * currentDifficulty.speedMultiplier,
            amplitude = data.amplitude * currentDifficulty.amplitudeMultiplier,
            frequency = data.frequency * currentDifficulty.frequencyMultiplier,
            duration = data.duration
        };

        // 스파이럴 패턴일 경우 랜덤한 움직임 값 설정
        if (pattern == MovementType.Spiral)
        {
            adjustedData.rotationDirection = Random.value < 0.5f ? -1 : 1;  // 50% 확률로 왼쪽/오른쪽 회전
            adjustedData.speedMultiplier = Random.Range(0.8f, 1.5f);       // 속도 변화
            adjustedData.amplitudeMultiplier = Random.Range(0.7f, 1.3f);   // 반경 변화
        }

        StopMovement(obj);
        Coroutine coroutine = StartCoroutine(MoveObject(obj, pattern, adjustedData, targetPosition));
        movementCoroutines[obj] = coroutine;
    }

    private IEnumerator MoveObject(GameObject obj, MovementType pattern, MovementData data, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = obj.transform.position;
        Vector3 previousPosition = obj.transform.position;
        
        // 회전 컴포넌트 가져오기
        RotateObject rotator = obj.GetComponent<RotateObject>();
        if (rotator == null)
        {
            rotator = obj.AddComponent<RotateObject>();
        }

        while (obj != null && obj.activeInHierarchy)
        {
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
                if (playerTransform == null) yield break;
            }

            Vector3 directionToPlayer = playerTransform.position - obj.transform.position;
            
            // 방향 계산 시 예외 처리 추가
            Vector3 currentDirection;
            if (directionToPlayer.magnitude < 0.001f)
            {
                currentDirection = Vector3.forward;
            }
            else
            {
                currentDirection = directionToPlayer.normalized;
            }

            Vector3 newPosition = CalculatePosition(obj.transform.position, currentDirection, pattern, data, elapsedTime);
            
            // 이동 방향 계산 및 회전 업데이트
            Vector3 movementDirection = (newPosition - obj.transform.position).normalized;
            if (rotator != null)
            {
                rotator.UpdateRotation(movementDirection, data.speed * currentDifficulty.speedMultiplier);
            }
            
            // 위치 값 유효성 검사
            if (!float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y) && !float.IsNaN(newPosition.z))
            {
                obj.transform.position = newPosition;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        movementCoroutines.Remove(obj);
    }

    private Vector3 ClampToScreen(Vector3 position)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);
        
        // 화면 경계에서 약간의 여유 공간을 둡니다 (10%)
        float padding = 0.1f;
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, padding, 1 - padding);
        viewportPoint.y = Mathf.Clamp(viewportPoint.y, padding, 1 - padding);
        
        return Camera.main.ViewportToWorldPoint(viewportPoint);
    }

    private Vector3 CalculatePosition(Vector3 currentPos, Vector3 direction, MovementType pattern, MovementData data, float time)
    {
        if (data.speed <= 0) data.speed = 5f; // 기본 속도 설정
        
        // 기본 이동을 현재 위치 기준으로 계산
        Vector3 basePosition = currentPos + direction * data.speed * Time.deltaTime;
        Vector3 finalPosition;

        try
        {
            switch (pattern)
            {
                case MovementType.Straight:
                    finalPosition = basePosition;
                    break;

                case MovementType.Zigzag:
                    if (data.frequency <= 0) data.frequency = 1f;
                    if (data.amplitude <= 0) data.amplitude = 1f;
                    float zigzag = Mathf.Sin(time * data.frequency) * data.amplitude;
                    finalPosition = basePosition + Vector3.right * zigzag * Time.deltaTime;
                    break;

                case MovementType.Spiral:
                    if (data.frequency <= 0) data.frequency = 1f;
                    if (data.amplitude <= 0) data.amplitude = 1f;
                    float speedVariation = Mathf.Sin(time * 0.5f) + 1f;
                    float spiral = Mathf.Sin(time * data.frequency) * data.amplitude;
                    Vector3 offset = new Vector3(
                        Mathf.Cos(time * data.frequency),
                        Mathf.Sin(time * data.frequency),
                        0
                    ) * data.amplitude;
                    finalPosition = basePosition + offset * Time.deltaTime;
                    break;

                default:
                    finalPosition = basePosition;
                    break;
            }

            // 화면 경계 제한 적용
            return ClampToScreen(finalPosition);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error calculating position: {e.Message}");
            return currentPos;
        }
    }

    public void StopMovement(GameObject obj)
    {
        if (movementCoroutines.TryGetValue(obj, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            movementCoroutines.Remove(obj);
        }
    }

    private void OnDestroy()
    {
        foreach (var coroutine in movementCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        movementCoroutines.Clear();
    }
}