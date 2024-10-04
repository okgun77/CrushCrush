using UnityEngine;

[System.Serializable]
public class ObjectTypes : MonoBehaviour
{
    [SerializeField] private string type;           // 오브젝트의 종류: 일반, 폭발, 파괴 불가능
    [SerializeField] private float  health;         // 체력
    [SerializeField] private float  mass;           // 질량 (터치 시 밀려나는 정도)
    [SerializeField] private bool   isDestructible; // 파괴 가능 여부

    public string Type => type;
    public float Health => health;
    public float Mass => mass;
    public bool IsDestructible => isDestructible;

    // Inspector에서 설정된 값으로 초기화하기 위한 기본 생성자
    public ObjectTypes(string _type, float _health, float _mass, bool _isDestructible)
    {
        this.type = _type;
        this.health = _health;
        this.mass = _mass;
        this.isDestructible = _isDestructible;
    }
}
