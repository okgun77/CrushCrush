using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private ObjectProperties objectProperties;  // 오브젝트의 체력 관리
    private BreakObject breakObject;            // 파괴 처리를 위한 BreakObject 참조

    private void Awake()
    {
        objectProperties = GetComponent<ObjectProperties>();
        breakObject = GetComponent<BreakObject>();

        if (objectProperties == null)
        {
            Debug.LogError("DamageHandler에 ObjectProperties가 없습니다!");
        }

        if (breakObject == null)
        {
            Debug.LogError("DamageHandler에 BreakObject가 없습니다!");
        }
    }

    // 공격을 받아 체력을 감소시키고 체력이 없으면 오브젝트 파괴
    public void TakeDamage(int damage)
    {
        if (objectProperties != null)
        {
            objectProperties.ReduceHealth(damage);  // 체력을 공격력만큼 감소

            Debug.Log($"오브젝트에 {damage}의 데미지를 입혔습니다. 남은 체력: {objectProperties.GetHealth()}");

            // 체력이 0 이하가 되면 파괴 처리
            if (objectProperties.GetHealth() <= 0)
            {
                if (breakObject != null)
                {
                    breakObject.HandleObjectDestruction();  // 파괴 로직 실행
                }
            }
        }
    }
}
