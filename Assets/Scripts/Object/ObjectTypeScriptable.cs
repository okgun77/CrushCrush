using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectType", menuName = "ObjectType")]
public class ObjectTypeScriptable : ScriptableObject
{
    public string   type;               // 오브젝트의 종류: 일반, 폭발, 파괴 불가능
    public float    health;             // 체력
    public float    mass;               // 질량 (터치 시 밀려나는 정도)
    public bool     isDestructible;     // 파괴 가능 여부
}
