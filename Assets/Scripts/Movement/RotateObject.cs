using UnityEngine;

public class RotateObject : MonoBehaviour, IMovable
{
    [SerializeField] private Vector3 baseRotationSpeed = new Vector3(5, 5, 5);
    private Vector3 currentRotationSpeed;
    private bool isPaused = false;

    private void Start()
    {
        currentRotationSpeed = baseRotationSpeed;
    }

    private void Update()
    {
        if (!isPaused)
        {
            transform.Rotate(currentRotationSpeed * Time.deltaTime);
        }
    }

    public void UpdateRotation(Vector3 movementDirection, float speedMultiplier)
    {
        float speed = movementDirection.magnitude * speedMultiplier;
        Vector3 rotationAxis = Vector3.Cross(Vector3.forward, movementDirection).normalized;
        currentRotationSpeed = baseRotationSpeed + (rotationAxis * speed * 50f);
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
}
