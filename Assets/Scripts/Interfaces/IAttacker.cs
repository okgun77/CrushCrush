using UnityEngine;

public interface IAttacker
{
    int GetAttackPower();
    void PerformAttack(IDamageable target);
}
