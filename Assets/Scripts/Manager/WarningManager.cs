using UnityEngine;
using System.Collections.Generic;

public class WarningManager : MonoBehaviour
{
    [SerializeField] private GameObject warningEffectPrefab;

    private Dictionary<BreakableObject, GameObject> activeWarnings = new Dictionary<BreakableObject, GameObject>();

    public void ApplyWarningEffect(BreakableObject targetObject)
    {
        if (warningEffectPrefab == null)
        {
            Debug.LogError("Warning Effect Prefab is not set!");
            return;
        }

        if (!activeWarnings.ContainsKey(targetObject))
        {
            GameObject warningEffect = Instantiate(warningEffectPrefab, targetObject.transform.position, Quaternion.identity);
            warningEffect.transform.SetParent(targetObject.transform, false);

            // 경고 이펙트 크기 조정
            AdjustWarningEffectSize(warningEffect, targetObject);

            activeWarnings.Add(targetObject, warningEffect);
            targetObject.SetWarningState(true);
        }
    }

    public void RemoveWarningEffect(BreakableObject targetObject)
    {
        if (activeWarnings.ContainsKey(targetObject))
        {
            Destroy(activeWarnings[targetObject]);
            activeWarnings.Remove(targetObject);
            targetObject.SetWarningState(false);
        }
    }

    private void AdjustWarningEffectSize(GameObject warningEffect, BreakableObject targetObject)
    {
        Renderer targetRenderer = targetObject.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            Vector3 objectSize = targetRenderer.bounds.size;
            warningEffect.transform.localScale = objectSize * 1.2f; // 오브젝트 크기보다 약간 더 크게 설정
        }
    }
}
