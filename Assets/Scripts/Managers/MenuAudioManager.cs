using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MenuAudioManager : MonoBehaviour
{
    public static MenuAudioManager instance;
    public AudioSource menuMusicSource;
    public List<string> gameLevelSceneNames;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Start()
    {
        LoadVolume();
        if (!menuMusicSource.isPlaying)
        {
            menuMusicSource.loop = true;
            menuMusicSource.Play();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameLevelSceneNames.Contains(scene.name))
        {
            Destroy(gameObject);
        }
    }

    public void LoadVolume()
    {
        SettingsData data = SaveSystem.LoadSettings();
        menuMusicSource.outputAudioMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(data.musicVolume) * 20);
    }
}
