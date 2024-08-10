using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private BGMManager bgmManager;
    [SerializeField] private SFXManager sfxManager;

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

    public void PlayBGM(string _trackname)
    {
        if (bgmManager != null)
        {
            bgmManager.PlayTrack(_trackname);
        }
    }


}
