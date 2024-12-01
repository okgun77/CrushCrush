using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 0.1f;
    private float lastAttackTime;

    private void Awake()
    {
        lastAttackTime = -attackCooldown;
    }

    public void Attack(IDamageable target)
    {
        if (target == null) return;

        float currentTime = Time.time;
        if (currentTime >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"Attacking target with damage: {attackDamage}");
            target.TakeDamage(attackDamage);
            lastAttackTime = currentTime;
        }
        else
        {
            Debug.Log($"Cannot attack. Time remaining: {(lastAttackTime + attackCooldown) - currentTime}");
        }
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void SetAttackDamage(float damage)
    {
        attackDamage = damage;
    }

    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }
}

