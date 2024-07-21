using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ScoreType
{
    TypeA,
    TypeB,
    TypeC
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int maxFragmentLevel = 3;
    private int score;
    private Dictionary<ScoreType, int> scoreTable;
    private GameManager gameManager;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
        score = 0;
        UpdateScoreUI();

        scoreTable = new Dictionary<ScoreType, int>
        {
            { ScoreType.TypeA, 10 },
            { ScoreType.TypeB, 20 },
            { ScoreType.TypeC, 30 }
        };
    }

    public void AddScore(int _calculatedScore)
    {
        score += _calculatedScore;
        UpdateScoreUI();
    }

    public int GetScoreForScoreType(ScoreType _scoreType)
    {
        if (scoreTable.TryGetValue(_scoreType, out int value))
        {
            return value;
        }
        return 0;
    }

    public int CalculateScore(ScoreType _scoreType, int _fragmentLevel)
    {
        int baseScore = GetScoreForScoreType(_scoreType);
        float multiplier = Mathf.Pow(0.5f, _fragmentLevel);
        return Mathf.CeilToInt(baseScore * multiplier);
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
}
