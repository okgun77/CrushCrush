using UnityEngine;
using System.Linq;

public class StageManager : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private StageData stageData;
    [SerializeField] private float stageStartDelay = 2f;
    [SerializeField] private ObjectMovementManager movementManager;

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = true;

    private GameManager gameManager;
    private SpawnManager spawnManager;
    private int currentStageIndex = -1;
    private StageState currentState = StageState.Ready;
    private StageData.Stage currentStage => stageData.stages[currentStageIndex];
    
    private float timeElapsed;
    private int destroyCount;
    private int currentScore;
    private float stageProgress = 0f;
    private float stageDuration = 60f;

    #region Initialization
    public void Init(GameManager _gameManager, SpawnManager _spawnManager)
    {
        gameManager = _gameManager;
        spawnManager = _spawnManager;

        if (stageData == null)
        {
            LogError("StageData is not assigned!");
            return;
        }
        
        if (stageData.stages == null || stageData.stages.Length == 0)
        {
            LogError("No stages defined in StageData!");
            return;
        }

        if (movementManager == null)
        {
            movementManager = FindFirstObjectByType<ObjectMovementManager>();
        }

        if (movementManager == null)
        {
            LogError("MovementManager is not found in the scene!");
            return;
        }

        Log($"Initialized with {stageData.stages.Length} stages");
        ResetStageProgress();
        StartNextStage();
    }

    private void ResetStageProgress()
    {
        timeElapsed = 0f;
        destroyCount = 0;
        currentScore = 0;
    }
    #endregion

    #region Stage Control
    private void StartNextStage()
    {
        currentStageIndex++;
        if (currentStageIndex >= stageData.stages.Length)
        {
            GameComplete();
            return;
        }

        StartStage(currentStageIndex);
    }

    private void StartStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= stageData.stages.Length)
        {
            LogError($"Invalid stage index: {stageIndex}");
            return;
        }

        var stage = stageData.stages[stageIndex];
        Log($"Starting Stage {stageIndex + 1}: {stage.stageName}");
        Log($"Spawn Interval: {stage.spawnSettings.spawnInterval}");
        Log($"Movement Patterns: {string.Join(", ", stage.spawnSettings.availablePatterns)}");

        spawnManager.StopSpawning();            // 이전 스폰 완전히 중지
        
        currentState = StageState.InProgress;
        ResetStageProgress();
        stageProgress = 0f;
        stageDuration = stage.duration;
        movementManager.ResetDifficulty();

        UpdateSpawnSettings(stage.spawnSettings);
        spawnManager.StartSpawning();           // 새로운 설정으로 스폰 시작

        gameManager.OnStageStart(stageIndex);
    }

    private void UpdateSpawnSettings(SpawnSettings settings)
    {
        if (spawnManager != null)
        {
            spawnManager.UpdateSpawnSettings(
                settings.spawnInterval,
                settings.availablePatterns,
                new MovementData
                {
                    speed = settings.movementSpeed,
                    amplitude = settings.patternAmplitude,
                    frequency = settings.patternFrequency,
                    duration = settings.spawnInterval * 2
                }
            );
            
            spawnManager.StartSpawning();
        }
    }
    #endregion

    #region Condition Checking
    private void Update()
    {
        if (currentState != StageState.InProgress) return;

        timeElapsed += Time.deltaTime;
        stageProgress = Mathf.Clamp01(timeElapsed / stageDuration);
        
        // 진행도에 따라 난이도 업데이트
        UpdateStageDifficulty(stageProgress);
        
        CheckStageConditions();
    }

    private void CheckStageConditions()
    {
        if (currentStageIndex < 0 || currentStageIndex >= stageData.stages.Length) return;

        var currentStage = stageData.stages[currentStageIndex];
        bool allConditionsMet = true;

        foreach (var condition in currentStage.conditions)
        {
            float currentValue = GetCurrentValueForCondition(condition.type);
            bool isMet = condition.CheckCondition(currentValue);
            
            if (!isMet)
            {
                allConditionsMet = false;
                break;
            }
        }

        if (allConditionsMet)
        {
            CompleteCurrentStage();
        }
    }

    private float GetCurrentValueForCondition(ConditionType type)
    {
        return type switch
        {
            ConditionType.Time => timeElapsed,
            ConditionType.DestroyCount => destroyCount,
            ConditionType.Score => currentScore,
            _ => 0
        };
    }
    #endregion

    #region Stage Completion
    private void CompleteCurrentStage()
    {
        currentState = StageState.Complete;
        spawnManager.StopSpawning();                        // 현재 스테이지의 스폰 중지
        Log($"Stage {currentStageIndex + 1} Complete!");
        
        // 스테이지 완료 이벤트 발생
        gameManager.OnStageComplete(currentStageIndex);
        
        // 다음 스테이지 시작
        Invoke(nameof(StartNextStage), stageStartDelay);
    }

    private void GameComplete()
    {
        Log("All Stages Complete! Game Clear!");
        gameManager.OnGameComplete();
    }
    #endregion

    #region Public Methods
    public void AddDestroyCount()
    {
        destroyCount++;
        Log($"Object Destroyed. Count: {destroyCount}");
    }

    public void UpdateScore(int score)
    {
        currentScore = score;
    }

    public bool IsLastStage()
    {
        return currentStageIndex == stageData.stages.Length - 1;
    }

    public int GetCurrentStage()
    {
        return currentStageIndex + 1;
    }
    #endregion

    #region Debug
    private void Log(string message)
    {
        if (showDebugLog)
            Debug.Log($"[StageManager] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[StageManager] {message}");
    }
    #endregion

    private void UpdateStageDifficulty(float progress)
    {
        // 움직임 난이도 조절
        movementManager.UpdateDifficulty(progress);

        // 스폰 간격 조절
        if (spawnManager != null)
        {
            float initialInterval = currentStage.spawnSettings.spawnInterval;
            float minInterval = currentStage.spawnSettings.minimumSpawnInterval;
            float currentInterval = Mathf.Lerp(initialInterval, minInterval, progress);
            spawnManager.SetSpawnInterval(currentInterval);
        }
    }
}
