using UnityEngine;

public class HealthSystem
{
    protected float currentHealth;
    protected float maxHealth;

    public HealthSystem()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - _damage);
        Debug.Log($"HealthSystem - Taking Damage: {_damage}, Current Health: {currentHealth}");
    }

    public void Heal(float _amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + _amount);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public float GetCurrentHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual void SetMaxHealth(float _maxHealth)
    {
        maxHealth = _maxHealth;
        currentHealth = maxHealth;
    }
}
