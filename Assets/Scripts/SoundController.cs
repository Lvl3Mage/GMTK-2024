using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("--------- Arrays Of Sounds ---------")]
    public AudioClip[] ambientSounds;
    public AudioClip[] effectSounds;
    public AudioClip[] musicTracks;

    [Header("--------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip dog;

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

        // Evita que este objeto se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Método para reproducir un sonido de efecto aleatorio
    public void PlayEffectSound()
    {
        if (effectSounds.Length == 0)
        {
            Debug.LogWarning("El array de sonidos está vacío. No se puede reproducir ningún sonido.");
            return;
        }

        int randomIndex = Random.Range(0, effectSounds.Length);
        sfxSource.PlayOneShot(effectSounds[randomIndex]);
    }

    public void PlayDog()
    {
        sfxSource.PlayOneShot(dog);
    }

    // Método genérico para reproducir un sonido aleatorio de cualquier array
    public void PlayRandomSound(AudioClip[] soundArray)
    {
        if (soundArray.Length == 0)
        {
            Debug.LogWarning("El array de sonidos está vacío. No se puede reproducir ningún sonido.");
            return;
        }

        int randomIndex = Random.Range(0, soundArray.Length);
        sfxSource.PlayOneShot(soundArray[randomIndex]);
    }
}
