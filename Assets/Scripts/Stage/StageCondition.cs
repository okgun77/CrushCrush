[System.Serializable]

public class StageCondition
{
    public string conditionName;    // 조건 이름
    public ConditionType type;      // 조건 타입
    public float targetValue;       // 목표값
    public bool isSatisfied;        // 달성 여부

    public bool CheckCondition(float _currentValue)
    {
        isSatisfied = _currentValue >= targetValue;
        return isSatisfied;
    }

}
