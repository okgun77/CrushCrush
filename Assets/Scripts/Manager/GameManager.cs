using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SlowMotionManager slowMotionManager;
    [SerializeField] private HPManager hpManager;
    [SerializeField] private TouchManager touchManager;
    [SerializeField] private MoveManager moveManager;
    [SerializeField] private AudioManager audioManager;

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
        hpManager.Init(this);
        // touchManager.Init(this);
        moveManager.Init(this);
        audioManager.Init(this);
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

    public void TakeDamage(int _damage)
    {
        hpManager.TakeDamage(_damage);
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
        hpManager.Heal(_amount);
    }
}
