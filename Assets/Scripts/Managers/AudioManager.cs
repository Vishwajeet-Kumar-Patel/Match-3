using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Sound Effects")]
    public AudioClip swapSound;
    public AudioClip matchSound;
    public AudioClip invalidMoveSound;
    public AudioClip comboSound;
    public AudioClip gameOverSound;
    
    [Header("Music")]
    public AudioClip backgroundMusic;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudio()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    public void PlaySwapSound()
    {
        PlaySFX(swapSound);
    }
    
    public void PlayMatchSound()
    {
        PlaySFX(matchSound);
    }
    
    public void PlayInvalidMoveSound()
    {
        PlaySFX(invalidMoveSound);
    }
    
    public void PlayComboSound()
    {
        PlaySFX(comboSound);
    }
    
    public void PlayGameOverSound()
    {
        PlaySFX(gameOverSound);
    }
    
    void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            if (musicSource.isPlaying)
                musicSource.Pause();
            else
                musicSource.UnPause();
        }
    }
}
