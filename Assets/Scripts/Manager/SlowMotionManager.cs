using UnityEngine;
using System;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField] private KeyCode activateSlowMotionKey = KeyCode.F;
    [SerializeField] private KeyCode deactivateSlowMotionKey = KeyCode.D;
    [SerializeField] private float slowMotionScale = 0.2f;
    [SerializeField] private float normalTimeScale = 1.0f;
    [SerializeField] private float transitionSpeed = 2.0f;
    private UIManager uiManager;
    private GameManager gameManager;

    private bool isSlowMotionActive = false;

    public void Init(GameManager gm)
    {
        gameManager = gm;
        uiManager = FindObjectOfType<UIManager>();
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
        gameManager.UpdateTimeScale(Time.timeScale);
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
        uiManager.ShowSlowMotionPanel();
    }

    private void DeactivateSlowMotion()
    {
        isSlowMotionActive = false;
        uiManager.HideSlowMotionPanel();
    }
}
