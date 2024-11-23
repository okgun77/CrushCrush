using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/Stage Data")]

public class StageData : ScriptableObject
{
    [System.Serializable]
    public class Stage
    {
        public string stageName;
        public SpawnSettings spawnSettings;
        public StageCondition[] conditions;
        public float duration = 60f;
        [TextArea(3, 5)]
        public string StageDescription;
    }
    
    public Stage[] stages;
}