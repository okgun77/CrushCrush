using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - _damage);
    }

    public void Heal(float _amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + _amount);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    public void SetMaxHealth(float _maxHealth)
    {
        maxHealth = _maxHealth;
        currentHealth = maxHealth;
    }

}
