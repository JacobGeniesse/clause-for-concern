using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Static singleton instance accessible from any script
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer Variables")]
    [Tooltip("Reference to the master audio mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Tooltip("Reference to the music channel of the selected audio mixer")]
    [SerializeField] private AudioSource musicSource;

    [Tooltip("Reference to the sfx channel of the selected audio mixer")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Slider Variables")]

    [Tooltip("Reference to the master volume slider.")]
    [SerializeField] private Slider masterSlider;

    [Tooltip("Reference to the music volume slider")]
    [SerializeField] private Slider musicSlider;

    [Tooltip("Reference to the sound effects volume slider")]
    [SerializeField] private Slider sfxSlider;

    // Add audioclips here
    [Header("Audio Clips")]
    [Tooltip("Background music to play during the scene")]
    public AudioClip backgroundMusic;

    [Header("Sound Object Variables")]
    [Tooltip("Prefab for the SoundObject")]
    [SerializeField] private GameObject soundObjectPrefab;

    [Tooltip("Transform for the container of the SoundObjects")]
    [SerializeField] private Transform soundObjectContainer;
    [HideInInspector] public List<GameObject> soundObjectPool = new List<GameObject>(); //List that serves as the SoundObject pool

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
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

        //If there are playerprefs to load, load them
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadVolume();
        }
        else
        {
            //If the player prefs don't exist set their values to the audio slider values
            SetMasterVolume(masterSlider.value);
            SetMusicVolume(musicSlider.value);
            SetSFXVolume(sfxSlider.value);
        }

        //If the ObjectPool exists create spare soundObjectPrefabs to deploy instantly.
        if(ObjectPool.instance != null)
        {
            WarmAudioPool();
        }
    }

    //Function for playing music in the scene
    public void PlayMusic(AudioClip clip)
    {
        // Don't restart if already playing
        if (musicSource.clip == clip) return;

        musicSource.clip = clip; //Define the music for the scene
        musicSource.loop = true; //Loop the music
        musicSource.Play(); //Play the music
    }

    //Function for playing a sound effect without 3D sound
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume); //Play the audio clip if the audio clip exists
        }
    }

    //Function for playing a sound effect with 3D sound
    public void Play3DSFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        //Check that the audio clip supplied exists
        if (clip != null)
        {
            GameObject soundObject = ObjectPool.instance.Create(soundObjectPrefab, position, Quaternion.identity, soundObjectPool); //Pull a sound object from the object pool and place it where the call describes
            
            if(soundObject.transform.parent != soundObjectContainer)
            {
                soundObject.transform.parent = soundObjectContainer; //Parent the sound object to the soundObjectContainer, in case a new sound object needed to be created
            }

            if (soundObject.TryGetComponent<SoundObject>(out SoundObject soundObj))
            {
                soundObj.PlayClip(clip); //If the new sound object has the sound object component created, trigger its behavior to play its audio clip
            }
            else
            {
                Debug.LogWarning("Supplied soundObjectPrefab prefab has no SoundObject script attached to it! Cannot play sound!");
            }
        }
    }

    //Function for loading the saved information partaining to volume settings
    private void LoadVolume()
    {
        //Grabbed saved information
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume"); 

        if(masterSlider != null)
        {
            SetMasterVolume(masterSlider.value); //Set the master volume slider's value
        }

        if(musicSlider != null)
        {
            SetMusicVolume(musicSlider.value); //Set the music slider's value 
        }

        if(sfxSlider != null)
        {
            SetSFXVolume(sfxSlider.value); //Set the SFXVolume slider's value
        }
    }

    //Function for setting the master volume slider's volume
    public void SetMasterVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(sliderValue) * 20; //Scale the slider value to better match the channel's logarithmic sound scale
        audioMixer.SetFloat("MasterVol",dbValue); //Set the value of the master volume slider
        PlayerPrefs.SetFloat("masterVolume", sliderValue); //Set that info to the settings data
    }

    //Function for setting the music audio channel volume
    public void SetMusicVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(sliderValue) * 20; //Scale the slider value to better match the channel's logarithmic sound scale
        audioMixer.SetFloat("MusicVol", dbValue); //Set the value of the music volume slider
        PlayerPrefs.SetFloat("musicVolume", sliderValue); //Set that info to the settings data
    }

    //Function for setting the sound effects audio channel volume
    public void SetSFXVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(sliderValue) * 20; //Scale the slider value to better match the channel's logarithmic sound scale
        audioMixer.SetFloat("SFXVol", dbValue); //Set the value of the SFX slider
        PlayerPrefs.SetFloat("SFXVolume", sliderValue); //Save that info to the settings data
    }

    //Function to set up audio objects in advance to sit in the object pool for deployment
    private void WarmAudioPool()
    {
        for(int i = 0; i < 10; i++)
        {
            GameObject freshMeat = ObjectPool.instance.Create(soundObjectPrefab, transform.position, Quaternion.identity); //Create the warm up object
            freshMeat.transform.parent = soundObjectContainer; //Parent that object to the soundObjectContainer for storage
            ObjectPool.instance.Destroy(freshMeat, soundObjectPool); //Return this soundobject to the audio manager's pool
        }
    }
}
