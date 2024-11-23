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
        // 이전 Tween이 있다면 Kill하고 원래 크기로 즉시 복원
        if (currentTween != null)
        {
            currentTween.Kill(true);  // true를 전달하여 즉시 완료 상태로 만듦
            currentTween = null;
            transform.localScale = Vector3.one;  // 기본 크기로 리셋
        }

        // 새로운 펀치 효과 시작
        if (this != null && gameObject != null && gameObject.activeInHierarchy)
        {
            transform.localScale = Vector3.one;  // 시작 전 크기 리셋
            
            // punchScale이 1.2라면 0.2만큼만 증가하도록 수정
            Vector3 punchAmount = Vector3.one * (punchScale - 1f);
            
            currentTween = transform
                .DOPunchScale(punchAmount, duration, vibrato, elasticity)
                .SetEase(Ease.OutQuad);
        }
    }       

    private void OnDestroy()
    {
        if (currentTween != null)
        {
            currentTween.Kill(true);
            transform.localScale = Vector3.one;
        }
    }
}