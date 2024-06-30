using UnityEngine;

public class SlowMotionGame : MonoBehaviour, ISlowMotion
{
    [SerializeField] private KeyCode activateSlowMotionKey = KeyCode.F; // 슬로우모션을 활성화하는 키
    [SerializeField] private KeyCode deactivateSlowMotionKey = KeyCode.D; // 슬로우모션을 비활성화하는 키
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

    private void Update()
    {
        if (Input.GetKeyDown(activateSlowMotionKey))
        {
            IsSlowMotionActive = true;
            slowMotionManager.uiManager?.ShowSlowMotionPanel(); // 슬로우모션 패널 활성화
        }
        else if (Input.GetKeyDown(deactivateSlowMotionKey))
        {
            IsSlowMotionActive = false;
            slowMotionManager.uiManager?.HideSlowMotionPanel(); // 슬로우모션 패널 비활성화
        }
    }
}
