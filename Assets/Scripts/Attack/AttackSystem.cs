using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;

    private float lastAttackTime;

    public float GetDamage()
    {
        return baseDamage;
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + (1f / attackSpeed);
    }

    public void Attack(IDamageable _target)
    {
        if (CanAttack() && _target != null)
        {
            _target.TakeDamage(GetDamage());
            lastAttackTime = Time.time;
        }
    }


}
