using UnityEngine;

public class CrushSoundManager : MonoBehaviour
{
    public static CrushSoundManager Instance;
    public AudioSource audioSource;
    public AudioClip destructionSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayDestructionSound(Vector3 position)
    {
        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, position);
        }
    }
}