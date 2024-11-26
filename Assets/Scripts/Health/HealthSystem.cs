using UnityEngine;
using System;

public abstract class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    public virtual void TakeDamage(int damage)
    {
        if (IsDead()) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        NotifyHealthChanged();

        if (IsDead())
        {
            NotifyDeath();
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        if (IsDead()) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        NotifyHealthChanged();
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => currentHealth <= 0;

    protected virtual void Die()
    {
        // 자식 클래스에서 구현
    }

    protected void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected void NotifyDeath()
    {
        OnDeath?.Invoke();
    }
}
