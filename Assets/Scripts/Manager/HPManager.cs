using UnityEngine;
using TMPro;

public class HPManager : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    private int currentHP;
    private UIManager uiManager;
    private GameManager gameManager;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
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
        uiManager.BlinkHPSlider();

        if (currentHP == 0)
        {
            gameManager.GameOver();
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
