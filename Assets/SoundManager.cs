using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound) { 
        audioSource.PlayOneShot(sound);
    }
}
