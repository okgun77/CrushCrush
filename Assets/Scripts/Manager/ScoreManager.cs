using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private int score;
    private Dictionary<EScoreType, int> scoreTable;
    private int consecutiveDestroys = 0;
    private float lastDestroyTime;
    private float comboResetTime = 2.0f; // 콤보 리셋 시간

    // GameManager 참조 추가
    private GameManager gameManager;

    // 점수 변경 이벤트 추가
    public System.Action<int> OnScoreChanged;

    public void Init(GameManager _gameManager)  // Init 메서드 수정
    {
        gameManager = _gameManager;
        score = 0;
        consecutiveDestroys = 0;
        scoreText = GetComponent<TextMeshProUGUI>();
        UpdateScoreUI();

        // 스코어 타입별 기본 점수 설정
        scoreTable = new Dictionary<EScoreType, int>
        {
            { EScoreType.NORMAL, 100 },
            { EScoreType.BOSS_1, 500 },
            { EScoreType.BOSS_2, 1000 }
        };
    }

    private void Update()
    {
        // 콤보 시간 체크
        if (Time.time - lastDestroyTime > comboResetTime)
        {
            ResetConsecutiveDestroys();
        }
    }

    public void AddScore(int calculatedScore)
    {
        // 연속 파괴 보너스 계산 (10% 씩 증가)
        float comboMultiplier = 1 + (consecutiveDestroys * 0.1f);
        int finalScore = Mathf.RoundToInt(calculatedScore * comboMultiplier);
        
        score += finalScore;
        UpdateScoreUI();
    }

    public int CalculateScore(EScoreType scoreType, int fragmentLevel, float distanceToCamera)
    {
        int baseScore = GetScoreForScoreType(scoreType);
        
        // 프래그먼트 레벨에 따른 감소 (레벨이 높을수록 점수 감소)
        float fragmentMultiplier = Mathf.Pow(0.5f, fragmentLevel);
        
        // 거리에 따른 보너스 수정 (가까울수록 높은 점수)
        float normalizedDistance = Mathf.Clamp01(distanceToCamera / 20f);
        float distanceMultiplier = Mathf.Lerp(2f, 1f, normalizedDistance);
        
        return Mathf.RoundToInt(baseScore * fragmentMultiplier * distanceMultiplier);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score:N0}";
        }
        // 이벤트 발생
        OnScoreChanged?.Invoke(score);
    }

    // 추가된 메서드들
    public void IncreaseConsecutiveDestroys()
    {
        consecutiveDestroys++;
        lastDestroyTime = Time.time;
    }

    private void ResetConsecutiveDestroys()
    {
        consecutiveDestroys = 0;
    }

    private int GetScoreForScoreType(EScoreType scoreType)
    {
        if (scoreTable.TryGetValue(scoreType, out int baseScore))
        {
            return baseScore;
        }
        return 100; // 기본 점수
    }

    // 게터 메서드들
    public int GetScore() => score;
    public int GetConsecutiveDestroys() => consecutiveDestroys;
    public float GetComboTimeRemaining() => Mathf.Max(0, comboResetTime - (Time.time - lastDestroyTime));
}
