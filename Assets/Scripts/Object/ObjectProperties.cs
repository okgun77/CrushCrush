using UnityEngine;

public enum ObjectTypes
{
    BASIC,          // 기본형 오브젝트
    EXPLOSIVE,      // 폭발형 오브젝트
    INDESTRUCTIBLE  // 파괴 불가능한 오브젝트
}

public class ObjectProperties : MonoBehaviour
{
    [SerializeField] private ObjectTypes objectTypes;  // 오브젝트의 종류
    [SerializeField] private int health;             // 오브젝트의 체력
    [SerializeField] private int fragmentLevel = 0;  // 파편 레벨 (0: 원본 오브젝트)
    [SerializeField] private bool isBreakable;       // 파괴 가능한지 여부
    [SerializeField] private ScoreType scoreType = ScoreType.TypeA;    // 점수 타입 기본값 설정
    [SerializeField] private int defaultHealth = 100;  // 기본 체력 값 추가

    // Getter와 Setter 메서드들
    public ObjectTypes GetObjectType() => objectTypes;
    public bool IsBreakable() => isBreakable;
    public int GetHealth() => health;
    public void ReduceHealth(int _damage) => health -= _damage;
    public int GetFragmentLevel() => fragmentLevel;  // 파편 레벨 Getter
    public ScoreType GetScoreType() => scoreType;    // 점수 타입 Getter

    public void SetFragmentLevel(int _fragmentLevel)
    {
        fragmentLevel = _fragmentLevel;
    }

    public void SetScoreType(ScoreType _scoreType)
    {
        scoreType = _scoreType;
    }

    public void SetBreakable(bool _isBreakable)
    {
        isBreakable = _isBreakable;
    }

    public void ResetProperties()
    {
        health = defaultHealth;
        fragmentLevel = 0;
        isBreakable = true;
        scoreType = ScoreType.TypeA;  // 기본 점수 타입으로 TypeA 설정
    }

}
