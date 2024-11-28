using UnityEngine;

public class PlayerHealth : HealthSystem, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private UIManager uiManager;

    private void Start()
    {
        SetMaxHealth(maxHealth);
        
        // UIManager 찾기
        uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not found!");
        }
    }

    public override void TakeDamage(float _damage)
    {
        base.TakeDamage(_damage);
        Debug.Log($"Player took damage: {_damage}, Current Health: {GetHealthPercent() * 100}%");

        // UI 업데이트
        if (uiManager != null)
        {
            uiManager.OnDamageTaken();
        }

        if (IsDead())
        {
            OnPlayerDeath();
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Player Dead!!");
        
        // UI에 게임오버 표시
        if (uiManager != null)
        {
            uiManager.ShowGameOverUI();
        }
    }

    public void Heal(float _amount)
    {
        base.Heal(_amount);
        
        // UI 업데이트
        if (uiManager != null)
        {
            uiManager.OnHeal();
        }
    }

    public bool IsAlive()
    {
        return !IsDead();
    }

    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealthPercent() => GetHealthPercent();
    public float GetCurrentHealth() => currentHealth;
}
