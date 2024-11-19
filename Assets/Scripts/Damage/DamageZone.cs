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
        hpManager = FindAnyObjectByType<HPManager>();
        if (hpManager == null)
        {
            Debug.LogError("HPManager를 찾을 수 없습니다!");
        }

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
        // SpawnManager가 없을 경우 함수 실행 중단
        if (spawnManager == null) return;

        // SpawnManager를 통해 활성화된 오브젝트 리스트를 가져옴
        List<GameObject> activeObjects = spawnManager.GetActiveObjects();

        foreach (GameObject obj in activeObjects)
        {
            // BreakObject로 수정
            BreakObject breakObject = obj.GetComponent<BreakObject>();
            if (breakObject == null) continue;

            // WarningManager가 있으면 경고 효과 적용
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
                hpManager.TakeDamage(damageAmount);
                obj.SetActive(false); // Destroy 대신 비활성화 (오브젝트 풀링 사용)
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
