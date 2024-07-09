using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject slowMotionPanel;
    [SerializeField] private TextMeshProUGUI timeScaleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private GameObject gameOverPanel;

    public void Init()
    {
        // 초기화 로직
    }

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

    public void UpdateHPText(int hp)
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {hp}";
        }
    }

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
