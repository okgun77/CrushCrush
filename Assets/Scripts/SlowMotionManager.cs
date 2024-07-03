using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField] private KeyCode activateSlowMotionKey = KeyCode.F; // �로�모�을 �성�하
    [SerializeField] private KeyCode deactivateSlowMotionKey = KeyCode.D; // �로�모�을 비활�화�는 
    [SerializeField] private float slowMotionScale = 0.2f; // �로�모�태�서��스케
    [SerializeField] private float normalTimeScale = 1.0f; // �반 �태�서��스케
    [SerializeField] private float transitionSpeed = 2.0f; // ��스케�환 �도

    private bool isSlowMotionActive = false; // �로�모�태�추적�는 변
    private UIManager uiManager; // UIManager 참조

    private void Start()
    {
        // UIManager 컴포�트 가�오�
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager�찾을 �습�다!");
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

        // ��스케변경에 �라 고정 �� ��도 조정
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // UIManager��스케�데�트 �청
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
        uiManager?.ShowSlowMotionPanel(); // �로�모�널 �성
    }

    private void DeactivateSlowMotion()
    {
        isSlowMotionActive = false;
        uiManager?.HideSlowMotionPanel(); // �로�모�널 비활�화
    }
}
