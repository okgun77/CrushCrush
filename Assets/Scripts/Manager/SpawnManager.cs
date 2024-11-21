using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private SpawnableItemData spawnableItemData;
    private List<SpawnableItem> spawnableItems => spawnableItemData.items;
    
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float spawnRadius = 10f;
    
    [Header("Spawn Interval Settings")]
    [SerializeField] private float initialSpawnInterval = 5.0f;
    [SerializeField] private float minimumSpawnInterval = 1.0f;
    [SerializeField] private float spawnIntervalDecreaseRate = 0.1f;

    private Transform playerTransform;
    private GameManager gameManager;
    private ObjectPoolManager poolManager;
    private ObjectMovementManager movementManager;
    
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
                    ObjectMovementManager movementManager, Transform playerTransform)
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
        
        try
        {
            if (movementManager == null)
            {
                Debug.LogError("MovementManager is null in SpawnManager!");
                return;
            }
            
            MovementData movementData = GenerateMovementData();
            MovementType movementType = GetMovementTypeForObject(selectedItem.objectType);
            Debug.Log($"Assigning movement pattern: {movementType} to {spawnedObject.name}");
            movementManager.AssignMovementPattern(spawnedObject, movementType, movementData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error assigning movement pattern: {e.Message}\n{e.StackTrace}");
        }
        
        activeObjects.Add(spawnedObject);
        CleanInactiveObjects();
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
        Vector3 playerForward = playerTransform.forward;
        Vector3 spawnCenter = playerTransform.position + playerForward * spawnDistance;
        
        Vector3 right = Vector3.Cross(playerForward, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, playerForward).normalized;
        
        float randomX = Random.Range(-spawnRadius, spawnRadius);
        float randomY = Random.Range(-spawnRadius, spawnRadius);
        
        return spawnCenter + right * randomX + up * randomY;
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
        spawnRadius = radius;
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

