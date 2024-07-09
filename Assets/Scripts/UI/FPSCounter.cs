using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.5f;

    private float accumulatedFPS;
    private int frames;
    private float timeLeft;

    private void Start()
    {
        if (fpsText == null)
        {
            Debug.LogError("FPS 텍스트를 설정해 주세요");
            enabled = false;
            return;
        }

        timeLeft = updateInterval;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        accumulatedFPS += Time.timeScale / Time.deltaTime;
        frames++;

        // 업데이트 간격이 지나면 FPS를 계산하여 텍스트에 표시
        if (timeLeft <= 0.0f)
        {
            float fps = accumulatedFPS / frames;
            fpsText.text = $"FPS: {fps:F2}";

            // 초기화
            timeLeft = updateInterval;
            accumulatedFPS = 0.0f;
            frames = 0;
        }
    }
}
