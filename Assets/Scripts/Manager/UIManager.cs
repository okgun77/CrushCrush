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
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;

    [SerializeField] private Color blinkColor = Color.red;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private int blinkCount = 3;
    private GameManager gameManager;

    private Coroutine blinkCoroutine;

    public void Init(GameManager gm)
    {
        gameManager = gm;
        if (hpSlider != null)
        {
            hpSlider.maxValue = 100;
            hpSlider.value = 100;
            hpSlider.interactable = false;
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
                StartCoroutine(SmoothSliderChange(hpSlider, hpSlider.value, currentHP, 0.5f));
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

    public void BlinkHPSlider()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkCoroutine(blinkColor, blinkDuration, blinkCount));
    }

    private IEnumerator BlinkCoroutine(Color blinkColor, float duration, int blinkCount)
    {
        Color originalColor = fillImage.color;
        float halfDuration = duration / (blinkCount * 2);

        for (int i = 0; i < blinkCount; i++)
        {
            fillImage.color = blinkColor;
            yield return new WaitForSeconds(halfDuration);
            fillImage.color = originalColor;
            yield return new WaitForSeconds(halfDuration);
        }

        fillImage.color = originalColor;
    }
}
