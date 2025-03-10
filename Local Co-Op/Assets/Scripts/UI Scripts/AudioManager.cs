using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private List<AudioClip> songs = new List<AudioClip>();
    private AudioSource audioSource;
    private string currentSceneName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadVolumeSettings();
            PlayNextSong();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        ApplySceneVolume();
    }

    private void LoadVolumeSettings()
    {
        if (!PlayerPrefs.HasKey(currentSceneName))
        {
            PlayerPrefs.SetFloat(currentSceneName, 1.0f);  // Default volume if not set
        }
        ApplySceneVolume();
    }

    private void ApplySceneVolume()
    {
        if (PlayerPrefs.HasKey(currentSceneName))
        {
            audioSource.volume = PlayerPrefs.GetFloat(currentSceneName);
        }
    }

    public void SaveVolumeSetting(float volume)
    {
        PlayerPrefs.SetFloat(currentSceneName, volume);
        audioSource.volume = volume;
    }

    void PlayNextSong()
    {
        if (songs.Count == 0) return;

        if (!audioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, songs.Count);
            audioSource.clip = songs[randomIndex];
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextSong();
        }

        // Check if the "J" key is pressed to reset volume settings
        if (Input.GetKeyDown(KeyCode.J))
        {
            ResetVolumeSettings();
        }
    }

    public void SetSongs(List<AudioClip> newSongs)
    {
        songs = newSongs;
        PlayNextSong();
    }

    // Method to reset the volume settings for the current scene
    public void ResetVolumeSettings()
    {
        if (PlayerPrefs.HasKey(currentSceneName))
        {
            PlayerPrefs.DeleteKey(currentSceneName);  // Reset the saved volume for this scene
            ApplySceneVolume();  // Apply the default volume (which is 1.0f)
        }
    }
}
