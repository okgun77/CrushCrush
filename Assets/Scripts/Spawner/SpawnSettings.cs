[System.Serializable]

public class SpawnSettings
{
    public float spawnInterval = 2f;
    public MovementType[] availablePatterns;
    public float movementSpeed = 5f;
    public float patternAmplitude = 1f;
    public float patternFrequency = 1f;
    public int maxObjectCount = 10;
}
