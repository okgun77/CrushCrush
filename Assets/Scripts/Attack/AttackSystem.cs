using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float damageVariance = 0.3f;
    [SerializeField] private float minDamageMultiplier = 0.7f;
    [SerializeField] private float attackCooldown = 0.1f;
    private float lastAttackTime;
    private AudioManager audioManager;
    private EffectManager effectManager;
    private System.Random random;
    [SerializeField] private GameObject damageTextPrefab;

    private void Awake()
    {
        lastAttackTime = -attackCooldown;
        audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager를 찾을 수 없습니다!");
        }
        effectManager = FindFirstObjectByType<EffectManager>();
        if (effectManager == null)
        {
            Debug.LogError("EffectManager를 찾을 수 없습니다!");
        }
        random = new System.Random();
    }

    private float CalculateDamage()
    {
        float minDamage = baseDamage * minDamageMultiplier;
        float maxDamage = baseDamage * (1f + damageVariance);
        return Random.Range(minDamage, maxDamage);
    }

    public void Attack(IDamageable target)
    {
        if (target == null) return;

        float currentTime = Time.time;
        if (currentTime >= lastAttackTime + attackCooldown)
        {
            float finalDamage = CalculateDamage();
            Debug.Log($"Attacking target with damage: {finalDamage}");
            
            Vector3 targetPosition = (target as MonoBehaviour)?.transform.position ?? Vector3.zero;
            var targetTransform = (target as MonoBehaviour)?.transform;
            
            var objProps = (target as MonoBehaviour)?.GetComponent<ObjectProperties>();
            if (objProps != null)
            {
                float currentHealth = objProps.GetHealth();

                if (currentHealth <= finalDamage)
                {
                    effectManager.PlayRandomEffect(EEffectType.BREAK, targetPosition, 0.5f, targetTransform);
                }
                else
                {
                    int randomSound = random.Next(1, 11);
                    audioManager.PlaySFX($"Hit_{randomSound:D2}");
                    effectManager.PlayRandomEffect(EEffectType.HIT, targetPosition, 0.5f, targetTransform);
                }
            }

            if (damageTextPrefab != null)
            {
                Vector3 textPosition = targetPosition + Vector3.up * 1f;
                GameObject damageTextObj = Instantiate(damageTextPrefab, textPosition, Quaternion.identity);
                damageTextObj.GetComponent<DamageText>()?.Setup(finalDamage);
            }

            target.TakeDamage(finalDamage);
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
        baseDamage = damage;
    }

    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }

    public void SetDamageVariance(float variance)
    {
        damageVariance = Mathf.Clamp(variance, 0f, 1f);
    }

    public void SetMinDamageMultiplier(float multiplier)
    {
        minDamageMultiplier = Mathf.Clamp(multiplier, 0f, 1f);
    }
}

