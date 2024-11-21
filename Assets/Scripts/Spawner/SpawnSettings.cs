using UnityEngine;

[System.Serializable]

public class SpawnSettings
{
    [Header("Spawn Control")]
    public float spawnInterval = 2f;
    public float minimumSpawnInterval = 0.5f;
    public float spawnIntervalDecreaseRate = 0.1f;
    public int maxObjectCount = 10;

    [Header("Movement Settings")]
    public MovementType[] availablePatterns;
    public float movementSpeed = 5f;
    public float patternAmplitude = 1f;
    public float patternFrequency = 1f;
}
