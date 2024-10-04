using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10; // 플레이어의 공격력

    // 프로퍼티
    public int AttackDamage
    {
        get => attackDamage;
        set => attackDamage = value;
    }
}
