using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VolcanoIsland : MonoBehaviour
{
    public void OnRocketIslandClicked()
    {
        // Start the scene transition coroutine
        StartCoroutine(TransitionToVolcanoIsland());
    }

    private IEnumerator TransitionToVolcanoIsland()
    {
        // Load the LevelSele2 scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelSele 2");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Once the scene is loaded, notify the AudioManager to switch music
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayVolcanoMusic();
        }
    }
}
