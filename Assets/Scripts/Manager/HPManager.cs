using UnityEngine;
using TMPro;

public class HPManager : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    private int currentHP;
    private UIManager uiManager;

    public void Init()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager를 찾을 수 없습니다!");
            return;
        }

        currentHP = maxHP;
        uiManager.UpdateHPUI(currentHP, maxHP, true); // 초기화 시 즉시 업데이트
    }

    public void TakeDamage(int _damage)
    {
        currentHP -= _damage;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        uiManager.UpdateHPUI(currentHP, maxHP);

        // 데미지를 입을 때 HP 슬라이더를 깜빡이게 함
        uiManager.BlinkHPSlider();

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
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
}
