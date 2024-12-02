using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private HealthSystem healthSystem;
    private ObjectProperties objectProperties;

    private void Awake()
    {
        healthSystem = new HealthSystem();
        objectProperties = GetComponent<ObjectProperties>();
        
        // ObjectProperties의 체력값을 사용하도록 수정
        healthSystem.SetMaxHealth(objectProperties.GetHealth());
    }

    public void TakeDamage(float _damage)
    {
        healthSystem.TakeDamage(_damage);
        // ObjectProperties의 체력도 같이 감소
        objectProperties.TakeDamage(_damage);

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
        healthSystem = null;
    }
}
