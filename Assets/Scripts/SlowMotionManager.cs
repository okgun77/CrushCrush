using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField] private KeyCode activateSlowMotionKey = KeyCode.F; // ¬ë¡œ°ëª¨˜ì„ œì„±”í•˜
    [SerializeField] private KeyCode deactivateSlowMotionKey = KeyCode.D; // ¬ë¡œ°ëª¨˜ì„ ë¹„í™œ±í™”˜ëŠ” 
    [SerializeField] private float slowMotionScale = 0.2f; // ¬ë¡œ°ëª¨íƒœì„œ€„ìŠ¤ì¼€
    [SerializeField] private float normalTimeScale = 1.0f; // ¼ë°˜ íƒœì„œ€„ìŠ¤ì¼€
    [SerializeField] private float transitionSpeed = 2.0f; // €„ìŠ¤ì¼€„í™˜ ë„

    private bool isSlowMotionActive = false; // ¬ë¡œ°ëª¨íƒœë¥ì¶”ì ˜ëŠ” ë³€
    private UIManager uiManager; // UIManager ì°¸ì¡°

    private void Start()
    {
        // UIManager ì»´í¬ŒíŠ¸ ê°€¸ì˜¤ê¸
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManagerë¥ì°¾ì„ †ìŠµˆë‹¤!");
        }
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

        // €„ìŠ¤ì¼€ë³€ê²½ì— °ë¼ ê³ ì • ¸í €„ë„ ì¡°ì •
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // UIManager€„ìŠ¤ì¼€…ë°´íŠ¸ ”ì²­
        uiManager?.UpdateTimeScaleText(Time.timeScale);
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
        uiManager?.ShowSlowMotionPanel(); // ¬ë¡œ°ëª¨¨ë„ œì„±
    }

    private void DeactivateSlowMotion()
    {
        isSlowMotionActive = false;
        uiManager?.HideSlowMotionPanel(); // ¬ë¡œ°ëª¨¨ë„ ë¹„í™œ±í™”
    }
}
