using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField] private KeyCode activateSlowMotionKey = KeyCode.F; // 슬로우모션을 활성화하는 키
    [SerializeField] private KeyCode deactivateSlowMotionKey = KeyCode.D; // 슬로우모션을 비활성화하는 키
    [SerializeField] private float slowMotionScale = 0.2f; // 슬로우모션 상태에서의 타임스케일
    [SerializeField] private float normalTimeScale = 1.0f; // 일반 상태에서의 타임스케일
    [SerializeField] private float transitionSpeed = 2.0f; // 타임스케일 전환 속도

    private bool isSlowMotionActive = false; // 슬로우모션 상태를 추적하는 변수
    private UIManager uiManager; // UIManager 참조

    private void Start()
    {
        // UIManager 컴포넌트 가져오기
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateSlowMotionKey))
        {
            isSlowMotionActive = true;
            uiManager?.ShowSlowMotionPanel(); // 슬로우모션 패널 활성화
        }
        else if (Input.GetKeyDown(deactivateSlowMotionKey))
        {
            isSlowMotionActive = false;
            uiManager?.HideSlowMotionPanel(); // 슬로우모션 패널 비활성화
        }

        float targetTimeScale = isSlowMotionActive ? slowMotionScale : normalTimeScale;
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.deltaTime * transitionSpeed);

        // 타임스케일 변경에 따라 고정 델타 타임도 조정
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // UIManager에 타임스케일 업데이트 요청
        uiManager?.UpdateTimeScaleText(Time.timeScale);
    }
}
