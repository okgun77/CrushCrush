using UnityEngine;

public abstract class AttackSystem : MonoBehaviour, IAttacker
{
    [SerializeField] protected int attackPower = 10;
    
    public int GetAttackPower() => attackPower;
    
    public virtual void PerformAttack(IDamageable target)
    {
        if (target != null)
        {
            target.TakeDamage(attackPower);
        }
    }

    public void SetAttackPower(int power)
    {
        attackPower = power;
    }
}
