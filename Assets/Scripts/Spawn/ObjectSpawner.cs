using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject prefab;
        public float spawnWeight = 1f;
    }

    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float defaultSpawnWeight = 1f;
    
    private SpawnableItem[] spawnableItems;
    
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float spawnInterval = 1f;
    
    private Transform playerTransform;
    private float totalWeight;
    
    private void Awake()
    {
        InitializeSpawnableItems();
    }
    
    private void InitializeSpawnableItems()
    {
        spawnableItems = new SpawnableItem[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            spawnableItems[i] = new SpawnableItem
            {
                prefab = prefabs[i],
                spawnWeight = defaultSpawnWeight
            };
        }
    }
    
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        CalculateTotalWeight();
        StartCoroutine(SpawnRoutine());
    }
    
    private void CalculateTotalWeight()
    {
        totalWeight = 0f;
        foreach (var item in spawnableItems)
        {
            totalWeight += item.spawnWeight;
        }
    }
    
    private GameObject SelectRandomPrefab()
    {
        float random = Random.Range(0f, totalWeight);
        float weightSum = 0f;
        
        foreach (var item in spawnableItems)
        {
            weightSum += item.spawnWeight;
            if (random <= weightSum)
            {
                return item.prefab;
            }
        }
        
        return spawnableItems[0].prefab;
    }
    
    private void SpawnObject()
    {
        // 플레이어의 forward 방향을 기준으로 스폰 위치 계산
        Vector3 playerForward = playerTransform.forward;
        Vector3 spawnCenter = playerTransform.position + playerForward * spawnDistance;
        
        // 플레이어의 forward 방향에 수직인 평면상에서 랜덤 위치 계산
        Vector3 randomOffset = Vector3.zero;
        if (playerForward != Vector3.up && playerForward != Vector3.down)
        {
            Vector3 right = Vector3.Cross(playerForward, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(right, playerForward).normalized;
            
            float randomX = Random.Range(-spawnRadius, spawnRadius);
            float randomY = Random.Range(-spawnRadius, spawnRadius);
            
            randomOffset = right * randomX + up * randomY;
        }
        
        Vector3 spawnPosition = spawnCenter + randomOffset;
        GameObject prefabToSpawn = SelectRandomPrefab();
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
