using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ProgressionMilestone
{
    public float scoreThreshold;
    public float timeThreshold;
    public int destroyCountThreshold;
    public UnityEngine.Events.UnityEvent onMilestoneReached;
}

public class ProgressionManager : MonoBehaviour
{
    [Header("Progression Settings")]
    [SerializeField] private List<ProgressionMilestone> milestones = new List<ProgressionMilestone>();
    
    [Header("Score Settings")]
    [SerializeField] private float baseScorePerDestroy = 100f;
    [SerializeField] private float comboMultiplier = 1.1f;
    [SerializeField] private float comboTimeWindow = 1f;
    
    private GameManager gameManager;
    private DifficultyManager difficultyManager;
    private float currentScore;
    private int destroyCount;
    private float gameTime;
    private int currentCombo;
    private float lastDestroyTime;
    
    public event Action<float> OnScoreChanged;
    public event Action<int> OnDestroyCountChanged;
    public event Action<int> OnComboChanged;
    
    public void Init(GameManager _gameManager, DifficultyManager _difficultyManager)
    {
        gameManager = _gameManager;
        difficultyManager = _difficultyManager;
        ResetProgression();
    }
    
    private void Update()
    {
        if (gameManager == null) return;
        
        gameTime += Time.deltaTime;
        CheckMilestones();
        
        // Reset combo if time window passed
        if (Time.time - lastDestroyTime > comboTimeWindow)
        {
            ResetCombo();
        }
    }
    
    public void OnObjectDestroyed(Vector3 position)
    {
        destroyCount++;
        UpdateCombo();
        AddScore(CalculateScore(position));
        lastDestroyTime = Time.time;
        
        OnDestroyCountChanged?.Invoke(destroyCount);
    }
    
    private float CalculateScore(Vector3 position)
    {
        float difficultyMultiplier = difficultyManager.GetCurrentDifficulty();
        float comboBonus = Mathf.Pow(comboMultiplier, currentCombo);
        
        // Additional position-based bonus (optional)
        float distanceFromCenter = Vector3.Distance(Vector3.zero, position);
        float positionMultiplier = 1f + (distanceFromCenter * 0.1f);
        
        return baseScorePerDestroy * difficultyMultiplier * comboBonus * positionMultiplier;
    }
    
    private void AddScore(float amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }
    
    private void UpdateCombo()
    {
        currentCombo++;
        OnComboChanged?.Invoke(currentCombo);
    }
    
    private void ResetCombo()
    {
        if (currentCombo > 0)
        {
            currentCombo = 0;
            OnComboChanged?.Invoke(currentCombo);
        }
    }
    
    private void CheckMilestones()
    {
        foreach (var milestone in milestones)
        {
            if (currentScore >= milestone.scoreThreshold &&
                gameTime >= milestone.timeThreshold &&
                destroyCount >= milestone.destroyCountThreshold)
            {
                milestone.onMilestoneReached?.Invoke();
                milestones.Remove(milestone);
                break;
            }
        }
    }
    
    public void ResetProgression()
    {
        currentScore = 0;
        destroyCount = 0;
        gameTime = 0;
        currentCombo = 0;
        lastDestroyTime = 0;
        
        OnScoreChanged?.Invoke(currentScore);
        OnDestroyCountChanged?.Invoke(destroyCount);
        OnComboChanged?.Invoke(currentCombo);
    }
    
    public float GetCurrentScore() => currentScore;
    public int GetDestroyCount() => destroyCount;
    public int GetCurrentCombo() => currentCombo;
    public float GetGameTime() => gameTime;
}
