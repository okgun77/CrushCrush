using UnityEngine;

public class JunkCombinedEffect : MonoBehaviour 
{
    [Header("Float Settings")]
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Vector3 rotationAxis;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveRadius = 1f;
    [SerializeField] private float moveSpeed = 1f;
    
    private Vector3 startPosition;
    private float randomOffset;
    private Quaternion startRotation;
    
    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        randomOffset = Random.Range(0f, Mathf.PI * 2);
        rotationAxis = Random.onUnitSphere;
    }
    
    private void Update()
    {
        // 부유 효과
        float verticalOffset = floatAmplitude * Mathf.Sin((Time.time + randomOffset) * floatFrequency);
        
        // 원형 움직임
        float horizontalOffset = Mathf.Cos(Time.time * moveSpeed + randomOffset) * moveRadius;
        float depthOffset = Mathf.Sin(Time.time * moveSpeed + randomOffset) * moveRadius;
        
        Vector3 newPosition = startPosition + new Vector3(horizontalOffset, verticalOffset, depthOffset);
        transform.position = newPosition;
        
        // 자연스러운 회전
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
