using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public class EnemyTypeData
    {
        public EObjectType enemyType;
        public int baseHealth;
        public int scoreValue;
    }

    public List<EnemyTypeData> enemyTypes = new List<EnemyTypeData>();

    public int GetBaseHealth(EObjectType type)
    {
        var data = enemyTypes.Find(x => x.enemyType == type);
        return data?.baseHealth ?? 100;
    }
}