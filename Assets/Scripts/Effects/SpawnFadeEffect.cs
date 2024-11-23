using UnityEngine;
using System.Collections;

public class SpawnFadeEffect : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scaleDuration = 0.4f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Renderer[] renderers;
    private Color[] originalColors;
    private Vector3 targetScale;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        originalColors = new Color[renderers.Length];
        targetScale = transform.localScale;

        // 원래 색상 저장
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                originalColors[i] = renderers[i].material.color;
            }
        }
    }

    private void OnDisable()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // 비활성화될 때 원래 상태로 복구
        transform.localScale = targetScale;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }

    public void StartEffect()
    {
        // 이전 효과가 실행 중이면 중지
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 초기 상태 설정
        transform.localScale = Vector3.zero;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                Color startColor = originalColors[i];
                startColor.a = 0f;
                renderers[i].material.color = startColor;
            }
        }

        // 효과 시작
        fadeCoroutine = StartCoroutine(FadeInWithScale());
    }

    private IEnumerator FadeInWithScale()
    {
        float elapsedTime = 0f;
        float maxDuration = Mathf.Max(fadeDuration, scaleDuration);

        while (elapsedTime < maxDuration)
        {
            elapsedTime += Time.deltaTime;

            // 페이드 인 효과
            if (elapsedTime <= fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null && renderers[i].material != null)
                    {
                        Color newColor = originalColors[i];
                        newColor.a = alpha;
                        renderers[i].material.color = newColor;
                    }
                }
            }

            // 스케일 효과
            if (elapsedTime <= scaleDuration)
            {
                float scaleProgress = elapsedTime / scaleDuration;
                float curveValue = scaleCurve.Evaluate(scaleProgress);
                transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, curveValue);
            }

            yield return null;
        }

        // 최종 상태로 설정
        transform.localScale = targetScale;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                renderers[i].material.color = originalColors[i];
            }
        }

        fadeCoroutine = null;
    }
}
