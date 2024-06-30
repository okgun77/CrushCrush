using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField] private float slowMotionScale = 0.2f; // 슬로우모션 상태에서의 타임스케일
    [SerializeField] private float normalTimeScale = 1.0f; // 일반 상태에서의 타임스케일
    [SerializeField] private float transitionSpeed = 2.0f; // 타임스케일 전환 속도

    public UIManager uiManager; // UIManager 참조
    private List<ISlowMotion> slowMotionObjects = new List<ISlowMotion>();

    private void Update()
    {
        float targetTimeScale = normalTimeScale;

        foreach (var obj in slowMotionObjects)
        {
            if (obj.IsSlowMotionActive)
            {
                targetTimeScale = slowMotionScale;
                break;
            }
        }

        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.deltaTime * transitionSpeed);

        // 타임스케일 변경에 따라 고정 델타 타임도 조정
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // UIManager에 타임스케일 업데이트 요청
        uiManager?.UpdateTimeScaleText(Time.timeScale);
    }

    public void RegisterSlowMotionObject(ISlowMotion obj)
    {
        if (!slowMotionObjects.Contains(obj))
        {
            slowMotionObjects.Add(obj);
        }
    }

    public void UnregisterSlowMotionObject(ISlowMotion obj)
    {
        if (slowMotionObjects.Contains(obj))
        {
            slowMotionObjects.Remove(obj);
        }
    }
}
