using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab; // 스폰할 오브젝트 프리팹
    [SerializeField] private Transform spawnArea; // 스폰 영역의 중심점
    [SerializeField] private float spawnRange = 10f; // 스폰 영역의 반경
    [SerializeField] private Transform targetPoint; // 타겟 위치
    [SerializeField] private float spawnInterval = 2.0f; // 스폰 간격

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }

    private void SpawnObject()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange));
        Vector3 spawnPosition = spawnArea.position + randomOffset;

        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        MoveToTargetPoint moveScript = spawnedObject.GetComponent<MoveToTargetPoint>();
        if (moveScript == null)
        {
            moveScript = spawnedObject.AddComponent<MoveToTargetPoint>();
        }
        moveScript.SetTarget(targetPoint);
    }
}
