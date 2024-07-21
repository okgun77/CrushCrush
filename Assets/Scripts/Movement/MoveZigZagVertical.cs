using UnityEngine;

public class MoveZigZagVertical : MonoBehaviour, IMovementCondition
{
    [SerializeField] private float verticalZigzagHeight; // 상하 지그재그 이동의 높이
    [SerializeField] private float frequency; // 지그재그 주파수 (왕복 시간)
    [SerializeField] private float difficultyThreshold; // 추가될 난이도 임계값

    private Vector3 startPosition;
    private float time;

    private void Start()
    {
        startPosition = transform.position;
        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime * frequency;

        // 새로운 y 위치 계산 (cos 파형을 사용하여 지그재그 패턴 생성)
        float newY = startPosition.y + Mathf.Cos(time) * verticalZigzagHeight;

        // 현재 위치를 유지하면서 y 위치만 변경
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public bool ShouldAddBehavior(float gameTime)
    {
        return gameTime > difficultyThreshold;
    }
}
