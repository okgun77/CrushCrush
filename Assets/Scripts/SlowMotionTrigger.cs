using System.Collections;
using UnityEngine;

public class SlowMotionTrigger : MonoBehaviour, ISlowMotion
{
    [SerializeField] private float slowMotionDuration = 2.0f; // 슬로우모션 지속 시간
    [SerializeField] private SlowMotionManager slowMotionManager; // SlowMotionManager 참조

    public bool IsSlowMotionActive { get; private set; } = false;

    private void Start()
    {
        if (slowMotionManager == null)
        {
            slowMotionManager = FindObjectOfType<SlowMotionManager>();
        }
        slowMotionManager.RegisterSlowMotionObject(this);
    }

    private void OnDestroy()
    {
        slowMotionManager.UnregisterSlowMotionObject(this);
    }

    public void TriggerSlowMotion()
    {
        StartCoroutine(SlowMotionCoroutine());
    }

    private IEnumerator SlowMotionCoroutine()
    {
        IsSlowMotionActive = true;
        slowMotionManager.uiManager?.ShowSlowMotionPanel(); // 슬로우모션 패널 활성화

        yield return new WaitForSecondsRealtime(slowMotionDuration);

        IsSlowMotionActive = false;
        slowMotionManager.uiManager?.HideSlowMotionPanel(); // 슬로우모션 패널 비활성화
    }
}
