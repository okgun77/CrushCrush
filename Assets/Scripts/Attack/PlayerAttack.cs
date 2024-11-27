using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;


    private void Awake()
    {
        // AttackSystem이 없다면 추가
        if (attackSystem == null)
        {
            attackSystem = gameObject.AddComponent<AttackSystem>();
        }
    }

    public void AttackTarget(IDamageable _target)
    {
        if (_target != null)
        {
            attackSystem.Attack(_target);
        }
    }
}
