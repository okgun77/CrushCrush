using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private SpawnableItemData spawnableItemData;
    private List<SpawnableItem> spawnableItems => spawnableItemData.items;
    
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float spawnWidthRatio = 0.8f; // 화면 너비의 몇 %를 사용할지
    [SerializeField] private float spawnHeightRatio = 0.6f; // 화면 높이의 몇 %를 사용할지
    
    [Header("Spawn Interval Settings")]
    [SerializeField] private float initialSpawnInterval = 5.0f;
    [SerializeField] private float minimumSpawnInterval = 1.0f;
    [SerializeField] private float spawnIntervalDecreaseRate = 0.1f;

    private Transform playerTransform;
    private GameManager gameManager;
    private ObjectPoolManager poolManager;
    private MovementManager movementManager;
    
    private float totalWeight;
    private float currentSpawnInterval;
    private Coroutine spawnCoroutine;
    private bool isPaused = false;
    private List<GameObject> activeObjects = new List<GameObject>();

    [Header("Movement Settings")]
    private MovementType[] currentAvailablePatterns;
    private MovementData currentMovementData;
    private bool isSpawning = false;

    public void Init(GameManager gameManager, ObjectPoolManager poolManager, 
                    MovementManager movementManager, Transform playerTransform)
    {
        this.gameManager = gameManager;
        this.poolManager = poolManager;
        this.movementManager = movementManager;
        this.playerTransform = playerTransform;
        
        Initialize();
    }

    private void Initialize()
    {
        CalculateTotalWeight();
        currentSpawnInterval = initialSpawnInterval;
    }

    private void CalculateTotalWeight()
    {
        totalWeight = 0f;
        foreach (var item in spawnableItems)
        {
            totalWeight += item.spawnWeight;
        }
    }

    public void StartSpawning()
    {
        if (spawnableItems == null || spawnableItems.Count == 0)
        {
            Debug.LogWarning("No items to spawn!");
            return;
        }

        Debug.Log("Starting spawn routine...");
        isSpawning = true;
        if (!isPaused && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
    }

    public void PauseSpawning()
    {
        isPaused = true;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    public void ResumeSpawning()
    {
        isPaused = false;
        StartSpawning();
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (!isPaused)
        {
            SpawnObject();
            yield return new WaitForSeconds(currentSpawnInterval);

            // 스폰 간격 감소
            if (currentSpawnInterval > minimumSpawnInterval)
            {
                currentSpawnInterval -= spawnIntervalDecreaseRate;
                currentSpawnInterval = Mathf.Max(currentSpawnInterval, minimumSpawnInterval);
            }
        }
    }

    private void SpawnObject()
    {
        if (spawnableItems == null || spawnableItems.Count == 0)
        {
            Debug.LogWarning("No spawnable items available!");
            return;
        }

        if (movementManager == null)
        {
            Debug.LogError("MovementManager is not assigned!");
            return;
        }

        Debug.Log($"Total Weight: {totalWeight}");
        SpawnableItem selectedItem = SelectRandomItem();
        if (selectedItem == null || selectedItem.prefab == null)
        {
            Debug.LogWarning("Selected item or prefab is null!");
            return;
        }
 
        Vector3 spawnPosition = CalculateSpawnPosition();
        Debug.Log($"Spawning {selectedItem.prefab.name} at {spawnPosition}");
        
        GameObject spawnedObject = poolManager.GetObject(selectedItem.prefab);
        if (spawnedObject == null)
        {
            Debug.LogWarning("Failed to get object from pool!");
            return;
        }

        spawnedObject.transform.position = spawnPosition;
        
        // 스폰 효과 자동 적용
        SpawnFadeEffect spawnEffect = spawnedObject.GetComponent<SpawnFadeEffect>();
        if (spawnEffect == null)
        {
            spawnEffect = spawnedObject.AddComponent<SpawnFadeEffect>();
        }
        spawnEffect.StartEffect();
        
        spawnedObject.SetActive(true);
        activeObjects.Add(spawnedObject);

        MovementType pattern = GetMovementTypeForObject(selectedItem.objectType);
        Debug.Log($"Assigning movement pattern: {pattern} to {spawnedObject.name}");

        MovementData movementData = GenerateMovementData();
        
        if (movementManager != null)
        {
            Debug.Log($"Starting movement for {spawnedObject.name} with pattern {pattern}");
            movementManager.StartMovement(spawnedObject, pattern, movementData, playerTransform.position);
        }
        else
        {
            Debug.LogError("MovementManager is null!");
        }
    }

    private SpawnableItem SelectRandomItem()
    {
        float random = Random.Range(0f, totalWeight);
        float weightSum = 0f;
        
        foreach (var item in spawnableItems)
        {
            weightSum += item.spawnWeight;
            if (random <= weightSum)
            {
                return item;
            }
        }
        
        return spawnableItems[0];
    }

    private Vector3 CalculateSpawnPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return Vector3.zero;
        }

        // 스폰 중심점 계산
        Vector3 playerForward = playerTransform.forward;
        Vector3 spawnCenter = playerTransform.position + playerForward * spawnDistance;

        // 카메라의 뷰포트 상의 스폰 가능 영역 계산
        float viewportWidth = spawnWidthRatio;
        float viewportHeight = spawnHeightRatio;
        
        // 랜덤한 뷰포트 좌표 생성 (-0.5 ~ 0.5 범위로 조정)
        float randomX = Random.Range(-viewportWidth/2, viewportWidth/2);
        float randomY = Random.Range(-viewportHeight/2, viewportHeight/2);

        // 스폰 중심점에서의 뷰포트 좌표
        Vector3 spawnCenterViewport = mainCamera.WorldToViewportPoint(spawnCenter);
        
        // 랜덤 위치의 뷰포트 좌표
        Vector3 randomViewport = new Vector3(
            spawnCenterViewport.x + randomX,
            spawnCenterViewport.y + randomY,
            spawnCenterViewport.z
        );

        // 뷰포트 좌표를 월드 좌표로 변환
        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(randomViewport);
        
        // z 좌표는 원래 스폰 거리를 유지
        spawnPosition.z = spawnCenter.z;

        return spawnPosition;
    }

    private MovementData GenerateMovementData()
    {
        if (currentMovementData.speed != 0 || 
            currentMovementData.amplitude != 0 || 
            currentMovementData.frequency != 0 || 
            currentMovementData.duration != 0)
        {
            return currentMovementData;
        }

        return new MovementData
        {
            speed = Random.Range(5f, 10f),
            amplitude = Random.Range(1f, 3f),
            frequency = Random.Range(1f, 3f),
            duration = Random.Range(5f, 10f)
        };
    }

    private MovementType GetMovementTypeForObject(ObjectTypes objectType)
    {
        if (currentAvailablePatterns != null && currentAvailablePatterns.Length > 0)
        {
            return currentAvailablePatterns[Random.Range(0, currentAvailablePatterns.Length)];
        }

        return objectType switch
        {
            ObjectTypes.BASIC => MovementType.Straight,
            ObjectTypes.EXPLOSIVE => MovementType.Zigzag,
            ObjectTypes.INDESTRUCTIBLE => MovementType.Spiral,
            _ => MovementType.Straight
        };
    }

    private void CleanInactiveObjects()
    {
        activeObjects.RemoveAll(obj => obj == null || !obj.activeInHierarchy);
    }

    public List<GameObject> GetActiveObjects()
    {
        CleanInactiveObjects();
        return activeObjects;
    }

    private void OnDestroy()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    // 난이도 조절을 위한 메서드들
    public void SetSpawnInterval(float interval)
    {
        currentSpawnInterval = Mathf.Max(interval, minimumSpawnInterval);
    }

    public void SetSpawnDistance(float distance)
    {
        spawnDistance = distance;
    }

    public void SetSpawnRadius(float radius)
    {
        spawnWidthRatio = radius;
        spawnHeightRatio = radius;
    }

    public void SetMaxObjectCount(int count)
    {
        // 최대 오브젝트 수 설정
    }

    public void SetAvailablePatterns(MovementType[] patterns)
    {
        currentAvailablePatterns = patterns;
    }

    public void SetMovementSpeed(float speed)
    {
        if (currentMovementData.Equals(default(MovementData)))
        {
            currentMovementData = new MovementData();
        }
        currentMovementData.speed = speed;
    }

    public void SetPatternAmplitude(float amplitude)
    {
        if (currentMovementData.Equals(default(MovementData)))
        {
            currentMovementData = new MovementData();
        }
        currentMovementData.amplitude = amplitude;
    }

    public void SetPatternFrequency(float frequency)
    {
        if (currentMovementData.Equals(default(MovementData)))
        {
            currentMovementData = new MovementData();
        }
        currentMovementData.frequency = frequency;
    }

    public float GetCurrentSpawnInterval()
    {
        return currentSpawnInterval;
    }

    // 추가: spawnableItemData에 접근하기 위한 public 메서드
    public List<SpawnableItem> GetSpawnableItems()
    {
        return spawnableItemData?.items ?? new List<SpawnableItem>();
    }

    public void UpdateSpawnSettings(float interval, MovementType[] patterns, MovementData data)
    {
        currentSpawnInterval = interval;
        currentAvailablePatterns = patterns;
        currentMovementData = data;
        
        if (isSpawning)
        {
            StopSpawning();
            StartSpawning();
        }
    }
}
