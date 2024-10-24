using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObject
{
    public GameObject prefab;
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private SpawnObject[] spawnObjects;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRange = 10f;

    [SerializeField] private float initialSpawnInterval = 5.0f; // 초기 스폰 간격
    [SerializeField] private float minimumSpawnInterval = 1.0f; // 최소 스폰 간격
    [SerializeField] private float spawnIntervalDecreaseRate = 0.1f; // 스폰 간격 감소량

    private GameManager gameManager; // GameManager 참조는 선택적
    private Coroutine spawnCoroutine;
    private bool isPaused = false;

    private int lastSpawnPointIndex = -1;
    private List<int> availableIndexes = new List<int>();
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();  // GameManager가 존재할 경우만 찾음
        InitIndexes();
        StartSpawning(); // 스폰 시작
    }

    private void InitIndexes()
    {
        for (int i = 0; i < spawnObjects.Length; i++)
        {
            availableIndexes.Add(i);
        }
    }

    private IEnumerator SpawnObjectCoroutine()
    {
        float currentSpawnInterval = initialSpawnInterval;

        while (!isPaused)
        {
            SpawnObjectMethod();
            yield return new WaitForSeconds(currentSpawnInterval);

            if (currentSpawnInterval > minimumSpawnInterval)
            {
                currentSpawnInterval -= spawnIntervalDecreaseRate;
                currentSpawnInterval = Mathf.Max(currentSpawnInterval, minimumSpawnInterval);
            }
        }
    }

    private void SpawnObjectMethod()
    {
        if (spawnPoints.Length == 0 || spawnObjects.Length == 0)
        {
            Debug.LogWarning("Spawn points or spawn objects are not set!");
            return;
        }

        GameObject objectToSpawn = GetRandomObject();
        if (objectToSpawn == null)
        {
            Debug.LogWarning("No object to spawn!");
            return;
        }

        lastSpawnPointIndex = (lastSpawnPointIndex + 1) % spawnPoints.Length;
        Transform selectedSpawnPoint = spawnPoints[lastSpawnPointIndex];

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRange, spawnRange),
            Random.Range(-spawnRange, spawnRange),
            Random.Range(-spawnRange, spawnRange)
        );
        Vector3 spawnPosition = selectedSpawnPoint.position + randomOffset;

        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

        // MoveManager를 통한 이동 관리 (선택적으로 사용)
        if (gameManager != null)
        {
            var moveManager = gameManager.GetMoveManager();
            if (moveManager != null)
            {
                moveManager.ApplyMovements(spawnedObject);
            }
        }

        spawnedObjects.Add(spawnedObject);
    }

    public List<GameObject> GetSpawnedObjects()
    {
        spawnedObjects.RemoveAll(obj => obj == null);
        return spawnedObjects;
    }

    private GameObject GetRandomObject()
    {
        if (availableIndexes.Count == 0)
        {
            InitIndexes();
        }

        int randomIndex = Random.Range(0, availableIndexes.Count);
        int objectIndex = availableIndexes[randomIndex];
        availableIndexes.RemoveAt(randomIndex);

        return spawnObjects[objectIndex].prefab;
    }

    public void StartSpawning()
    {
        isPaused = false;
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnObjectCoroutine());
        }
    }

    public void StopSpawning()
    {
        isPaused = true;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}
