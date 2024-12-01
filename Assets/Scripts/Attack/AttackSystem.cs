using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private float attackDamage = 10f;
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

    public void Attack(IDamageable target)
    {
        if (target == null) return;

        float currentTime = Time.time;
        if (currentTime >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"Attacking target with damage: {attackDamage}");
            
            Vector3 targetPosition = (target as MonoBehaviour)?.transform.position ?? Vector3.zero;
            var targetTransform = (target as MonoBehaviour)?.transform;
            
            var objProps = (target as MonoBehaviour)?.GetComponent<ObjectProperties>();
            if (objProps != null)
            {
                float currentHealth = objProps.GetHealth();

                if (currentHealth <= attackDamage)
                {
                    // Break 랜덤 이펙트 재생
                    effectManager.PlayRandomEffect(EEffectType.BREAK, targetPosition, 0.5f, targetTransform);
                }
                else
                {
                    // Hit 랜덤 사운드 재생
                    int randomSound = random.Next(1, 11);
                    audioManager.PlaySFX($"Hit_{randomSound:D2}");
                    
                    // Hit 랜덤 이펙트 재생
                    effectManager.PlayRandomEffect(EEffectType.HIT, targetPosition, 0.5f, targetTransform);
                }
            }

            // 데미지 텍스트 생성
            if (damageTextPrefab != null)
            {
                Vector3 textPosition = targetPosition + Vector3.up * 1f; // 타겟 위치보다 1유닛 위에 생성
                GameObject damageTextObj = Instantiate(damageTextPrefab, textPosition, Quaternion.identity);
                damageTextObj.GetComponent<DamageText>()?.Setup(attackDamage);
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

