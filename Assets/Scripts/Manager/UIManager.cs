using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject slowMotionPanel;
    [SerializeField] private TextMeshProUGUI timeScaleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject damagePanel;
    [SerializeField] private RectTransform fragmentTargetIcon;

    [Header("Visual Effects")]
    [SerializeField] private Color damageBlinkColor = Color.red;
    [SerializeField] private Color healBlinkColor = Color.green;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private int blinkCount = 3;

    private GameManager gameManager;
    private PlayerHealth playerHealth;
    private Coroutine blinkCoroutine;
    private Coroutine damagePanelCoroutine;
    private int blinkRequests = 0;
    private ScoreManager scoreManager;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
        
        // PlayerHealth 찾기
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null && hpSlider != null)
        {
            hpSlider.maxValue = playerHealth.GetMaxHealth();
            hpSlider.value = playerHealth.GetMaxHealth();
            hpSlider.interactable = false;
            UpdateHPUI(true);
        }

        // ScoreManager 찾고 이벤트 구독
        scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += UpdateScoreText;
            // 초기 점수 표시
            UpdateScoreText(scoreManager.GetScore());
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= UpdateScoreText;
        }
    }

    #region Panel Controls
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

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    #endregion

    #region UI Updates
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
            scoreText.text = $"Score: {_score:N0}";
        }
    }

    public void UpdateHPUI(bool _immediate = false)
    {
        if (playerHealth == null) return;

        float currentHealth = playerHealth.GetCurrentHealth();
        float maxHealth = playerHealth.GetMaxHealth();

        if (hpText != null)
        {
            hpText.text = $"{currentHealth:F0}/{maxHealth:F0}";
        }

        if (hpSlider != null)
        {
            if (_immediate)
            {
                hpSlider.value = currentHealth;
            }
            else
            {
                StartCoroutine(SmoothSliderChange(hpSlider, hpSlider.value, currentHealth, 0.5f));
            }
        }
    }
    #endregion

    #region Visual Effects
    public void OnDamageTaken()
    {
        UpdateHPUI();
        BlinkHPSlider(damageBlinkColor);
        BlinkDamagePanel();
    }

    public void OnHeal()
    {
        UpdateHPUI();
        BlinkHPSlider(healBlinkColor);
    }

    public void BlinkHPSlider(Color _blinkColor)
    {
        blinkRequests++;
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(BlinkCoroutine(_blinkColor, blinkDuration, blinkCount));
        }
    }

    public void BlinkDamagePanel()
    {
        if (damagePanelCoroutine == null)
        {
            damagePanelCoroutine = StartCoroutine(DamagePanelCoroutine(damageBlinkColor, blinkDuration, blinkCount));
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator SmoothSliderChange(Slider _slider, float _fromValue, float _toValue, float _duration)
    {
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            _slider.value = Mathf.Lerp(_fromValue, _toValue, elapsed / _duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _slider.value = _toValue;
    }

    private IEnumerator BlinkCoroutine(Color _blinkColor, float _duration, int _blinkCount)
    {
        Color originalColor = fillImage.color;
        float halfDuration = _duration / (_blinkCount * 2);

        for (int i = 0; i < _blinkCount * blinkRequests; i++)
        {
            fillImage.color = _blinkColor;
            yield return new WaitForSeconds(halfDuration);
            fillImage.color = originalColor;
            yield return new WaitForSeconds(halfDuration);
        }

        blinkRequests = 0;
        blinkCoroutine = null;
    }

    private IEnumerator DamagePanelCoroutine(Color _blinkColor, float _duration, int _blinkCount)
    {
        if (damagePanel == null) yield break;

        float halfDuration = _duration / (_blinkCount * 2);
        damagePanel.SetActive(true);
        Image panelImage = damagePanel.GetComponent<Image>();
        if (panelImage == null) yield break;

        for (int i = 0; i < _blinkCount; i++)
        {
            panelImage.color = _blinkColor;
            yield return new WaitForSeconds(halfDuration);
            panelImage.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(halfDuration);
        }

        damagePanel.SetActive(false);
        damagePanelCoroutine = null;
    }
    #endregion

    public RectTransform GetFragmentTargetIcon()
    {
        return fragmentTargetIcon;
    }
}
