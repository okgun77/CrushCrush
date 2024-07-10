using UnityEngine;
using System;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField] private KeyCode activateSlowMotionKey = KeyCode.F;
    [SerializeField] private KeyCode deactivateSlowMotionKey = KeyCode.D;
    [SerializeField] private float slowMotionScale = 0.2f;
    [SerializeField] private float normalTimeScale = 1.0f;
    [SerializeField] private float transitionSpeed = 2.0f;

    private bool isSlowMotionActive = false;
    private Action<bool> onSlowMotionChanged;

    public void Init(Action<bool> _slowMotionChangedCallback)
    {
        onSlowMotionChanged = _slowMotionChangedCallback;
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateSlowMotionKey))
        {
            ActivateSlowMotion();
        }
        else if (Input.GetKeyDown(deactivateSlowMotionKey))
        {
            DeactivateSlowMotion();
        }

        float targetTimeScale = isSlowMotionActive ? slowMotionScale : normalTimeScale;
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.deltaTime * transitionSpeed);

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        FindObjectOfType<GameManager>().UpdateTimeScale(Time.timeScale);
    }

    public void ToggleSlowMotion()
    {
        if (isSlowMotionActive)
        {
            DeactivateSlowMotion();
        }
        else
        {
            ActivateSlowMotion();
        }
    }

    private void ActivateSlowMotion()
    {
        isSlowMotionActive = true;
        onSlowMotionChanged?.Invoke(true);
    }

    private void DeactivateSlowMotion()
    {
        isSlowMotionActive = false;
        onSlowMotionChanged?.Invoke(false);
    }
}
