using System;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    

    [Header("Health Settings")]
    [SerializeField] private int defaultHealth = 100;
    [SerializeField] private int health;

    [Header("Fragment Settings")]
    [SerializeField] private int fragmentLevel = 0;
    [SerializeField] private bool isBreakable = true;

    [Header("Score Settings")]

    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;  // ScriptableObject 참조
    [SerializeField] private EObjectType enemyType;
    
    private EScoreType scoreType = EScoreType.TYPE_A;

    private EObjectType objectType = EObjectType.BASIC;  // 기본값 설정

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;

    private void Awake()
    {
        // 초기 체력 설정
        health = defaultHealth;


        if (enemyData != null)
        {
            defaultHealth = enemyData.GetBaseHealth(enemyType);
            attackDamage = enemyData.GetAttackDamage(enemyType);
        }
        health = defaultHealth;
    }

    // Getter 메서드들
    public EObjectType ObjectType => objectType;
    public bool IsBreakable() => isBreakable;
    public int GetHealth() => health;
    public int GetFragmentLevel() => fragmentLevel;
    public EScoreType GetScoreType() => scoreType;
    public int GetDefaultHealth() => defaultHealth;
    public float GetAttackDamage() => attackDamage;

    // Setter 메서드들
    public void SetObjectType(EObjectType type) => objectType = type;
    public void SetFragmentLevel(int level) => fragmentLevel = level;
    public void SetScoreType(EScoreType type) => scoreType = type;
    public void SetBreakable(bool breakable) => isBreakable = breakable;
    public void SetAttackDamage(float damage) => attackDamage = damage;
    
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
        scoreType = EScoreType.TYPE_A;
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
        attackDamage = other.attackDamage;
    }
}
