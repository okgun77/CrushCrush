using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private BGMManager bgmManager; // BGM 관리 매니저
    [SerializeField] private SFXManager sfxManager; // SFX 관리 매니저

    public void Init(GameManager _gameManager)
    {
        bgmManager.Init(this);
        sfxManager.Init(this);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 게임 씬 간의 오디오매니저 유지
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    public void PlayBGM(string _trackName)
    {
        if (bgmManager != null)
        {
            bgmManager.PlayTrack(_trackName);
        }
    }

    public void StopBGM()
    {
        if (bgmManager != null)
        {
            bgmManager.StopTrack();
        }
    }

    public void PlaySFX(string _sfxName)
    {
        if (sfxManager != null)
        {
            sfxManager.PlaySFX(_sfxName);
        }
    }

    public void SetVolume(float _volume)
    {
        AudioListener.volume = _volume;
    }

    public void Mute(bool _isMuted)
    {
        AudioListener.pause = _isMuted;
    }
}
