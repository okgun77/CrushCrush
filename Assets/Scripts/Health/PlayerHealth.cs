using UnityEngine;

public class PlayerHealth : HealthSystem
{
    [SerializeField] private int consecutiveDestroyThreshold = 5;
    [SerializeField] private int hpIncreaseAmount = 10;
    
    private int consecutiveDestroys = 0;
    private UIManager uiManager;
    private GameManager gameManager;

    protected override void Awake()
    {
        base.Awake();
        uiManager = FindAnyObjectByType<UIManager>();
        gameManager = FindAnyObjectByType<GameManager>();

        if (uiManager == null)
            Debug.LogError("UIManager를 찾을 수 없습니다!");
        if (gameManager == null)
            Debug.LogError("GameManager를 찾을 수 없습니다!");
    }

    private void Start()
    {
        UpdateUI(true);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UpdateUI();
        uiManager?.OnDamageTaken();
        ResetConsecutiveDestroys();
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
        UpdateUI();
        uiManager?.OnHeal();
    }

    private void UpdateUI(bool forceUpdate = false)
    {
        uiManager?.UpdateHPUI(currentHealth, maxHealth, forceUpdate);
    }

    protected override void Die()
    {
        gameManager?.GameOver();
    }

    public void IncreaseConsecutiveDestroys()
    {
        consecutiveDestroys++;
        if (consecutiveDestroys >= consecutiveDestroyThreshold)
        {
            Heal(hpIncreaseAmount);
            consecutiveDestroys = 0;
        }
    }

    public void ResetConsecutiveDestroys()
    {
        consecutiveDestroys = 0;
    }
}
