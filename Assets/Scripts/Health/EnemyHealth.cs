using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private HealthSystem healthSystem;
    private ObjectProperties objectProperties;

    private void Awake()
    {
        healthSystem = gameObject.AddComponent<HealthSystem>();
        objectProperties = GetComponent<ObjectProperties>();
        
        // ObjectProperties의 체력 값을 HealthSystem에 적용
        healthSystem.SetMaxHealth(objectProperties.GetDefaultHealth());
    }

    public void TakeDamage(float _damage)
    {
        healthSystem.TakeDamage(_damage);

        if (!IsAlive())
        {
            OnEnemyDeath();
        }
    }

    public bool IsAlive()
    {
        return !healthSystem.IsDead();
    }

    private void OnEnemyDeath()
    {
        // 기존 BreakObject 로직 실행
        var breakObject = gameObject.AddComponent<BreakObject>();
        breakObject.Initialize(objectProperties.GetScoreType(), 
                             objectProperties.GetFragmentLevel(), 
                             2.0f);
        breakObject.OnTouch();
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            Destroy(healthSystem);
        }
    }
}
