using UnityEngine;

public class RotateObject : MonoBehaviour, IMovable
{
    [SerializeField] private Vector3 baseRotationSpeed = new Vector3(50, 50, 50);
    private Vector3 currentRotationSpeed;
    private bool isPaused = false;
    private Vector3 centerOffset;
    private GameObject rotationCenter;
    
    private void Start()
    {
        currentRotationSpeed = baseRotationSpeed;
        SetupRotationCenter();
    }

    private void SetupRotationCenter()
    {
        // 모든 Renderer의 Bounds를 합쳐서 실제 오브젝트의 중심점 계산
        Bounds combinedBounds = new Bounds();
        bool firstBound = true;
        
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (firstBound)
            {
                combinedBounds = renderer.bounds;
                firstBound = false;
            }
            else
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }
        }

        if (renderers.Length > 0)
        {
            // 실제 중심점과 현재 피벗 포인트의 차이 계산
            centerOffset = combinedBounds.center - transform.position;
            
            // 회전 중심점을 위한 빈 게임오브젝트 생성
            rotationCenter = new GameObject("RotationCenter");
            rotationCenter.transform.position = combinedBounds.center;
            
            // 현재 오브젝트를 회전 중심점의 자식으로 설정
            Transform originalParent = transform.parent;
            transform.parent = rotationCenter.transform;
            rotationCenter.transform.parent = originalParent;
        }
    }

    private void Update()
    {
        if (!isPaused && rotationCenter != null)
        {
            rotationCenter.transform.Rotate(currentRotationSpeed * Time.deltaTime);
        }
    }

    public void UpdateRotation(Vector3 movementDirection, float speedMultiplier)
    {
        if (rotationCenter != null)
        {
            float speed = movementDirection.magnitude * speedMultiplier;
            Vector3 rotationAxis = Vector3.Cross(Vector3.forward, movementDirection).normalized;
            currentRotationSpeed = baseRotationSpeed + (rotationAxis * speed * 50f);
        }
    }

    public void SetBaseRotationSpeed(Vector3 rotationSpeed)
    {
        baseRotationSpeed = rotationSpeed;
        currentRotationSpeed = baseRotationSpeed;
    }

    public void SetPaused(bool _isPaused)
    {
        isPaused = _isPaused;
    }

    private void OnDestroy()
    {
        // 회전 중심점 오브젝트 제거
        if (rotationCenter != null)
        {
            Destroy(rotationCenter);
        }
    }
}
