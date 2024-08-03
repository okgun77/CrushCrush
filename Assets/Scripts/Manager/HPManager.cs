using UnityEngine;
using TMPro;

public class HPManager : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int consecutiveDestroyThreshold = 5; // 연속 파괴 임계값
    [SerializeField] private int hpIncreaseAmount = 10; // HP 증가량

    private int currentHP;
    private int consecutiveDestroys = 0;
    private UIManager uiManager;

    public void Init(GameManager _gameManager)
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager를 찾을 수 없습니다!");
            return;
        }

        currentHP = maxHP;
        uiManager.UpdateHPUI(currentHP, maxHP, true);
    }

    public void TakeDamage(int _damage)
    {
        currentHP -= _damage;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        uiManager.UpdateHPUI(currentHP, maxHP);
        uiManager.OnDamageTaken(); // 데미지를 입으면 빨간색으로 깜빡임
        ResetConsecutiveDestroys(); // 데미지를 입으면 연속 파괴 횟수 초기화

        if (currentHP == 0)
        {
            FindObjectOfType<GameManager>().GameOver();
        }
    }

    public void Heal(int _heal)
    {
        currentHP += _heal;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        uiManager.UpdateHPUI(currentHP, maxHP);
        uiManager.OnHeal(); // 힐을 받으면 녹색으로 깜빡임
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public void IncreaseConsecutiveDestroys()
    {
        consecutiveDestroys++;
        if (consecutiveDestroys >= consecutiveDestroyThreshold)
        {
            Heal(hpIncreaseAmount);
            consecutiveDestroys = 0; // 리셋
        }
    }

    public void ResetConsecutiveDestroys()
    {
        consecutiveDestroys = 0;
    }
}
