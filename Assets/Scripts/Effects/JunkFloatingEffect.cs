using UnityEngine;

public class JunkFloatingEffect : MonoBehaviour 
{
    [SerializeField] private float amplitude = 0.5f;    // 움직임 크기
    [SerializeField] private float frequency = 1f;      // 움직임 속도
    [SerializeField] private float rotationSpeed = 30f; // 회전 속도
    
    private Vector3 startPosition;
    private float randomOffset;
    
    private void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // 각 오브젝트마다 다른 시작점
    }
    
    private void Update()
    {
        // 상하 부유 움직임
        float newY = startPosition.y + amplitude * Mathf.Sin((Time.time + randomOffset) * frequency);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // 천천히 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
