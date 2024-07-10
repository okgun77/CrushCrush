using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject slowMotionPanel;
    [SerializeField] private TextMeshProUGUI timeScaleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Slider hpSlider; // HP 슬라이더 추가

    public void Init()
    {
        // 초기화 로직
        if (hpSlider != null)
        {
            hpSlider.maxValue = 100; // 최대 HP 값 설정
            hpSlider.value = 100;    // 초기 HP 값 설정
            hpSlider.interactable = false; // 슬라이더 조작 불가 설정
        }
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

    public void UpdateTimeScaleText(float _timeScale)
    {
        if (timeScaleText != null)
        {
            timeScaleText.text = $"Time Scale: {_timeScale:F2}";
        }
    }

    public void UpdateScoreText(int _score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {_score}";
        }
    }

    public void UpdateHPUI(int currentHP, int maxHP, bool immediate = false)
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP}";
        }
        if (hpSlider != null)
        {
            if (immediate)
            {
                hpSlider.value = currentHP;
            }
            else
            {
                StartCoroutine(SmoothSliderChange(hpSlider, hpSlider.value, currentHP, 0.5f)); // 0.5초 동안 부드럽게 변경
            }
        }
    }

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private IEnumerator SmoothSliderChange(Slider slider, float fromValue, float toValue, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            slider.value = Mathf.Lerp(fromValue, toValue, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        slider.value = toValue;
    }
}
