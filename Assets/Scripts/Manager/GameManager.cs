using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SlowMotionManager slowMotionManager;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        uiManager.Init();
        spawnManager.Init();
        scoreManager.Init();
        slowMotionManager.Init(OnSlowMotionChanged);
        
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

    public void UpdateHP(int hp)
    {
        uiManager.UpdateHPText(hp);
    }

    public void UpdateScore(int score)
    {
        uiManager.UpdateScoreText(score);
    }

    public void UpdateTimeScale(float timeScale)
    {
        uiManager.UpdateTimeScaleText(timeScale);
    }

    public int GetScore(ScoreType scoreType)
    {
        return scoreManager.GetScoreForScoreType(scoreType);
    }

    public void AddScore(ScoreType scoreType)
    {
        scoreManager.AddScore(scoreType);
    }

    private void OnSlowMotionChanged(bool isActive)
    {
        if (isActive)
        {
            uiManager.ShowSlowMotionPanel();
        }
        else
        {
            uiManager.HideSlowMotionPanel();
        }
    }
}
