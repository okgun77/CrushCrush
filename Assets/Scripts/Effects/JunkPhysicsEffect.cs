using UnityEngine;

public class JunkPhysicsEffect : MonoBehaviour 
{
    [SerializeField] private float forceAmount = 1f;
    [SerializeField] private float maxVelocity = 2f;
    
    private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        // 초기 랜덤 힘 적용
        rb.AddForce(Random.insideUnitSphere * forceAmount, ForceMode.Impulse);
    }
    
    private void FixedUpdate()
    {
        // 속도 제한
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
        
        // 주기적으로 랜덤한 힘 추가
        if (Random.value < 0.05f) // 5% 확률
        {
            rb.AddForce(Random.insideUnitSphere * forceAmount, ForceMode.Impulse);
        }
    }
}
