using System;
using UnityEngine;

public class ObjectProperties : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int health;
    private int? customHealth = null;

    [Header("Fragment Settings")]
    [SerializeField] private int fragmentLevel = 0;
    [SerializeField] private bool isBreakable = true;

    [Header("Score Settings")]
    private EScoreType scoreType = EScoreType.NORMAL;

    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EObjectType enemyType = EObjectType.BASIC;
    private EObjectType objectType = EObjectType.BASIC;

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;

    private void Awake()
    {
        InitializeHealth();
    }

    private void OnEnable()
    {
        InitializeHealth();
    }

    private void InitializeHealth()
    {
        if (customHealth.HasValue)
        {
            health = customHealth.Value;
            Debug.Log($"[ObjectProperties] Using custom health: {health}");
            return;
        }

        if (enemyData == null)
        {
            health = 100;
            Debug.Log($"[ObjectProperties] Using default health: {health}");
            return;
        }

        health = enemyData.GetBaseHealth(enemyType);
        Debug.Log($"[ObjectProperties] Using enemyData health for type {enemyType}: {health}");
    }

    // Getter 메서드들
    public EObjectType ObjectType => objectType;
    public bool IsBreakable() => isBreakable;
    public int GetHealth() => health;
    public int GetFragmentLevel() => fragmentLevel;
    public EScoreType GetScoreType() => scoreType;
    public int GetDefaultHealth() => health;
    public float GetAttackDamage() => attackDamage;

    // Setter 메서드들
    public void SetObjectType(EObjectType type) => objectType = type;
    public void SetFragmentLevel(int level) => fragmentLevel = level;
    public void SetScoreType(EScoreType type) => scoreType = type;
    public void SetBreakable(bool breakable) => isBreakable = breakable;
    public void SetAttackDamage(float damage) => attackDamage = damage;
    public void SetHealth(int value)
    {
        customHealth = value;
        health = value;
        Debug.Log($"[ObjectProperties] Health set to: {health}");
    }

    // 속성 초기화
    public void ResetProperties()
    {
        customHealth = null;
        InitializeHealth();
        
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
        customHealth = other.customHealth;
        health = other.health;
        fragmentLevel = other.fragmentLevel;
        isBreakable = other.isBreakable;
        scoreType = other.scoreType;
        attackDamage = other.attackDamage;
    }

    // IDamageable 인터페이스 구현
    public void TakeDamage(float _damage)
    {
        health = Mathf.Max(0, health - Mathf.RoundToInt(_damage));
        
        if (health <= 0)
        {
            var breakObject = gameObject.AddComponent<BreakObject>();
            if (breakObject != null)
            {
                breakObject.HandleObjectDestruction();
            }
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}
