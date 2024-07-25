using UnityEngine;
using System.Collections.Generic;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float warningDistance = 5f; // 경고를 활성화할 거리
    private HPManager hpManager;
    private SpawnManager spawnManager;
    private WarningManager warningManager;

    private void Start()
    {
        hpManager = FindObjectOfType<HPManager>();
        if (hpManager == null)
        {
            Debug.LogError("HPManager를 찾을 수 없습니다!");
        }

        spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager를 찾을 수 없습니다!");
        }

        warningManager = FindObjectOfType<WarningManager>();
        if (warningManager == null)
        {
            Debug.LogError("WarningManager를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        CheckObjectsInZone();
    }

    private void CheckObjectsInZone()
    {
        // SpawnManager를 통해 스폰된 오브젝트 리스트를 가져옴
        List<GameObject> breakableObjects = spawnManager.GetSpawnedObjects();

        foreach (GameObject obj in breakableObjects)
        {
            if (IsObjectCloseToDamageZone(obj))
            {
                warningManager.ApplyWarningEffect(obj.GetComponent<BreakableObject>());
            }
            else
            {
                warningManager.RemoveWarningEffect(obj.GetComponent<BreakableObject>());
            }

            if (IsObjectPassed(obj))
            {
                hpManager.TakeDamage(damageAmount);
                Destroy(obj);
            }
        }
    }

    private bool IsObjectCloseToDamageZone(GameObject _object)
    {
        // 오브젝트가 데미지 존의 z 위치에 가까워졌는지 체크
        return Vector3.Distance(_object.transform.position, transform.position) <= warningDistance;
    }

    private bool IsObjectPassed(GameObject _object)
    {
        // 오브젝트가 데미지 존의 z 위치를 넘어갔는지 체크
        return _object.transform.position.z < transform.position.z;
    }
}
