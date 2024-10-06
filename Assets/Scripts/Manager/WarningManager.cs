using UnityEngine;
using System.Collections.Generic;
using HighlightPlus;

public class WarningManager : MonoBehaviour
{
    private Dictionary<BreakObject, HighlightEffect> activeWarnings = new Dictionary<BreakObject, HighlightEffect>();

    public void ApplyWarningEffect(BreakObject _targetObject)
    {
        if (_targetObject == null)
        {
            Debug.LogError("Target Object is not set!");
            return;
        }

        HighlightEffect highlightEffect = _targetObject.GetComponent<HighlightEffect>();
        if (highlightEffect == null)
        {
            Debug.LogError("HighlightEffect component is not attached to the target object!");
            return;
        }

        if (!activeWarnings.ContainsKey(_targetObject))
        {
            highlightEffect.enabled = true;
            activeWarnings.Add(_targetObject, highlightEffect);
            _targetObject.SetWarningState(true);
        }
    }

    public void RemoveWarningEffect(BreakObject _targetObject)
    {
        if (_targetObject == null) return;

        if (activeWarnings.ContainsKey(_targetObject))
        {
            HighlightEffect highlightEffect = activeWarnings[_targetObject];
            if (highlightEffect != null)
            {
                highlightEffect.enabled = false;
            }
            activeWarnings.Remove(_targetObject);
            _targetObject.SetWarningState(false);
        }
    }
}
