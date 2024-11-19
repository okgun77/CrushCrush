using DG.Tweening;
using UnityEngine;

public class JellyEffect : MonoBehaviour
{
    [SerializeField] private float scaleAmount = 1.2f; // 커질 크기
    [SerializeField] private float duration = 0.5f; // 애니메이션 시간

    private void Start()
    {
        // 크기를 반복적으로 커졌다 작아졌다 하도록 설정
        transform.DOScale(scaleAmount, duration)
            .SetLoops(-1, LoopType.Yoyo) // 무한 반복, Yoyo는 커졌다가 다시 작아짐
            .SetEase(Ease.InOutElastic); // 텐션 있게 설정
    }
}
