using UnityEngine;


[RequireComponent(typeof(PlayerHealth))]
public class PlayerHealth : HealthBase
{
    private UIManager uiManager;
    private GameManager gameManager;


    protected override void Awake()
    {
        base.Awake();

        uiManager = GetComponent<UIManager>();
        gameManager = GetComponent<GameManager>();

        // 이벤트 구독
        OnHealthChanged += UpdateHealthUI;
        OnDeath += OnPlayerDeath;
    }

    public override void TakeDamage(int _amount)
    {
        base.TakeDamage(_amount);

        // 추가로 데미지를 입었을 때 필요한 로직 구현
        uiManager?.OnDamageTaken();

        // 데미지를 받으면 연속 파괴 횟수 초기화
        gameManager?.ResetConsecutiveDestroys();
    }

    protected override void Die()
    {
        base.Die();

        // 플레이어 사망 시 게임 오버 처리
        gameManager?.GameOver();
    }

    private void UpdateHealthUI(int _currentHealth, int _maxHealth)
    {
        uiManager?.UpdateHPUI(_currentHealth, _maxHealth);
    }

    private void OnPlayerDeath()
    {
        // 플레이어 사망 시 추가 로직(애니메이션, 사운드등)
    }

    public void IncreaseConsecutiveDestroys()
    {
        // 게임 메니저이서 관리
        gameManager?.IncreaseConsecutiveDestroys();
    }

    public void ResetConsecutiveDestroys()
    {
        // 게임 매니저에서 관리
        gameManager?.ResetConsecutiveDestroys();
    }

}
