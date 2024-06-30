using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterAudioSource(AudioSource audioSource)
    {
        if (!audioSources.Contains(audioSource))
        {
            audioSources.Add(audioSource);
        }
    }

    public void SetPitch(float pitch)
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.pitch = pitch;
        }
    }
}
