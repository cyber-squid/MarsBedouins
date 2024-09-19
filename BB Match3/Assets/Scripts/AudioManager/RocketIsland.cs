using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketIsland : MonoBehaviour
{
    public void OnRocketIslandClicked()
    {
        // Change the scene to LevelSele1
        SceneManager.LoadScene("LevelSele 1");

        // Notify the AudioManager to switch music
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayRocketMusic();
        }
    }
}