using UnityEngine;

public class SpawnableObject : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waveMagnitude = 2f;    // 좌우 흔들림 크기
    [SerializeField] private float waveFrequency = 2f;    // 흔들림 주기
    [SerializeField] private bool randomizeMovement = true; // 랜덤 움직임 여부
    
    private Camera playerCamera;
    private Vector3 moveDirection;
    private Vector3 startPosition;
    private float timeOffset;
    private float randomWaveSpeed;
    
    private void Awake()
    {
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("MainCamera not found!");
        }
        
        // 각 오브젝트마다 다른 시작 시간과 속도를 가지도록 설정
        if (randomizeMovement)
        {
            timeOffset = Random.Range(0f, Mathf.PI * 2);
            randomWaveSpeed = Random.Range(0.5f, 1.5f);
        }
    }
    
    private void Start()
    {
        startPosition = transform.position;
        InitializeMovement();
    }
    
    private void InitializeMovement()
    {
        moveDirection = (playerCamera.transform.position - transform.position).normalized;
    }
    
    private void Update()
    {
        // 기본 진행 방향 계산
        moveDirection = (playerCamera.transform.position - transform.position).normalized;
        
        // 좌우 움직임 추가
        Vector3 waveMotion = CalculateWaveMotion();
        
        // 최종 이동
        Move(waveMotion);
    }
    
    private Vector3 CalculateWaveMotion()
    {
        float time = Time.time * waveFrequency * randomWaveSpeed + timeOffset;
        
        // moveDirection을 기준으로 한 오른쪽 벡터 계산
        Vector3 right = Vector3.Cross(moveDirection, Vector3.up).normalized;
        
        // Sin 함수를 사용하여 좌우 움직임 계산
        return right * Mathf.Sin(time) * waveMagnitude;
    }
    
    private void Move(Vector3 waveMotion)
    {
        // 기본 전진 움직임과 파도 움직임을 합성
        Vector3 movement = (moveDirection * moveSpeed + waveMotion) * Time.deltaTime;
        transform.position += movement;
        
        // 오브젝트가 진행 방향을 바라보도록 회전 (선택사항)
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }
    
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
    
    // 움직임 패턴 변경을 위한 메서드들
    public void SetWaveMagnitude(float magnitude)
    {
        waveMagnitude = magnitude;
    }
    
    public void SetWaveFrequency(float frequency)
    {
        waveFrequency = frequency;
    }
}
