using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game/Score Data")]
public class ScoreDataSO : ScriptableObject
{
    [System.Serializable]  // 직렬화 속성 추가
    public struct ScoreData
    {
        public EScoreType scoreType;
        public int baseScore;
        public float distanceMultiplier;
        public float fragmentMultiplier;
    }

    public List<ScoreData> scoreDataList = new List<ScoreData>();
}