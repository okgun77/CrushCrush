using UnityEngine;

public class JunkEffectManager : MonoBehaviour
{
    [SerializeField] private JunkEffectSettings settings;

    public void ApplyEffectToJunk(GameObject junkObject)
    {
        var effect = junkObject.AddComponent<JunkEffect>();
        effect.Initialize(settings);
    }

    // 런타임에서 설정 변경이 필요한 경우
    public void UpdateSettings(JunkEffectSettings newSettings)
    {
        settings = newSettings;
    }
} 