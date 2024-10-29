using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform planetCenter;
    [SerializeField] private MeshFilter planetMeshFilter;
    [SerializeField] private Camera mainCamera;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 30f;  // 자동 회전 속도
    [SerializeField] private float maxTurnAngle = 45f;  // 최대 회전 각도
    [SerializeField] private float smoothRotationTime = 0.5f;  // 회전 부드러움 정도
    
    private float orbitRadius;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector3 currentMoveDirection;
    private float currentTurnAngle;
    private float turnTimer;
    private float targetTurnAngle;

    private void Start()
    {
        if (!ValidateReferences()) return;
        
        InitializeMeshData();
        orbitRadius = Vector3.Distance(transform.position, planetCenter.position);
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 초기 이동 방향 설정
        Vector3 upDirection = (transform.position - planetCenter.position).normalized;
        currentMoveDirection = Vector3.ProjectOnPlane(transform.forward, upDirection).normalized;
        
        // 초기 회전 목표 설정
        SetNewTurnTarget();
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

    private void MoveAroundPlanet()
    {
        if (mainCamera == null) return;

        // 현재 위치에서 행성 중심까지의 방향 (up 방향)
        Vector3 upDirection = (transform.position - planetCenter.position).normalized;
        
        // 시간에 따른 자동 회전
        turnTimer += Time.deltaTime;
        if (turnTimer > 3f)  // 3초마다 새로운 회전 목표 설정
        {
            SetNewTurnTarget();
        }

        // 현재 회전 각도를 목표 각도로 부드럽게 보간
        currentTurnAngle = Mathf.Lerp(currentTurnAngle, targetTurnAngle, turnSpeed * Time.deltaTime);

        // 현재 이동 방향을 부드럽게 회전
        currentMoveDirection = Quaternion.AngleAxis(
            currentTurnAngle * Time.deltaTime, 
            upDirection
        ) * currentMoveDirection;

        // 이동 방향이 수평면에 있도록 보정
        currentMoveDirection = Vector3.ProjectOnPlane(currentMoveDirection, upDirection).normalized;

        // 이동 방향으로 위치 이동
        Vector3 movement = currentMoveDirection * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;

        // 새로운 위치에서 행성 중심까지의 방향
        Vector3 newUpDirection = (newPosition - planetCenter.position).normalized;
        
        // 새로운 위치를 행성 표면에 맞추기
        Vector3 finalPosition = planetCenter.position + (newUpDirection * orbitRadius);
        transform.position = finalPosition;

        // 부드러운 회전 적용 (새로운 up 방향 기준)
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
}
