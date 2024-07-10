using UnityEngine;
using TMPro;

public class HPManager : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private TextMeshProUGUI hpText;

    private int currentHP;

    public void Init()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }

    public void TakeDamage(int _damage)
    {
        currentHP -= _damage;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        UpdateHPUI();

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
        UpdateHPUI();
    }

    private void UpdateHPUI()
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP}";
        }
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
}
