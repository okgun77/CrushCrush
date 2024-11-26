using UnityEngine;

[System.Serializable]

public class SpawnSettings
{
    [Header("Spawn Control")]
    public float spawnInterval = 2f;
    public float minimumSpawnInterval = 0.5f;
    public AnimationCurve spawnIntervalCurve;
    public int maxObjectCount = 10;

    [Header("Spawn Count")]
    public int minSpawnCount = 1;
    public int maxSpawnCount = 3;
    public AnimationCurve spawnCountCurve;

    [Header("Movement Settings")]
    public MovementType[] availablePatterns;
    public float baseSpeed = 5f;
    public float maxSpeedMultiplier = 2f;
    public AnimationCurve speedCurve;

    [Header("Pattern Settings")]
    public float[] patternWeights = new float[] { 1f, 0.5f, 0.3f };
    public float patternAmplitude = 1f;
    public float patternFrequency = 1f;

    [Header("Rotation Settings")]
    public Vector3 baseRotationSpeed = new Vector3(50, 50, 50);
    public float rotationSpeedMultiplier = 1f;
    public AnimationCurve rotationSpeedCurve;

    public float GetSpawnInterval(float progress)
    {
        if (spawnIntervalCurve == null) return spawnInterval;
        float t = spawnIntervalCurve.Evaluate(progress);
        return Mathf.Lerp(spawnInterval, minimumSpawnInterval, t);
    }
    
    public float GetSpeedMultiplier(float progress)
    {
        if (speedCurve == null) return 1f;
        return speedCurve.Evaluate(progress) * maxSpeedMultiplier;
    }
    
    public int GetSpawnCount(float progress)
    {
        if (spawnCountCurve == null) return minSpawnCount;
        float t = spawnCountCurve.Evaluate(progress);
        return Mathf.RoundToInt(Mathf.Lerp(minSpawnCount, maxSpawnCount, t));
    }
}