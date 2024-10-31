using UnityEngine;

public class JunkOrbitEffect : MonoBehaviour 
{
    [SerializeField] private Transform planetCenter;
    [SerializeField] private float orbitSpeed = 5f;
    [SerializeField] private float wobbleAmount = 0.1f;
    
    private Vector3 orbitAxis;
    private float startRadius;
    
    private void Start()
    {
        orbitAxis = Random.onUnitSphere; // 랜덤한 공전 축
        startRadius = Vector3.Distance(transform.position, planetCenter.position);
    }
    
    private void Update()
    {
        // 행성 중심을 기준으로 공전
        transform.RotateAround(planetCenter.position, orbitAxis, orbitSpeed * Time.deltaTime);
        
        // 현재 반경 유지하면서 약간의 흔들림 추가
        Vector3 fromCenter = transform.position - planetCenter.position;
        float wobble = 1f + Mathf.Sin(Time.time) * wobbleAmount;
        transform.position = planetCenter.position + fromCenter.normalized * (startRadius * wobble);
    }
}
