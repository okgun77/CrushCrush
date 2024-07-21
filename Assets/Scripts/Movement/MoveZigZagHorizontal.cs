using UnityEngine;

public class MoveZigZagHorizontal : MonoBehaviour, IMovementCondition
{
    [SerializeField] private float horizontalZigzagWidth = 0.5f; // 좌우 지그재그 이동의 폭
    [SerializeField] private float frequency = 1f; // 지그재그 주파수 (왕복 시간)
    [SerializeField] private float difficultyThreshold = 20f; // 추가될 난이도 임계값

    private Vector3 startPosition;
    private float time;

    private void Start()
    {
        startPosition = transform.localPosition;
        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime * frequency;

        // 새로운 x 위치 계산 (sin 파형을 사용하여 지그재그 패턴 생성)
        float newX = startPosition.x + Mathf.Sin(time) * horizontalZigzagWidth;

        // 현재 위치를 유지하면서 x 위치만 변경
        Vector3 newPosition = new Vector3(newX, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = ClampPositionToScreen(newPosition);
    }

    public bool ShouldAddBehavior(float gameTime)
    {
        return gameTime > difficultyThreshold;
    }

    public void SetSettings(float _width, float _frequency, float _threshold)
    {
        horizontalZigzagWidth = _width;
        frequency = _frequency;
        difficultyThreshold = _threshold;
    }

    private Vector3 ClampPositionToScreen(Vector3 position)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(position);
        bool isClamped = false;

        if (viewportPosition.x < 0.1f || viewportPosition.x > 0.9f)
        {
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.1f, 0.9f);
            isClamped = true;
        }

        if (viewportPosition.y < 0.1f || viewportPosition.y > 0.9f)
        {
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.1f, 0.9f);
            isClamped = true;
        }

        if (isClamped)
        {
            // 자연스러운 곡선을 위한 Slerp 사용
            startPosition = Vector3.Slerp(startPosition, Camera.main.ViewportToWorldPoint(viewportPosition), Time.deltaTime * frequency);
        }

        return Camera.main.ViewportToWorldPoint(viewportPosition);
    }
}
