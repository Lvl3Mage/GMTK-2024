using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance { get; private set; }

    [Header("--------- Arrays Of Sounds ---------")]
    public AudioClip[] ambientSounds;
    public AudioClip[] effectSounds;
    public AudioClip[] musicTracks;

    [Header("--------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip dog;
    public AudioClip gong;
    public AudioClip rope;

    [Header("--------- Audio Source ---------")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    private void Awake()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        if (instance != null)
        {
            Debug.LogWarning("Another instance of WorldGrid exists! Destroying this one.");
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Evita que este objeto se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    // M�todo para reproducir un sonido de efecto aleatorio
    public void PlayEffectSound()
    {
        if (effectSounds.Length == 0)
        {
            Debug.LogWarning("Cannot play sound. Sound array is empty.");
            return;
        }

        int randomIndex = Random.Range(0, effectSounds.Length);
        sfxSource.PlayOneShot(effectSounds[randomIndex]);
    }

    public void PlayDog()
    {
        sfxSource.PlayOneShot(dog);
    }

    public void PlayGong()
    {
        sfxSource.PlayOneShot(gong);
    }

    public void PlayRope()
    {
        sfxSource.PlayOneShot(rope);
    }

    public void PlayRandomSound(AudioClip[] soundArray)
    {
        if (soundArray.Length == 0)
        {
            Debug.LogWarning("Cannot play sound. Sound array is empty.");
            return;
        }

        int randomIndex = Random.Range(0, soundArray.Length);
        sfxSource.PlayOneShot(soundArray[randomIndex]);
    }
}
