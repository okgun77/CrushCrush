using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> bgmTracks;     // BGM Track List
    [SerializeField] private AudioSource bgmSource;         // BGM을 재생할 AudioSource

    public void Init(AudioManager _audioManager)
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;  // BGM 기본 반복재생
        }
    }

    public void PlayTrack(string _trackName)
    {
        AudioClip track = bgmTracks.Find(t => t.name == _trackName);
        if (track != null)
        {
            bgmSource.clip = track;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM track '{_trackName}' not found!");
        }
    }

    public void StopTrack()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void FadeIn(float _duration)
    {
        StartCoroutine(FadeInCoroutine(_duration));
    }

    public void FadeOut(float _duration)
    {
        StopCoroutine(FadeOutCoroutine(_duration));
    }


    private IEnumerator FadeInCoroutine(float _duration)
    {
        float startVolume = 0f;
        bgmSource.volume = startVolume;
        bgmSource.Play();

        while (bgmSource.volume < 1f)
        {
            bgmSource.volume += Time.deltaTime / _duration;
            yield return null;
        }

        bgmSource.volume = 1f;
    }

    private IEnumerator FadeOutCoroutine(float _duration)
    {
        float startVolume = bgmSource.volume;

        while (bgmSource.volume > 0f)
        {
            bgmSource.volume -= startVolume * Time.deltaTime / _duration;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }
}
