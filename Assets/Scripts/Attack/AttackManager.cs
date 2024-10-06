using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] private int attackPower = 10;  // 기본 공격력

    // 공격력을 설정하는 함수
    public void SetAttackPower(int power)
    {
        attackPower = power;
    }

    // 공격을 수행하는 함수 (공격 대상의 DamageHandler를 호출)
    public void PerformAttack(GameObject target)
    {
        DamageHandler damageHandler = target.GetComponent<DamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.TakeDamage(attackPower);  // 대상에게 공격력만큼의 데미지 전달
        }
        else
        {
            Debug.Log("타겟에 DamageHandler가 없습니다.");
        }
    }

    // 공격력 반환 함수
    public int GetAttackPower()
    {
        return attackPower;
    }
}
