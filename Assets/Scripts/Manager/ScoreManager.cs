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
    [SerializeField] private int maxFragmentLevel = 3; // 최대 파편화 단계
    private int score;
    private Dictionary<ScoreType, int> scoreTable;

    public void Init()
    {
        score = 0;
        UpdateScoreUI();

        // ScoreType별 점수 설정
        scoreTable = new Dictionary<ScoreType, int>
        {
            { ScoreType.TypeA, 10 },
            { ScoreType.TypeB, 20 },
            { ScoreType.TypeC, 30 }
        };
    }

    public void AddScore(int calculatedScore)
    {
        score += calculatedScore;
        UpdateScoreUI();
    }

    public int GetScoreForScoreType(ScoreType scoreType)
    {
        if (scoreTable.TryGetValue(scoreType, out int value))
        {
            return value;
        }
        return 0;
    }

    public int CalculateScore(ScoreType scoreType, int fragmentLevel)
    {
        int baseScore = GetScoreForScoreType(scoreType);
        float multiplier = Mathf.Pow(0.5f, fragmentLevel);
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
            scoreText.text = "Score: " + score;
        }
    }
}
