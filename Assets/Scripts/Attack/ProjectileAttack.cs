using UnityEngine;

public class ProjectileAttack : AttackSystem
{
    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            PerformAttack(damageable);
            gameObject.SetActive(false); // 오브젝트 풀링을 위해 비활성화
        }
    }
}
