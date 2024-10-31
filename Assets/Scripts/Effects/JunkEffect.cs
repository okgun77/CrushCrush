using UnityEngine;

public class JunkEffect : MonoBehaviour
{
    private Vector3 startPosition;
    private float randomOffset;
    private Vector3 rotationAxis;
    private float floatAmplitude;
    private float floatFrequency;
    private float rotationSpeed;
    private float moveRadius;
    private float moveSpeed;

    public void Initialize(JunkEffectSettings settings)
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, Mathf.PI * 2);
        rotationAxis = Random.onUnitSphere;
        
        floatAmplitude = Random.Range(settings.floatAmplitudeRange.x, settings.floatAmplitudeRange.y);
        floatFrequency = Random.Range(settings.floatFrequencyRange.x, settings.floatFrequencyRange.y);
        rotationSpeed = Random.Range(settings.rotationSpeedRange.x, settings.rotationSpeedRange.y);
        moveRadius = Random.Range(settings.moveRadiusRange.x, settings.moveRadiusRange.y);
        moveSpeed = Random.Range(settings.moveSpeedRange.x, settings.moveSpeedRange.y);
    }

    private void Update()
    {
        float verticalOffset = floatAmplitude * Mathf.Sin((Time.time + randomOffset) * floatFrequency);
        float horizontalOffset = Mathf.Cos(Time.time * moveSpeed + randomOffset) * moveRadius;
        float depthOffset = Mathf.Sin(Time.time * moveSpeed + randomOffset) * moveRadius;
        
        Vector3 newPosition = startPosition + new Vector3(horizontalOffset, verticalOffset, depthOffset);
        transform.position = newPosition;
        
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
} 