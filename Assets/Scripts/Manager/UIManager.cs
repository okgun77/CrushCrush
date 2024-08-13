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
    [SerializeField] private GameObject damagePanel;

    [SerializeField] private Color damageBlinkColor = Color.red;
    [SerializeField] private Color healBlinkColor = Color.green;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private int blinkCount = 3;
    private GameManager gameManager;

    private Coroutine blinkCoroutine;
    private Coroutine damagePanelCoroutine;
    private int blinkRequests = 0; // 깜빡임 요청 수

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
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

    public void UpdateHPUI(int _currentHP, int _maxHP, bool _immediate = false)
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {_currentHP}";
        }
        if (hpSlider != null)
        {
            if (_immediate)
            {
                hpSlider.value = _currentHP;
            }
            else
            {
                StartCoroutine(SmoothSliderChange(hpSlider, hpSlider.value, _currentHP, 0.5f));
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

    public void BlinkHPSlider(Color _blinkColor)
    {
        blinkRequests++; // 새로운 깜빡임 요청 추가

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

    private IEnumerator BlinkCoroutine(Color _blinkColor, float _duration, int _blinkCount)
    {
        Color originalColor = fillImage.color;
        float halfDuration = _duration / (_blinkCount * 2);

        for (int i = 0; i < _blinkCount * blinkRequests; i++) // 누적된 요청 수만큼 깜빡임
        {
            fillImage.color = _blinkColor;
            yield return new WaitForSeconds(halfDuration);
            fillImage.color = originalColor;
            yield return new WaitForSeconds(halfDuration);
        }

        blinkRequests = 0; // 요청 수 초기화
        blinkCoroutine = null; // 코루틴 초기화
    }

    private IEnumerator DamagePanelCoroutine(Color _blinkColor, float _duration, int _blinkCount)
    {
        if (damagePanel == null) yield break;

        float halfDuration = _duration / (_blinkCount * 2);
        damagePanel.SetActive(true);
        Image panelImage = damagePanel.GetComponent<Image>();
        if (panelImage == null) yield break;

        for (int i = 0; i < _blinkCount; i++) // 지정된 횟수만큼 깜빡임
        {
            panelImage.color = _blinkColor;
            yield return new WaitForSeconds(halfDuration);
            panelImage.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(halfDuration);
        }

        damagePanel.SetActive(false);
        damagePanelCoroutine = null; // 코루틴 초기화
    }

    public void OnDamageTaken()
    {
        BlinkHPSlider(damageBlinkColor);
        BlinkDamagePanel();
    }

    public void OnHeal()
    {
        BlinkHPSlider(healBlinkColor);
    }
}
