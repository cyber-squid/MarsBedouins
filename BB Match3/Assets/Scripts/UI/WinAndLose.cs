using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class WinAndLose : MonoBehaviour
{
    OptionManager op;
    public static WinAndLose Instance;
    [SerializeField] GameObject pauseHolder;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public GameObject replayButton;
    public GameObject nextlevel;

    public void lose()
    {
        replayButton.SetActive(true);
        OptionManager.GameIsPaused = true;
    }
    public void win()
    {
        if (GridGenerator.levelNum == LevelSelectionManager.UnlockedLevels)
        {
            LevelSelectionManager.UnlockedLevels++;
            PlayerPrefs.SetInt("UnlockedLevels", LevelSelectionManager.UnlockedLevels);
            PlayerPrefs.Save(); // Ensure changes are saved
        }
        OptionManager.GameIsPaused = true;
        nextlevel.SetActive(true);
        pauseHolder.SetActive(false);
    }

    public void RestartButton()
    {

        if (GridGenerator.levelNum < 10)
        {
            Debug.Log("Loading Tent scene.");
            SceneManager.LoadScene("Tent");
        }
        else if (GridGenerator.levelNum >= 10)
        {
            Debug.Log("Loading Volcano scene.");
            SceneManager.LoadScene("Volcano");
        }
    }
    public void nextlevel1()
    {

       SceneManager.LoadScene("Tent");
    }

}
