using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.red;
    [SerializeField] private float outlineWidth = 5f;
    [SerializeField] private float defaultWidth = 5f;

    [Header("Blink Settings")]
    [SerializeField] private bool isBlinking = false;
    [SerializeField] private float blinkInterval = 0.5f;
    
    private Material outlineMaterial;
    private float nextBlinkTime = 0f;
    private bool initialized = false;

    private void OnEnable()
    {
        if (!initialized)
        {
            InitializeOutline();
        }
    }

    private void InitializeOutline() 
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader outlineShader = Shader.Find("Universal Render Pipeline/Lit");
            if (outlineShader == null)
            {
                Debug.LogError("Failed to find URP Lit shader!");
                return;
            }
            
            outlineMaterial = new Material(outlineShader);
            Material[] materials = renderer.sharedMaterials;
            System.Array.Resize(ref materials, materials.Length + 1);
            materials[materials.Length - 1] = outlineMaterial;
            renderer.materials = materials;

            SetOutlineWidth(outlineWidth);
            SetOutlineColor(outlineColor);
            
            initialized = true;
            Debug.Log($"OutlineEffect initialized for {gameObject.name}");
        }
        else
        {
            Debug.LogError($"No Renderer found on {gameObject.name}");
        }
    }

    public void SetOutlineColor(Color color)
    {
        if (outlineMaterial != null)
        {
            outlineColor = color;
            outlineMaterial.SetColor("_OutlineColor", color);
        }
    }

    public void SetOutlineWidth(float width)
    {
        if (outlineMaterial != null)
        {
            outlineWidth = width;
            defaultWidth = width;
            outlineMaterial.SetFloat("_OutlineWidth", width);
        }
    }

    public void EnableBlinking(bool enable)
    {
        isBlinking = enable;
        if (!enable)
        {
            SetOutlineWidth(defaultWidth);
        }
        nextBlinkTime = Time.time;
    }

    private void Update()
    {
        if (isBlinking && Time.time >= nextBlinkTime)
        {
            ToggleOutline();
            nextBlinkTime = Time.time + blinkInterval;
        }
    }

    private void ToggleOutline()
    {
        if (outlineMaterial != null)
        {
            float currentWidth = outlineMaterial.GetFloat("_OutlineWidth");
            outlineMaterial.SetFloat("_OutlineWidth", currentWidth > 0 ? 0 : defaultWidth);
        }
    }
}