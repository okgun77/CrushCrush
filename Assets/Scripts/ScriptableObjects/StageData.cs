using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/Stage Data")]

public class StageData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnWeight
    {
        public EObjectType enemyType;
        [Range(0, 100)]
        public float spawnWeight = 1f;  // 해당 적의 출현 가중치
    }

    [System.Serializable]
    public class Phase
    {
        public string phaseName;
        public float duration = 30f;  // 페이즈 지속 시간
        public EnemySpawnWeight[] enemyTypes;  // 이 페이즈에서 출현할 적 타입들
        [TextArea(2, 4)]
        public string phaseDescription;
    }

    [System.Serializable]
    public class Stage
    {
        public string stageName;
        public SpawnSettings spawnSettings;
        public Phase[] phases;  // 스테이지의 페이즈들
        public StageCondition[] conditions;
        public float duration = 60f;
        [TextArea(3, 5)]
        public string stageDescription;
    }
    
    public Stage[] stages;
}