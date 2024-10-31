using UnityEngine;

[CreateAssetMenu(fileName = "JunkEffectSettings", menuName = "Effects/Junk Effect Settings")]
public class JunkEffectSettings : ScriptableObject
{
    [Header("Float Settings")]
    public Vector2 floatAmplitudeRange = new Vector2(0.3f, 0.7f);
    public Vector2 floatFrequencyRange = new Vector2(0.8f, 1.2f);
    
    [Header("Rotation Settings")]
    public Vector2 rotationSpeedRange = new Vector2(20f, 40f);
    
    [Header("Movement Settings")]
    public Vector2 moveRadiusRange = new Vector2(0.8f, 1.2f);
    public Vector2 moveSpeedRange = new Vector2(0.8f, 1.2f);
} 