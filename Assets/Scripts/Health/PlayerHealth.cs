using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthSystem healthSystem;

    private void Awake()
    {
        // HealthSystem이 없다면 추가
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<HealthSystem>();
        }
    }

    public void TakeDamage(float _damage)
    {
        healthSystem.TakeDamage(_damage);

        if (healthSystem.IsDead())
        {
            OnPlayerDeath();
        }
    }

    public bool IsAlive()
    {
        return !healthSystem.IsDead();
    }

    private void OnPlayerDeath()
    {
        // 플레이어 사망 처리
        Debug.Log("Player Dead!!");
        // GameManager에게 게임오버 알림등 추가 구현
    }
}
