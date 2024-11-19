using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FlyInEffect : MonoBehaviour
{
    [SerializeField] private RectTransform uiElement; // UI 요소
    [SerializeField] private Vector2 startPos = new Vector2(-500, 0); // 시작 위치 (화면 밖)
    [SerializeField] private Vector2 endPos = new Vector2(0, 0); // 도착할 위치 (화면 안)
    [SerializeField] private float duration = 1.0f; // 애니메이션 시간

    private void Start()
    {
        // 시작 위치로 이동시킴
        uiElement.anchoredPosition = startPos;

        // 도착할 위치로 텐션 있게 날아옴
        uiElement.DOAnchorPos(endPos, duration)
            .SetEase(Ease.OutBack); // 날아올 때 텐션 있게 설정
    }
}
