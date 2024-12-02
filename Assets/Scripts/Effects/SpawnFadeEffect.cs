using UnityEngine;
using System.Collections;

public class SpawnFadeEffect : MonoBehaviour
{
    private float fadeDuration = 4.0f;
    private float scaleDuration = 4.0f;
    private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Renderer[] renderers;
    private Color[] originalColors;
    private Vector3 targetScale;
    private Coroutine fadeCoroutine;

    // 스폰 시 페이드 인 효과를 적용하는 정적 메서드
    public static GameObject SpawnWithFadeEffect(GameObject prefab, Vector3 position, Quaternion rotation, float fadeDuration = 0.5f, float scaleDuration = 0.4f)
    {
        GameObject spawnedObject = Instantiate(prefab, position, rotation);
        SpawnFadeEffect fadeEffect = spawnedObject.AddComponent<SpawnFadeEffect>();
        fadeEffect.fadeDuration = fadeDuration;
        fadeEffect.scaleDuration = scaleDuration;
        fadeEffect.StartEffect();
        return spawnedObject;
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        originalColors = new Color[renderers.Length];
        targetScale = transform.localScale;

        // 원래 색상 저장 및 머티리얼 설정
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                Material mat = renderers[i].material;
                
                // 홀로그램 셰이더인지 확인
                if (mat.shader.name.Contains("Hologram"))
                {
                    // 홀로그램 셰이더의 경우 다른 처리
                    continue;
                }

                // 일반 셰이더의 경우
                mat.SetFloat("_Mode", 2); // Fade 모드로 설정
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                
                if (mat.HasProperty("_Color"))
                {
                    originalColors[i] = mat.color;
                }
                else
                {
                    originalColors[i] = Color.clear; // 변경하지 않도록 Color.clear 사용
                }
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
                        if (originalColors[i] != Color.clear) // Color.clear가 아닌 경우에만 변경
                        {
                            Color newColor = originalColors[i];
                            newColor.a = alpha;
                            renderers[i].material.color = newColor;
                        }
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
                if (originalColors[i] != Color.clear) // Color.clear가 아닌 경우에만 복원
                {
                    renderers[i].material.color = originalColors[i];
                }
            }
        }

        fadeCoroutine = null;
        
        // 효과가 완료되면 컴포넌트 제거
        Destroy(this);
    }
}
