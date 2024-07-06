using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject slowMotionPanel; // 슬로우모션 패널
    [SerializeField] private TextMeshProUGUI timeScaleText; // 타임스케일을 표시할 TextMeshPro 텍스트
    [SerializeField] private TextMeshProUGUI scoreText; // 점수를 표시할 TextMeshPro 텍스트

    public void ShowSlowMotionPanel()
    {
        if (slowMotionPanel != null)
        {
            slowMotionPanel.SetActive(true);
        }
    }

    public void HideSlowMotionPanel()
    {
        if (slowMotionPanel != null)
        {
            slowMotionPanel.SetActive(false);
        }
    }

    public void UpdateTimeScaleText(float timeScale)
    {
        if (timeScaleText != null)
        {
            timeScaleText.text = $"Time Scale: {timeScale:F2}";
        }
    }

    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}
