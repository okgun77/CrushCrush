using UnityEngine;
using System.Collections;

public class DestroyFade : MonoBehaviour
{
    [SerializeField] private float lifetime = 3.5f; // 오브젝트가 유지되는 시간 (초)
    [SerializeField] private float fadeDuration = 0.5f; // 서서히 사라지는 시간 (초)
    [SerializeField] private GameObject destructionEffect; // 파괴 시 발생하는 효과

    private Renderer[] renderers;
    private Color[] originalColors;
    private bool isDestructionStarted = false;

    private void Start()
    {
        // Renderer 컴포넌트를 가져옵니다.
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        // 원래의 색상을 저장합니다.
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        // 지정된 시간 후 파괴 시작
        StartDestruction();
    }

    public void StartDestruction()
    {
        if (!isDestructionStarted)
        {
            isDestructionStarted = true;
            // 서서히 사라지기 시작할 시간을 계산합니다.
            float fadeStartTime = lifetime - fadeDuration;

            // fadeStartTime 이후에 서서히 사라지게 하고, lifetime 이후에 파괴합니다.
            Invoke(nameof(StartFading), fadeStartTime);
            Destroy(gameObject, lifetime); // 파괴 전 lifetime만큼 대기
        }
    }

    private void StartFading()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // 각 렌더러의 알파 값을 점진적으로 줄입니다.
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                {
                    Color newColor = originalColors[i];
                    newColor.a = alpha;
                    renderers[i].material.color = newColor;
                }
            }

            yield return null;
        }

        // 완전히 투명해진 후 파괴 효과를 생성합니다.
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, transform.rotation);
        }

        // 파괴가 끝났으니 오브젝트 삭제
        Destroy(gameObject);
    }
}
