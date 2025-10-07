using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    void Start()
    {
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
}