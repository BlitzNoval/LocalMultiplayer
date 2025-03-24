// Title: Do Not Destroy 
// Author: ChatGPT
// Date: 24 March  2025
// Do not destory didnt even work so the refrence is to say AI sucks 

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Settings")]
    [SerializeField] private AudioClip initialSong;
    [SerializeField] private List<AudioClip> songs = new List<AudioClip>();
    
    private AudioSource audioSource;
    private string currentSceneName;
    private bool initialSongPlayed = false;
    private bool isApplicationPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadVolumeSettings();
            PlayNextSong(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isApplicationPaused = pauseStatus;
    }

    void Update()
    {
        if (!isApplicationPaused && !audioSource.isPlaying)
        {
            if (audioSource.clip == null || audioSource.time >= audioSource.clip.length - 0.1f)
            {
                PlayNextSong();
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        ApplySceneVolume();
    }

    void LoadVolumeSettings()
    {
        if (!PlayerPrefs.HasKey(currentSceneName))
        {
            PlayerPrefs.SetFloat(currentSceneName, 1.0f);
        }
        ApplySceneVolume();
    }

    void ApplySceneVolume()
    {
        if (PlayerPrefs.HasKey(currentSceneName))
        {
            audioSource.volume = PlayerPrefs.GetFloat(currentSceneName);
        }
    }

    void PlayNextSong(bool forceInitial = false)
    {
        if (forceInitial && initialSong != null)
        {
            audioSource.clip = initialSong;
            audioSource.Play();
            initialSongPlayed = true;
            return;
        }

        if (!initialSongPlayed && initialSong != null)
        {
            audioSource.clip = initialSong;
            audioSource.Play();
            initialSongPlayed = true;
        }
        else if (songs.Count > 0)
        {
            int randomIndex = Random.Range(0, songs.Count);
            audioSource.clip = songs[randomIndex];
            audioSource.Play();
        }
    }

    public void SaveVolumeSetting(float volume)
    {
        PlayerPrefs.SetFloat(currentSceneName, volume);
        audioSource.volume = volume;
    }

    public void SetSongs(List<AudioClip> newSongs)
    {
        songs = newSongs;
        PlayNextSong();
    }
}