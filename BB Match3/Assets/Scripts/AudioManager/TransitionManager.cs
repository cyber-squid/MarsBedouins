using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    public float fadeDuration = 1.0f; // Duration for the fade out/in effect
    public AudioSource musicSource;

    // List of scenes that should use transition effects
    public string[] transitionScenes;
    public string[] volcanoScenes; // List of scenes with volcano music that should bypass transition effects

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

    public void StartTransition(string sceneName)
    {
        if (IsVolcanoScene(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else if (IsTransitionScene(sceneName))
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private bool IsTransitionScene(string sceneName)
    {
        foreach (string scene in transitionScenes)
        {
            if (scene == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsVolcanoScene(string sceneName)
    {
        foreach (string scene in volcanoScenes)
        {
            if (scene == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        yield return StartCoroutine(FadeOutMusic());

        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeInMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        if (musicSource != null)
        {
            float startVolume = musicSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            musicSource.volume = 0;
        }
    }

    private IEnumerator FadeInMusic()
    {
        if (musicSource != null)
        {
            float startVolume = 0;
            float targetVolume = AudioManager.instance.menuMusicVolume; // Adjust as needed
            musicSource.volume = startVolume;

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeDuration);
                yield return null;
            }
            musicSource.volume = targetVolume;
        }
    }
}
