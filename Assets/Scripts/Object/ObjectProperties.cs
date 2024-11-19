using UnityEngine;

public enum ObjectTypes
{
    BASIC,          // 기본형 오브젝트
    EXPLOSIVE,      // 폭발형 오브젝트
    INDESTRUCTIBLE  // 파괴 불가능한 오브젝트
}

public class ObjectProperties : MonoBehaviour
{
    [Header("Object Type Settings")]
    [SerializeField] private ObjectTypes objectType;  // objectTypes -> objectType으로 변경

    [Header("Health Settings")]
    [SerializeField] private int defaultHealth = 100;  // 기본 체력 값
    [SerializeField] private int health;              // 현재 체력

    [Header("Fragment Settings")]
    [SerializeField] private int fragmentLevel = 0;   // 파편 레벨 (0: 원본 오브젝트)
    [SerializeField] private bool isBreakable = true; // 기본값 true로 설정

    [Header("Score Settings")]
    [SerializeField] private ScoreType scoreType = ScoreType.TypeA;

    private void Awake()
    {
        // 초기 체력 설정
        health = defaultHealth;
    }

    // Getter 메서드들
    public ObjectTypes GetObjectType() => objectType;
    public bool IsBreakable() => isBreakable;
    public int GetHealth() => health;
    public int GetFragmentLevel() => fragmentLevel;
    public ScoreType GetScoreType() => scoreType;
    public int GetDefaultHealth() => defaultHealth;

    // Setter 메서드들
    public void SetObjectType(ObjectTypes type) => objectType = type;
    public void SetFragmentLevel(int level) => fragmentLevel = level;
    public void SetScoreType(ScoreType type) => scoreType = type;
    public void SetBreakable(bool breakable) => isBreakable = breakable;
    
    // 체력 관련 메서드들
    public void ReduceHealth(int damage)
    {
        health = Mathf.Max(0, health - damage); // 음수 체력 방지
    }

    public void ResetHealth()
    {
        health = defaultHealth;
    }

    // 모든 속성 초기화
    public void ResetProperties()
    {
        health = defaultHealth;
        fragmentLevel = 0;
        isBreakable = true;
        scoreType = ScoreType.TypeA;
    }

    // 속성 복사 메서드 추가 (프리팹 생성 시 유용)
    public void CopyPropertiesFrom(ObjectProperties other)
    {
        if (other == null) return;
        
        objectType = other.objectType;
        defaultHealth = other.defaultHealth;
        health = other.health;
        fragmentLevel = other.fragmentLevel;
        isBreakable = other.isBreakable;
        scoreType = other.scoreType;
    }
}
