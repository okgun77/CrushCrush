using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private int maxFragmentLevel = 3;
    private int score;
    private Dictionary<EScoreType, int> scoreTable;
    private GameManager gameManager;
    private int consecutiveDestroys = 0;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
        score = 0;
        scoreText = GetComponent<TextMeshProUGUI>();
        UpdateScoreUI();

        scoreTable = new Dictionary<EScoreType, int>
        {
            { EScoreType.TYPE_A, 10 },
            { EScoreType.TYPE_B, 20 },
            { EScoreType.TYPE_C, 30 }
        };
    }

    public void AddScore(int _calculatedScore)
    {
        score += _calculatedScore;
        UpdateScoreUI();
    }

    public int GetScoreForScoreType(EScoreType _scoreType)
    {
        if (scoreTable.TryGetValue(_scoreType, out int value))
        {
            return value;
        }
        return 0;
    }

    public int CalculateScore(EScoreType _scoreType, int _fragmentLevel, float _distanceToCamera)
    {
        int baseScore = GetScoreForScoreType(_scoreType);
        float fragmentMultiplier = Mathf.Pow(0.5f, _fragmentLevel);
        float distanceMultiplier = CalculateDistanceMultiplier(_distanceToCamera);

        return Mathf.CeilToInt(baseScore * fragmentMultiplier * distanceMultiplier);
    }

    public int GetMaxFragmentLevel()
    {
        return maxFragmentLevel;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }

    private float CalculateDistanceMultiplier(float _distanceToCamera)
    {
        return Mathf.Clamp01(1 / _distanceToCamera);
    }

    public void IncreaseConsecutiveDestroys()
    {
        consecutiveDestroys++;
        // 연속 파괴에 따른 추가 점수나 보너스 효과를 여기에 구현
    }

    public void ResetConsecutiveDestroys()
    {
        consecutiveDestroys = 0;
    }

    public int GetConsecutiveDestroys()
    {
        return consecutiveDestroys;
    }
}
