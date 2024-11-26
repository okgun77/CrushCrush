using UnityEngine;
using System.Collections.Generic;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float warningDistance = 5f;
    
    private SpawnManager spawnManager;
    private WarningManager warningManager;
    
    private void Start()
    {
        spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogWarning("SpawnManager를 찾을 수 없습니다. 독립적으로 동작합니다.");
        }

        warningManager = FindAnyObjectByType<WarningManager>();
        if (warningManager == null)
        {
            Debug.LogWarning("WarningManager를 찾을 수 없습니다. 경고 기능을 비활성화합니다.");
        }
    }

    private void Update()
    {
        if (spawnManager != null)
        {
            CheckObjectsInZone();
        }
    }

    private void CheckObjectsInZone()
    {
        if (spawnManager == null) return;

        List<GameObject> activeObjects = spawnManager.GetActiveObjects();

        foreach (GameObject obj in activeObjects)
        {
            BreakObject breakObject = obj.GetComponent<BreakObject>();
            if (breakObject == null) continue;

            if (warningManager != null)
            {
                if (IsObjectCloseToDamageZone(obj))
                {
                    warningManager.ApplyWarningEffect(breakObject);
                }
                else
                {
                    warningManager.RemoveWarningEffect(breakObject);
                }
            }

            if (IsObjectPassed(obj))
            {
                // 플레이어 찾기
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    var playerHealth = player.GetComponent<IDamageable>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damageAmount);
                    }
                }
                
                obj.SetActive(false);
            }
        }
    }

    private bool IsObjectPassed(GameObject _object)
    {
        return _object.transform.position.z < transform.position.z;
    }

    private bool IsObjectCloseToDamageZone(GameObject _object)
    {
        return Vector3.Distance(_object.transform.position, transform.position) <= warningDistance;
    }
}
