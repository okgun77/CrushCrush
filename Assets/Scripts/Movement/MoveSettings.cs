using UnityEngine;

[CreateAssetMenu(fileName = "MoveSettings", menuName = "ScriptableObjects/MoveSettings", order = 1)]
public class MoveSettings : ScriptableObject
{
    public MonoBehaviour movementBehavior;  // 움직임 관련 클래스
    public float speed;                     // 이동 속도 (MoveToTargetPoint 전용)
    public Vector3 rotationSpeed;           // 회전 속도 (RotateObject 전용)
    public float horizontalZigzagWidth;     // 좌우 지그재그 이동의 폭 (MoveZigZagHorizontal 전용)
    public float verticalZigzagHeight;      // 상하 지그재그 이동의 높이 (MoveZigZagVertical 전용)
    public float frequency;                 // 지그재그 주파수 (MoveZigZag 전용)
    public float difficultyThreshold;       // 난이도 임계값
}
