public enum ConditionType
{
    Time,           // 시간 경과
    DestroyCount,   // 파괴한 오브젝트 수
    Score,          // 점수
    None            // 조건 없음
}

public enum StageState
{
    Ready,          // 준비
    InProgress,     // 진행중
    Complete,       // 완료
    Failed          // 실패
}