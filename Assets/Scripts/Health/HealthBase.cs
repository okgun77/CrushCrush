using System;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected int      maxHealth =100;         // 최대 체력
    [SerializeField] protected float    mass = 1.0f;            // 질량(무게 같은...)
    [SerializeField] protected bool     isDestructible = true;  // 파괴가능 여부

    protected int currentHealth;

    // 프로퍼티
    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    public int CurrentHealth => currentHealth;
    public float Mass => mass;
    public bool IsDestructible
    {
        get => isDestructible;
        set => isDestructible = value;
    }


    // 이벤트
    public event Action<int, int> OnHealthChanged;  // 현재 체력, 최대 체력
    public event Action OnDeath;                    // 사망 이벤트


    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        // rigidbody가 있을 경우 질량 설정
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = mass;
        }
    }

    public virtual void TakeDamage(int _amount)
    {
        if (!isDestructible) return;

        currentHealth = Mathf.Max(currentHealth - _amount, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int _amount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + _amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }


    protected virtual void Die()
    {
        OnDeath?.Invoke();
        // 기본적으로는 아무동작 안함. 자식 클래스에서 구현
    }


}
