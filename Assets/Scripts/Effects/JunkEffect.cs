using UnityEngine;

public class JunkEffect : MonoBehaviour
{
    private JunkEffectSettings settings;
    private Renderer[] renderers;
    private Material[] materials;
    private float fadeTimer = 0f;
    private bool isFading = true;
    
    private Vector3 startPosition;
    private float randomOffset;
    private float rotationSpeed;
    private float floatAmplitude;
    private float floatFrequency;
    
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    
    public void Initialize(JunkEffectSettings _settings)
    {
        settings = _settings;
        SetupMaterials();
        InitializeEffects();
    }
    
    private void SetupMaterials()
    {
        renderers = GetComponentsInChildren<Renderer>();
        var materialList = new System.Collections.Generic.List<Material>();
        
        foreach (var renderer in renderers)
        {
            materialList.AddRange(renderer.materials);
        }
        materials = materialList.ToArray();
        
        foreach (Material material in materials)
        {
            if (material.HasProperty(BaseColorProperty))
            {
                SetupTransparentMaterial(material);
                SetAlpha(material, 0f);
            }
        }
    }
    
    private void SetupTransparentMaterial(Material material)
    {
        material.SetInt("_Surface", 1);
        material.SetInt("_Blend", 0);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
    
    private void InitializeEffects()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
        
        // 랜덤 효과 값 설정
        rotationSpeed = Random.Range(settings.rotationSpeedRange.x, settings.rotationSpeedRange.y);
        floatAmplitude = Random.Range(settings.floatAmplitudeRange.x, settings.floatAmplitudeRange.y);
        floatFrequency = Random.Range(settings.floatFrequencyRange.x, settings.floatFrequencyRange.y);
    }
    
    private void Update()
    {
        if (isFading)
        {
            UpdateFade();
        }
        UpdateFloating();
        UpdateRotation();
    }
    
    private void UpdateFade()
    {
        fadeTimer += Time.deltaTime;
        float normalizedTime = fadeTimer / settings.fadeInDuration;
        
        if (normalizedTime <= 1f)
        {
            float alpha = Mathf.Lerp(0f, 1f, normalizedTime);
            SetAlphaAll(alpha);
        }
        else
        {
            SetAlphaAll(1f);
            isFading = false;
        }
    }
    
    private void UpdateFloating()
    {
        float newY = startPosition.y + floatAmplitude * Mathf.Sin((Time.time + randomOffset) * floatFrequency);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    private void UpdateRotation()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    
    private void SetAlpha(Material material, float alpha)
    {
        if (material.HasProperty(BaseColorProperty))
        {
            Color color = material.GetColor(BaseColorProperty);
            color.a = alpha;
            material.SetColor(BaseColorProperty, color);
        }
    }
    
    private void SetAlphaAll(float alpha)
    {
        if (materials == null) return;
        
        foreach (Material material in materials)
        {
            if (material != null)
            {
                SetAlpha(material, alpha);
            }
        }
    }
    
    private void OnDestroy()
    {
        if (materials != null)
        {
            foreach (var material in materials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }
    }
} 