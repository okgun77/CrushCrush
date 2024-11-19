using DG.Tweening;
using UnityEngine;

public class RotationEffect : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 15f; // 회전할 각도
    [SerializeField] private float duration = 0.5f; // 애니메이션 시간

    private void Start()
    {
        // 왼쪽으로 회전한 후 다시 원위치로 돌아옴
        transform.DORotate(new Vector3(0, 0, rotationAngle), duration)
            .SetLoops(-1, LoopType.Yoyo) // 무한 반복
            .SetEase(Ease.InOutElastic); // 텐션 효과
    }
}
