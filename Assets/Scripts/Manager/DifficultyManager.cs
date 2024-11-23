using UnityEngine;
using System;

public class DifficultyManager : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnSettings currentSpawnSettings;
    private float stageProgress;
    
    public event Action<float> OnDifficultyChanged;
    
    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
        ResetDifficulty();
    }
    
    public void SetStageSettings(SpawnSettings settings)
    {
        currentSpawnSettings = settings;
        ResetDifficulty();
    }
    
    private void Update()
    {
        if (gameManager == null || currentSpawnSettings == null) return;
        
        // Update difficulty based on stage progress
        OnDifficultyChanged?.Invoke(stageProgress);
    }
    
    public void SetProgress(float progress)
    {
        stageProgress = Mathf.Clamp01(progress);
        OnDifficultyChanged?.Invoke(stageProgress);
    }
    
    public void ResetDifficulty()
    {
        stageProgress = 0f;
    }
    
    public float GetCurrentSpawnInterval()
    {
        if (currentSpawnSettings == null) return 2f;
        return currentSpawnSettings.GetSpawnInterval(stageProgress);
    }
    
    public float GetSpeedMultiplier()
    {
        if (currentSpawnSettings == null) return 1f;
        return currentSpawnSettings.GetSpeedMultiplier(stageProgress);
    }
    
    public int GetSpawnCount()
    {
        if (currentSpawnSettings == null) return 1;
        return currentSpawnSettings.GetSpawnCount(stageProgress);
    }
    
    public float GetCurrentDifficulty()
    {
        return stageProgress;
    }
}
