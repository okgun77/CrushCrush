using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ScoreType = GameEnums.ScoreType;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int maxFragmentLevel = 3;

    private ScoreType scoreType;
    private int score;
    private Dictionary<ScoreType, int> scoreTable;
    private GameManager gameManager;
    private int consecutiveDestroys = 0;


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

    public int CalculateScore(ScoreType _scoreType, int _fragmentLevel, float _distanceToCamera)
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

    // 거리 기반 가중치 계산 함수
    private float CalculateDistanceMultiplier(float _distanceToCamera)
    {
        // 거리가 가까울수록 가중치가 높아짐
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
