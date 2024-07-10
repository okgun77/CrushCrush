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

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public int GetScoreForScoreType(ScoreType scoreType)
    {
        if (scoreTable.TryGetValue(scoreType, out int value))
        {
            return value;
        }
        return 0;
    }
}
