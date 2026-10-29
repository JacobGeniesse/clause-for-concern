using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Static singleton instance accessible from any script
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Add audiclips here
    public AudioClip backgroundMusic;
    //public AudioClip clip1;
    //public AudioClip clip2;
    //public AudioClip clip3;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scene loads
        }
        else
        {
            Destroy(gameObject);    // Destroy duplicates
        }

        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Play the initial background music on start
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }

        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume(masterSlider.value);
            SetMusicVolume(musicSlider.value);
            SetSFXVolume(sfxSlider.value);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        // Don't restart if already playing
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void Play3DSFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat("MasterVol",dbValue);
        PlayerPrefs.SetFloat("masterVolume", sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat("MusicVol", dbValue);
        PlayerPrefs.SetFloat("musicVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat("SFXVol", dbValue);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }
}
