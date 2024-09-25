using UnityEngine;
using System.Collections;

public class FadeInObject : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 3f; // 서서히 나타나는 시간
    private Renderer[] renderers;
    private Color[] originalColors;

    private void Start()
    {
        // Renderer 컴포넌트를 가져옵니다.
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        // 처음에 알파값이 0인 상태로 설정
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                // 원래 색상을 저장
                originalColors[i] = renderers[i].material.color;

                // 투명한 상태로 만들기 위해 쉐이더 설정
                SetMaterialToTransparent(renderers[i].material);

                // 처음에 완전히 투명하게 만듦
                Color transparentColor = originalColors[i];
                transparentColor.a = 0f;
                renderers[i].material.color = transparentColor;
            }
        }
    }

    // 쉐이더가 알파를 지원하지 않으면 알파 블렌딩이 가능한 Transparent 쉐이더로 변경
    private void SetMaterialToTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1); // Transparent 설정
        mat.SetFloat("_Blend", 1); // Alpha Blend 설정
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0); // ZWrite 비활성화
        mat.DisableKeyword("_ALPHATEST_ON"); // Alpha Test 비활성화
        mat.EnableKeyword("_ALPHABLEND_ON"); // Alpha Blend 활성화
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // Alpha Premultiply 비활성화
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent; // 렌더 큐를 Transparent로 설정
    }

    // 페이드인 시작
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); // 알파값을 0에서 1로 서서히 증가

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                {
                    Color newColor = originalColors[i];
                    newColor.a = alpha; // 알파값을 서서히 증가
                    renderers[i].material.color = newColor;
                }
            }

            yield return null;
        }

        // 완전한 상태로 설정
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                Color finalColor = originalColors[i];
                finalColor.a = 1f;
                renderers[i].material.color = finalColor;
            }
        }

        // 페이드인이 완료되면, 다시 불투명한 상태로 변경할 수 있음
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                SetMaterialToOpaque(renderers[i].material); // 불투명으로 변경
            }
        }
    }

    // 페이드인이 완료되면 불투명으로 변경
    private void SetMaterialToOpaque(Material mat)
    {
        mat.SetFloat("_Surface", 0); // Opaque 설정
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1); // ZWrite 활성화
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1; // 기본 렌더 큐로 복구
    }

}
