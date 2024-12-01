using System;
using UnityEngine;

public class ObjectProperties : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int defaultHealth = 100;
    [SerializeField] private int health;

    [Header("Fragment Settings")]
    [SerializeField] private int fragmentLevel = 0;
    [SerializeField] private bool isBreakable = true;

    [Header("Score Settings")]
    private EScoreType scoreType = EScoreType.NORMAL;

    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EObjectType enemyType;
    private EObjectType objectType = EObjectType.BASIC;

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;

    private void Awake()
    {
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
    public void SetHealth(int value) => health = value;

    // 속성 초기화
    public void ResetProperties()
    {
        if (enemyData != null)
        {
            defaultHealth = enemyData.GetBaseHealth(enemyType);
            attackDamage = enemyData.GetAttackDamage(enemyType);
        }
        
        health = defaultHealth;
        fragmentLevel = 0;
        isBreakable = true;
        scoreType = EScoreType.NORMAL;
        objectType = EObjectType.BASIC;
    }

    // 속성 복사
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

    // IDamageable 인터페이스 구현
    public void TakeDamage(float _damage)
    {
        Debug.Log($"Before damage - Health: {health}, Damage received: {_damage}");
        health = Mathf.Max(0, health - Mathf.RoundToInt(_damage));
        Debug.Log($"After damage - Health: {health}");
        
        if (health <= 0)
        {
            Debug.Log("Health reached 0, attempting to break object");
            var breakObject = gameObject.AddComponent<BreakObject>();
            if (breakObject != null)
            {
                Debug.Log("BreakObject created, calling HandleObjectDestruction");
                breakObject.HandleObjectDestruction();
            }
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}
