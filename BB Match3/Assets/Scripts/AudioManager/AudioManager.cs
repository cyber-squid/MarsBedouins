using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource; // Use a single AudioSource for all music
    public AudioClip menuMusicClip;
    public AudioClip rocketMusicClip;
    public AudioClip volcanoMusicClip;

    [Range(0f, 1f)]
    public float menuMusicVolume = 0.5f;
    [Range(0f, 1f)]
    public float rocketMusicVolume = 0.5f;
    [Range(0f, 1f)]
    public float volcanoMusicVolume = 0.5f;

    private void Awake()
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

    private void Start()
    {
        InitializeMusic();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMusic();
    }

    private void InitializeMusic()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Current Scene: " + currentScene);

        if (currentScene == "Menu" || currentScene == "Islands" || currentScene == "Credits" || currentScene == "Glossary")
        {
            if (musicSource.clip != menuMusicClip)
            {
                Debug.Log("Playing Menu Music");
                PlayMenuMusic();
            }
        }
        else if (currentScene == "LevelSele 1" || currentScene == "Tent")
        {
            if (musicSource.clip != rocketMusicClip)
            {
                Debug.Log("Playing Rocket Music");
                PlayRocketMusic();
            }
        }
        else if (currentScene == "LevelSele 2" || currentScene == "Volcano")
        {
            if (musicSource.clip != volcanoMusicClip)
            {
                Debug.Log("Playing Volcano Music");
                PlayVolcanoMusic();
            }
        }
        else
        {
            Debug.Log("Stopping Music");
            StopMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (musicSource != null && menuMusicClip != null)
        {
            Debug.Log("Setting Menu Music Clip");
            musicSource.clip = menuMusicClip;
            musicSource.volume = menuMusicVolume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Menu Music Source or Clip is missing!");
        }
    }

    public void PlayRocketMusic()
    {
        if (musicSource != null && rocketMusicClip != null)
        {
            Debug.Log("Setting Rocket Music Clip");
            musicSource.clip = rocketMusicClip;
            musicSource.volume = rocketMusicVolume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Rocket Music Source or Clip is missing!");
        }
    }

    public void PlayVolcanoMusic()
    {
        if (musicSource != null && volcanoMusicClip != null)
        {
            Debug.Log("Setting Volcano Music Clip");
            musicSource.clip = volcanoMusicClip;
            musicSource.volume = volcanoMusicVolume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Volcano Music Source or Clip is missing!");
        }
    }

    private void StopMusic()
    {
        if (musicSource != null)
        {
            Debug.Log("Stopping Music");
            musicSource.Stop();
        }
    }
}
