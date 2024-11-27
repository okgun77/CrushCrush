using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;

    public void ProcessTouchAttack(IDamageable _target)
    {
        if (_target != null)
        {
            playerAttack.AttackTarget(_target);
        }
    }
}
