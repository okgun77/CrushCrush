using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class JunkFadeEffect : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    
    private MeshRenderer meshRenderer;
    private Material[] materials;
    private float currentAlpha = 0f;
    private bool isFading = false;
    private bool fadeIn = true;
    
    public static JunkFadeEffect CreateEffect(GameObject target, JunkFadeEffect settings)
    {
        if (settings == null)
        {
            Debug.LogError("JunkFadeEffect settings not provided!");
            return null;
        }

        var effect = target.AddComponent<JunkFadeEffect>();
        effect.fadeInDuration = settings.fadeInDuration;
        effect.fadeOutDuration = settings.fadeOutDuration;
        
        return effect;
    }
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
        
        foreach (Material material in materials)
        {
            if (material.HasProperty("_Color"))
            {
                material.SetFloat("_Surface", 1);
                material.SetFloat("_Blend", 1);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
        }
        
        SetAlpha(0f);
    }
    
    private void OnEnable()
    {
        StartFade(true);
    }
    
    private void Update()
    {
        if (!isFading) return;
        
        float fadeSpeed = fadeIn ? (1f / fadeInDuration) : (-1f / fadeOutDuration);
        currentAlpha += Time.deltaTime * fadeSpeed;
        
        if (fadeIn && currentAlpha >= 1f)
        {
            currentAlpha = 1f;
            isFading = false;
        }
        else if (!fadeIn && currentAlpha <= 0f)
        {
            currentAlpha = 0f;
            isFading = false;
        }
        
        SetAlpha(currentAlpha);
    }
    
    public void StartFade(bool fadeIn)
    {
        this.fadeIn = fadeIn;
        this.isFading = true;
        currentAlpha = fadeIn ? 0f : 1f;
    }
    
    private void SetAlpha(float alpha)
    {
        foreach (Material material in materials)
        {
            if (material.HasProperty("_Color"))
            {
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }
    }
} 