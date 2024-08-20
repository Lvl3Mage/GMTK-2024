using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance { get; private set; }

    [Header("--------- Arrays Of Sounds ---------")]
    public AudioClip[] ambientSounds;
    public AudioClip[] effectSounds;
    public AudioClip[] musicTracks;
    public AudioClip[] musicKeys; 

    [Header("--------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip gamePlayMusic;
    public AudioClip dog;
    public AudioClip gong;
    public AudioClip rope;
    public AudioClip destroyingBush;

    [Header("--------- Audio Source ---------")]
    public AudioSource sfxSource;
    public AudioSource ambienceMusic;
    public AudioSource musicSource;

    int keysIndex = 0;

    private void Awake()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        if (ambienceMusic == null)
        {
            ambienceMusic = gameObject.AddComponent<AudioSource>();
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
        ambienceMusic.clip = background;
        ambienceMusic.loop = true;
        ambienceMusic.Play();
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

    public void PlayMusicKeys()
    {
        if (musicKeys.Length == 0)
        {
            Debug.LogWarning("Cannot play sound. Sound array is empty.");
            return;
        }

        sfxSource.PlayOneShot(musicKeys[keysIndex++]);
        if(keysIndex >= musicKeys.Length)
        {
            keysIndex = 0;
        }
    }

    public void PlayGameplayMusic()
    {
        musicSource.clip = gamePlayMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayDestoyingBush()
    {
        sfxSource.PlayOneShot(destroyingBush);
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
