using UnityEngine;

[System.Serializable]
public class SpawnObject
{
    public GameObject prefab;
    public float spawnRate; // 스폰 확률
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private SpawnObject[] spawnObjects; // 스폰할 오브젝트 배열
    [SerializeField] private Transform[] spawnPoints; // 스폰 영역의 중심점들
    [SerializeField] private float spawnRange = 10f; // 스폰 영역의 반경
    [SerializeField] private Transform targetPoint; // 타겟 위치
    [SerializeField] private float spawnInterval = 2.0f; // 스폰 간격

    private int lastSpawnPointIndex = -1; // 마지막 스폰 포인트 인덱스
    private float totalSpawnRate; // 총 스폰 확률

    private void Start()
    {
        CalculateTotalSpawnRate();
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }

    private void CalculateTotalSpawnRate()
    {
        totalSpawnRate = 0f;
        foreach (var spawnObject in spawnObjects)
        {
            totalSpawnRate += spawnObject.spawnRate;
        }
    }

    private void SpawnObject()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn points are not set!");
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
    }

    private GameObject GetRandomObject()
    {
        float randomPoint = Random.value * totalSpawnRate;
        float cumulativeRate = 0f;

        foreach (var spawnObject in spawnObjects)
        {
            cumulativeRate += spawnObject.spawnRate;
            if (randomPoint <= cumulativeRate)
            {
                return spawnObject.prefab;
            }
        }
        return null;
    }
}
