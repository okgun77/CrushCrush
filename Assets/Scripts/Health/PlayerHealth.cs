using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private UIManager uiManager;
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = new HealthSystem();
    }

    private void Start()
    {
        healthSystem.SetMaxHealth(maxHealth);
        
        uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not found!");
        }
    }

    public void TakeDamage(float _damage)
    {
        healthSystem.TakeDamage(_damage);
        Debug.Log($"Player took damage: {_damage}, Current Health: {healthSystem.GetCurrentHealthPercent() * 100}%");

        if (uiManager != null)
        {
            uiManager.OnDamageTaken();
        }

        if (healthSystem.IsDead())
        {
            OnPlayerDeath();
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Player Dead!!");
        
        if (uiManager != null)
        {
            uiManager.ShowGameOverUI();
        }
    }

    public void Heal(float _amount)
    {
        healthSystem.Heal(_amount);
        
        if (uiManager != null)
        {
            uiManager.OnHeal();
        }
    }

    public bool IsAlive() => !healthSystem.IsDead();
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealthPercent() => healthSystem.GetCurrentHealthPercent();
    public float GetCurrentHealth() => healthSystem.GetCurrentHealth();
}
