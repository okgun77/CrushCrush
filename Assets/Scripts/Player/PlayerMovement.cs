using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform planetCenter;
    [SerializeField] private MeshFilter planetMeshFilter;
    [SerializeField] private Camera mainCamera;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 30f;
    [SerializeField] private float maxTurnAngle = 45f;
    [SerializeField] private float smoothRotationTime = 0.5f;
    
    [Header("Vertical Movement")]
    [SerializeField] private float baseHeight = 4f;         // 기본 비행 높이
    [SerializeField] private float heightVariation = 2f;    // 랜덤 높이 변화량
    [SerializeField] private float waveAmplitude = 0.5f;    // 물결 움직임의 크기
    [SerializeField] private float waveSpeed = 1f;          // 물결 움직임의 속도
    [SerializeField] private float minHeight = 2f;          // 최소 비행 높이
    [SerializeField] private float maxHeight = 6f;          // 최대 비행 높이

    private float orbitRadius;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector3 currentMoveDirection;
    private float currentTurnAngle;
    private float turnTimer;
    private float targetTurnAngle;
    private float verticalOffset;
    private float baseOrbitRadius;  // 기본 궤도 반경 저장
    
    private float targetHeight;
    private float heightChangeTimer;
    private const float HEIGHT_CHANGE_INTERVAL = 4f;  // 높이 변경 간격

    private void Start()
    {
        if (!ValidateReferences()) return;
        
        InitializeMeshData();
        baseOrbitRadius = Vector3.Distance(transform.position, planetCenter.position);
        orbitRadius = baseOrbitRadius;
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 upDirection = (transform.position - planetCenter.position).normalized;
        currentMoveDirection = Vector3.ProjectOnPlane(transform.forward, upDirection).normalized;
        
        SetNewTurnTarget();
        SetNewVerticalTarget();  // 새로운 수직 움직임 목표 설정
    }

    private bool ValidateReferences()
    {
        if (planetCenter == null)
        {
            Debug.LogError("Planet Center reference is missing!");
            return false;
        }
        
        if (planetMeshFilter == null)
        {
            Debug.LogError("Planet MeshFilter reference is missing!");
            return false;
        }

        return true;
    }

    private void InitializeMeshData()
    {
        Mesh planetMesh = planetMeshFilter.sharedMesh;
        vertices = planetMesh.vertices;
        normals = planetMesh.normals;
    }

    private void Update()
    {
        MoveAroundPlanet();
    }

    private void SetNewTurnTarget()
    {
        // 랜덤한 회전 각도 설정 (-maxTurnAngle에서 maxTurnAngle 사이)
        targetTurnAngle = Random.Range(-maxTurnAngle, maxTurnAngle);
        turnTimer = 0f;
    }

    private void SetNewVerticalTarget()
    {
        targetHeight = Random.Range(minHeight, maxHeight);
        heightChangeTimer = 0f;
    }

    private void MoveAroundPlanet()
    {
        if (mainCamera == null) return;

        UpdateVerticalMovement();  // 수직 움직임 업데이트
        
        Vector3 upDirection = (transform.position - planetCenter.position).normalized;
        
        // 회전 로직
        turnTimer += Time.deltaTime;
        if (turnTimer > 3f)
        {
            SetNewTurnTarget();
        }

        currentTurnAngle = Mathf.Lerp(currentTurnAngle, targetTurnAngle, turnSpeed * Time.deltaTime);
        
        currentMoveDirection = Quaternion.AngleAxis(
            currentTurnAngle * Time.deltaTime, 
            upDirection
        ) * currentMoveDirection;

        currentMoveDirection = Vector3.ProjectOnPlane(currentMoveDirection, upDirection).normalized;

        // 이동 및 높이 적용
        Vector3 movement = currentMoveDirection * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;
        Vector3 newUpDirection = (newPosition - planetCenter.position).normalized;
        
        // 현재 orbitRadius를 사용하여 최종 위치 계산
        Vector3 finalPosition = planetCenter.position + (newUpDirection * orbitRadius);
        transform.position = finalPosition;

        if (currentMoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection, newUpDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime / smoothRotationTime
            );
        }
    }

    private void UpdateVerticalMovement()
    {
        // 높이 변경 타이머 업데이트
        heightChangeTimer += Time.deltaTime;
        if (heightChangeTimer >= HEIGHT_CHANGE_INTERVAL)
        {
            // 현재 높이를 고려하여 다음 목표 높이 설정
            float currentRelativeHeight = orbitRadius - baseOrbitRadius;
            
            // 현재 높이가 최소/최대 높이에 가까울 때는 반대 방향으로 목표 설정
            if (currentRelativeHeight < minHeight + heightVariation)
            {
                targetHeight = baseHeight + Random.Range(0, heightVariation);  // 위로 이동
            }
            else if (currentRelativeHeight > maxHeight - heightVariation)
            {
                targetHeight = baseHeight + Random.Range(-heightVariation, 0);  // 아래로 이동
            }
            else
            {
                targetHeight = baseHeight + Random.Range(-heightVariation, heightVariation);
            }
            
            heightChangeTimer = 0f;
        }

        // 현재 높이를 목표 높이로 부드럽게 보간
        float currentHeight = orbitRadius - baseOrbitRadius;
        float newHeight = Mathf.Lerp(currentHeight, targetHeight, waveSpeed * Time.deltaTime);
        
        // 물결 움직임의 크기를 높이에 따라 동적으로 조절
        float heightRatio = Mathf.InverseLerp(minHeight, maxHeight, currentHeight);
        float dynamicAmplitude = waveAmplitude * (1f - Mathf.Abs(heightRatio - 0.5f) * 2f);
        
        // 부드러운 물결 움직임 추가
        float waveOffset = Mathf.Sin(Time.time * waveSpeed) * dynamicAmplitude;
        
        // 최종 궤도 반경 설정
        orbitRadius = baseOrbitRadius + newHeight + waveOffset;
        
        // 부드러운 제한을 위한 추가 보간
        if (orbitRadius < baseOrbitRadius + minHeight)
        {
            float blend = Mathf.SmoothStep(0, 1, (orbitRadius - (baseOrbitRadius + minHeight - 1f)));
            orbitRadius = Mathf.Lerp(orbitRadius, baseOrbitRadius + minHeight, blend);
        }
        else if (orbitRadius > baseOrbitRadius + maxHeight)
        {
            float blend = Mathf.SmoothStep(0, 1, ((baseOrbitRadius + maxHeight + 1f) - orbitRadius));
            orbitRadius = Mathf.Lerp(orbitRadius, baseOrbitRadius + maxHeight, blend);
        }
    }
}
