using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct SFXClip
{
    public string name;      // 사용자가 설정할 SFX 이름
    public AudioClip clip;   // 실제 오디오 클립
}

public class SFXManager : MonoBehaviour
{
    [SerializeField] private List<SFXClip> sfxClips; // SFX 클립 리스트
    [SerializeField] private AudioSource sfxSource;  // SFX를 재생할 AudioSource

    private Dictionary<string, AudioClip> sfxDictionary;

    public void Init(AudioManager _audioManager)
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false; // SFX는 반복되지 않음(반복할 일이 있나??)
        }

        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (SFXClip sfx in sfxClips)
        {
            if (!sfxDictionary.ContainsKey(sfx.name))
            {
                sfxDictionary.Add(sfx.name, sfx.clip);
            }
        }
    }

    public void PlaySFX(string _sfxName)
    {
        if (sfxDictionary.TryGetValue(_sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{_sfxName}' not found!");
        }
    }

    public void PlaySFXAtPoint(string _sfxName, Vector3 _position)
    {
        if (sfxDictionary.TryGetValue(_sfxName, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, _position);
        }
        else
        {
            Debug.LogWarning($"SFX '{_sfxName}' not found!");
        }
    }
}
