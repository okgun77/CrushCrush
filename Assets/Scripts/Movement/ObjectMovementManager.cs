using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectMovementManager : MonoBehaviour
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
        Debug.Log("ObjectMovementManager initialized");
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

        StopMovement(obj);
        Coroutine coroutine = StartCoroutine(MoveObject(obj, pattern, adjustedData, targetPosition));
        movementCoroutines[obj] = coroutine;
    }

    private IEnumerator MoveObject(GameObject obj, MovementType pattern, MovementData data, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = obj.transform.position;

        while (obj != null)
        {
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
                if (playerTransform == null) yield break;
            }

            Vector3 currentDirection = (playerTransform.position - obj.transform.position).normalized;
            Vector3 newPosition = CalculatePosition(obj.transform.position, currentDirection, pattern, data, elapsedTime);
            obj.transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        movementCoroutines.Remove(obj);
    }

    private Vector3 CalculatePosition(Vector3 currentPos, Vector3 direction, MovementType pattern, MovementData data, float time)
    {
        // 기본 이동을 현재 위치 기준으로 계산
        Vector3 basePosition = currentPos + direction * data.speed * Time.deltaTime;

        switch (pattern)
        {
            case MovementType.Straight:
                return basePosition;

            case MovementType.Zigzag:
                float zigzag = Mathf.Sin(time * data.frequency) * data.amplitude;
                return basePosition + Vector3.right * zigzag * Time.deltaTime;

            case MovementType.Spiral:
                float spiral = time * data.frequency;
                float x = Mathf.Cos(spiral) * data.amplitude;
                float y = Mathf.Sin(spiral) * data.amplitude;
                return basePosition + new Vector3(x, y, 0) * Time.deltaTime;

            default:
                return basePosition;
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