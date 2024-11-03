using UnityEngine;
using DG.Tweening;

/// <summary>
/// UI 요소에 펀치 스케일 효과를 적용하는 컴포넌트
/// </summary>
public class IconPunchEffect : MonoBehaviour
{
    [Header("Punch Effect Settings")]
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private int vibrato = 1;
    [SerializeField] private float elasticity = 0.5f;

    private Tween currentTween;

    /// <summary>
    /// 펀치 효과 재생
    /// </summary>
    public void PlayEffect()
    {
        // 이미 재생 중인 효과가 있다면 완료
        if (currentTween != null && currentTween.IsPlaying())
        {
            currentTween.Complete();
        }

        // 새로운 펀치 효과 시작
        currentTween = transform
            .DOPunchScale(Vector3.one * (punchScale - 1f), duration, vibrato, elasticity)
            .SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
} 