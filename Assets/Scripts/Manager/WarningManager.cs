using UnityEngine;
using System.Collections.Generic;
using HighlightPlus;

public class WarningManager : MonoBehaviour
{
    private Dictionary<BreakableObject, HighlightEffect> activeWarnings = new Dictionary<BreakableObject, HighlightEffect>();

    public void ApplyWarningEffect(BreakableObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object is not set!");
            return;
        }

        HighlightEffect highlightEffect = targetObject.GetComponent<HighlightEffect>();
        if (highlightEffect == null)
        {
            Debug.LogError("HighlightEffect component is not attached to the target object!");
            return;
        }

        if (!activeWarnings.ContainsKey(targetObject))
        {
            highlightEffect.enabled = true;
            activeWarnings.Add(targetObject, highlightEffect);
            targetObject.SetWarningState(true);
        }
    }

    public void RemoveWarningEffect(BreakableObject targetObject)
    {
        if (targetObject == null) return;

        if (activeWarnings.ContainsKey(targetObject))
        {
            HighlightEffect highlightEffect = activeWarnings[targetObject];
            if (highlightEffect != null)
            {
                highlightEffect.enabled = false;
            }
            activeWarnings.Remove(targetObject);
            targetObject.SetWarningState(false);
        }
    }
}
