using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 0.1f;
    private float lastAttackTime;
    private AudioManager audioManager;
    private System.Random random;

    private void Awake()
    {
        lastAttackTime = -attackCooldown;
        audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager를 찾을 수 없습니다!");
        }
        random = new System.Random();
    }

    public void Attack(IDamageable target)
    {
        if (target == null) return;

        float currentTime = Time.time;
        if (currentTime >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"Attacking target with damage: {attackDamage}");
            
            var objProps = (target as MonoBehaviour)?.GetComponent<ObjectProperties>();
            if (objProps != null)
            {
                float currentHealth = objProps.GetHealth();
                if (currentHealth > attackDamage)
                {
                    int randomSound = random.Next(1, 11);
                    audioManager.PlaySFX($"Hit_{randomSound:D2}");
                }
            }

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

