using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SlowMotionManager slowMotionManager;
    [SerializeField] private HPManager hpManager; // HPManager 추가
    [SerializeField] private TouchManager touchManager; // TouchManager 추가
    // 다른 매니저 클래스들도 여기에 추가

    private void Awake()
    {
        // VSync 끄기
        QualitySettings.vSyncCount = 0;

        // 타겟 프레임레이트 설정
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        // 하위 매니저 초기화
        uiManager.Init();
        spawnManager.Init();
        scoreManager.Init();
        slowMotionManager.Init(OnSlowMotionChanged);
        hpManager.Init();
        touchManager = FindObjectOfType<TouchManager>(); // TouchManager 초기화
    }

    public void RestartGame()
    {
        // 현재 씬의 이름을 가져와서 씬을 다시 로드합니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ExitGame()
    {
        // 게임 종료
        Application.Quit();
        Debug.Log("Game is exiting");   // 확인
    }

    public void GameOver()
    {
        // 게임 오버 처리
        Debug.Log("Game Over!");
        // 게임 오버 UI를 표시하거나, 씬을 다시 로드할 수 있습니다.
        uiManager.ShowGameOverUI();
    }

    public void UpdateHP(int _hp)
    {
        uiManager.UpdateHPUI(_hp, hpManager.GetCurrentHP()); // UpdateHPUI 호출로 변경
    }

    public void UpdateScore(int _score)
    {
        uiManager.UpdateScoreText(_score);
    }

    public void UpdateTimeScale(float _timeScale)
    {
        uiManager.UpdateTimeScaleText(_timeScale);
    }

    public int GetScore(ScoreType _scoreType)
    {
        return scoreManager.GetScoreForScoreType(_scoreType);
    }

    public void AddScore(ScoreType _scoreType)
    {
        int scoreToAdd = scoreManager.GetScoreForScoreType(_scoreType);
        scoreManager.AddScore(scoreToAdd);
    }

    private void OnSlowMotionChanged(bool _isActive)
    {
        if (_isActive)
        {
            uiManager.ShowSlowMotionPanel();
        }
        else
        {
            uiManager.HideSlowMotionPanel();
        }
    }

    public void TakeDamage(int _damage)
    {
        hpManager.TakeDamage(_damage);
    }
}