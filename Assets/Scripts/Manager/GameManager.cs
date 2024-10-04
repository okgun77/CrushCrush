using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SlowMotionManager slowMotionManager;
    [SerializeField] private TouchManager touchManager;
    [SerializeField] private MoveManager moveManager;
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private int consecutiveDestroyThreshold = 5;   // 연속 파괴 제한
    [SerializeField] private int hpIncreaseAmount = 10;             // HP 증가량

    // [SerializeField] private HPManager hpManager;

    private PlayerHealth playerHealth;
    private int consecutiveDestroys = 0;


    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        uiManager.Init(this);
        spawnManager.Init(this);
        scoreManager.Init(this);
        slowMotionManager.Init(this);
        moveManager.Init(this);
        audioManager.Init(this);
        // hpManager.Init(this);
        // touchManager.Init(this);

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth를 찾을 수 없습니다.");
        }

    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        uiManager.ShowGameOverUI();
    }


    public void AddScore(int _score)
    {
        scoreManager.AddScore(_score);
    }

    public void UpdateTimeScale(float _timeScale)
    {
        uiManager.UpdateTimeScaleText(_timeScale);
    }

    public MoveManager GetMoveManager()
    {
        return moveManager;
    }

    public void HealPlayer(int _amount)
    {
        playerHealth?.Heal(_amount);
    }

    public void IncreaseConsecutiveDestroys()
    {
        consecutiveDestroys++;
        if (consecutiveDestroys >= consecutiveDestroyThreshold)
        {
            HealPlayer(hpIncreaseAmount);
            ResetConsecutiveDestroys();
        }
    }

    public void ResetConsecutiveDestroys()
    {
        consecutiveDestroys = 0;
    }
}
