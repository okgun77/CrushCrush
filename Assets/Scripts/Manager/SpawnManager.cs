using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObject
{
    public GameObject prefab;
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private SpawnObject[] spawnObjects; // 스폰할 오브젝트 배열
    [SerializeField] private Transform[] spawnPoints; // 스폰 영역의 중심점들
    [SerializeField] private float spawnRange = 10f; // 스폰 영역의 반경
    [SerializeField] private Transform targetPoint; // 타겟 위치
    [SerializeField] private float spawnInterval = 2.0f; // 스폰 간격

    private int lastSpawnPointIndex = -1; // 마지막 스폰 포인트 인덱스
    private List<int> availableIndexes = new List<int>(); // 사용 가능한 인덱스 리스트
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 스폰된 오브젝트 리스트

    public void Init()
    {
        InitIndexes();
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }

    private void InitIndexes()
    {
        for (int i = 0; i < spawnObjects.Length; i++)
        {
            availableIndexes.Add(i);
        }
    }

    private void SpawnObject()
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

        // 순차적으로 스폰 포인트 선택
        lastSpawnPointIndex = (lastSpawnPointIndex + 1) % spawnPoints.Length;
        Transform selectedSpawnPoint = spawnPoints[lastSpawnPointIndex];

        // 스폰 영역 내에서 랜덤한 오프셋 생성
        Vector3 randomOffset = new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange));
        Vector3 spawnPosition = selectedSpawnPoint.position + randomOffset;

        // 오브젝트 스폰
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        MoveToTargetPoint moveScript = spawnedObject.GetComponent<MoveToTargetPoint>();
        if (moveScript == null)
        {
            moveScript = spawnedObject.AddComponent<MoveToTargetPoint>();
        }
        moveScript.SetTarget(targetPoint);

        // 스폰된 오브젝트 리스트에 추가
        spawnedObjects.Add(spawnedObject);
    }

    public List<GameObject> GetSpawnedObjects()
    {
        // 유효하지 않은 오브젝트를 리스트에서 제거
        spawnedObjects.RemoveAll(obj => obj == null);
        return spawnedObjects;
    }

    private GameObject GetRandomObject()
    {
        if (availableIndexes.Count == 0)
        {
            InitIndexes(); // 모든 인덱스를 사용했으면 다시 초기화
        }

        int randomIndex = Random.Range(0, availableIndexes.Count);
        int objectIndex = availableIndexes[randomIndex];
        availableIndexes.RemoveAt(randomIndex); // 선택된 인덱스를 리스트에서 제거

        return spawnObjects[objectIndex].prefab;
    }
}
