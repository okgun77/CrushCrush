using UnityEngine;

public class ContactAttack : AttackSystem
{
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private void OnCollisionStay(Collision collision)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            PerformAttack(damageable);
            lastAttackTime = Time.time;
        }
    }
}
