using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private float spreadRange = 0.5f;
    [SerializeField] private float bounceStrength = 0.3f;
    
    // 크기 관련 변수
    [SerializeField] private float minFontSize = 5f;
    [SerializeField] private float maxFontSize = 8f;
    
    // 색상 관련 변수
    [SerializeField] private Color lowDamageColor = new Color(1f, 1f, 1f); // 흰색
    [SerializeField] private Color mediumDamageColor = new Color(1f, 0.8f, 0.2f); // 노란색
    [SerializeField] private Color highDamageColor = new Color(1f, 0.2f, 0.2f); // 빨간색
    
    // 데미지 임계값
    [SerializeField] private float mediumDamageThreshold = 20f;
    [SerializeField] private float highDamageThreshold = 50f;

    public void Setup(float damage)
    {
        damageText.text = damage.ToString("F0");
        
        // 데미지에 따른 색상 설정
        Color textColor;
        float sizeFactor;
        
        if (damage >= highDamageThreshold)
        {
            textColor = highDamageColor;
            sizeFactor = 1f;
        }
        else if (damage >= mediumDamageThreshold)
        {
            textColor = mediumDamageColor;
            sizeFactor = 0.8f;
        }
        else
        {
            textColor = lowDamageColor;
            sizeFactor = 0.6f;
        }
        
        // 약간의 랜덤성 추가
        textColor = Color.Lerp(textColor, Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f), 0.2f);
        damageText.color = textColor;
        
        // 랜덤 폰트 크기 설정 (데미지 크기에 비례)
        float randomSize = Random.Range(minFontSize, maxFontSize) * sizeFactor;
        damageText.fontSize = randomSize;
        
        transform.localScale = Vector3.zero;
        
        float randomX = Random.Range(-spreadRange, spreadRange);
        Vector3 targetPosition = transform.position + new Vector3(randomX, 1f, 0);
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(transform.DOScale(1.2f, 0.2f));
        sequence.Append(transform.DOScale(1f, 0.1f));
        
        sequence.Join(transform.DOMove(targetPosition, floatSpeed)
            .SetEase(Ease.OutQuad));
        
        float randomRotation = Random.Range(-30f, 30f);
        sequence.Join(transform.DORotate(new Vector3(0, 0, randomRotation), floatSpeed));
        
        sequence.Join(transform.DOScale(Vector3.one * 1.1f, floatSpeed * 0.5f)
            .SetLoops(2, LoopType.Yoyo));
        
        sequence.Join(damageText.DOFade(0, fadeSpeed)
            .SetEase(Ease.InQuad));
        
        Destroy(gameObject, destroyTime);
    }
}