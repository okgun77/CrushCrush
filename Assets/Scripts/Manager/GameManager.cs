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
    // [SerializeField] private MoveManager moveManager;
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private DifficultyManager difficultyManager;  // 추가
    [SerializeField] private ProgressionManager progressionManager;  // 추가

    private ObjectPoolManager poolManager;
    private Transform playerTransform;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        poolManager = ObjectPoolManager.Instance;
        Debug.Log("ObjectPoolManager created");

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure the player object has the 'Player' tag.");
            return;
        }

        if (spawnManager != null)
        {
            var items = spawnManager.GetSpawnableItems();
            Debug.Log($"Found {items.Count} items to pool");
            foreach (var item in items)
            {
                if (item.prefab != null)
                {
                    poolManager.CreatePool(item.prefab.name, item.prefab, 10);
                    Debug.Log($"Created pool for {item.prefab.name}");
                }
                else
                {
                    Debug.LogWarning("Null prefab found in spawnable items!");
                }
            }
            Debug.Log("Object pools initialized");
        }
        else
        {
            Debug.LogError("SpawnManager is not assigned!");
        }
    }

    private void Start()
    {
        Debug.Log("GameManager Start called");

        if (movementManager == null)
        {
            Debug.LogError("MovementManager is not assigned!");
            return;
        }

        // AudioManager 찾기
        if (audioManager == null)
        {
            audioManager = FindAnyObjectByType<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogWarning("AudioManager not found in the scene.");
            }
        }

        // 1. 기본 매니저들 초기화
        movementManager.Init(this);  // MovementManager를 먼저 초기화
        
        // 2. SpawnManager 초기화 (MovementManager 초기화 후)
        if (spawnManager != null && playerTransform != null)
        {
            spawnManager.Init(this, poolManager, movementManager, playerTransform);
        }
        else
        {
            Debug.LogError("SpawnManager or PlayerTransform is not assigned!");
            return;
        }

        // 3. 나머지 매니저들 초기화
        uiManager?.Init(this);
        scoreManager?.Init(this);
        slowMotionManager?.Init(this);
        hpManager?.Init(this);
        audioManager?.Init(this);
        difficultyManager?.Init(this);
        progressionManager?.Init(this, difficultyManager);

        // 4. StageManager는 마지막에 초기화 (모든 의존성이 준비된 후)
        stageManager?.Init(this, spawnManager);
    }

    public void RestartGame()
    {
        spawnManager?.PauseSpawning();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ExitGame()
    {
        spawnManager?.PauseSpawning();
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void GameOver()
    {
        spawnManager?.PauseSpawning();
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
        if (spawnManager != null)
        {
            spawnManager.SetSpawnInterval(spawnManager.GetCurrentSpawnInterval() * _timeScale);
        }
    }

    // public MoveManager GetMoveManager()
    // {
    //     return moveManager;
    // }

    public void HealPlayer(int _amount)
    {
        hpManager.Heal(_amount);
    }

    public DifficultyManager DifficultyManager => difficultyManager;

    private void OnDestroy()
    {
        if (spawnManager != null)
        {
            spawnManager.PauseSpawning();
        }
    }

    public void OnStageStart(int stageIndex)
    {
        Debug.Log($"Stage {stageIndex + 1} Started!");
        // 스테이지 시작 시 필요한 처리
    }

    public void OnStageComplete(int stageIndex)
    {
        Debug.Log($"Stage {stageIndex + 1} Completed!");
        // 스테이지 완료 시 필요한 처리
    }

    public void OnGameComplete()
    {
        Debug.Log("Game Completed!");
        // 게임 클리어 시 필요한 처리
    }
}
