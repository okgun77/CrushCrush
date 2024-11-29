using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private SpawnManager spawnManager;

    private void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found in parent!");
        }

        spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager not found!");
        }
    }

    private void Update()
    {
        if (spawnManager == null || playerHealth == null) return;
        CheckCollisionsWithSpawnedObjects();
    }

    private void CheckCollisionsWithSpawnedObjects()
    {
        var activeObjects = spawnManager.GetActiveObjects();
        if (activeObjects == null) return;

        foreach (var obj in activeObjects)
        {
            if (obj == null) continue;

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < 2f)
            {
                var properties = obj.GetComponent<ObjectProperties>();
                if (properties != null)
                {
                    float damage = properties.GetAttackDamage();
                    Debug.Log($"Applying damage to player: {damage}");
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Player health after damage: {playerHealth.GetCurrentHealthPercent() * 100}%");
                    
                    // 오브젝트 풀에 반환
                    ObjectPoolManager.Instance.ReturnObject(obj);
                }
            }
        }
    }
}
