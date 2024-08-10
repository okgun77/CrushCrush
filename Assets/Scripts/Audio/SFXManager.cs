using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> sfxClips;      // SFX 클립 리스트
    [SerializeField] private AudioSource sfxSources;        // SFX를 재생할 AudioSource;

    private void Awake()
    {
        if (sfxSources == null)
        {
            sfxSources = gameObject.AddComponent<AudioSource>();
            sfxSources.loop = false;    // sfx 기본 반복 안됨(반복될 일이 있나??)
        }
    }

    public void PlaySFX(string _sfxName)
    {
        AudioClip clip = sfxClips.Find(s => s.name == _sfxName);
        
        if(clip != null)
        {
            sfxSources.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{_sfxName}' not found!");
        }
    }

    public void PlaySFxAtPoint(string _sfxName, Vector3 _position)
    {
        AudioClip clip = sfxClips.Find(s => s.name == _sfxName);

        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, _position);
        }
        else
        {
            Debug.LogWarning($"SFX '{_sfxName}' not found!");
        }
    }

}
