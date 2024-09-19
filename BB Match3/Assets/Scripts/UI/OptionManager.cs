using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public SFXaudio sfx;
    public AudioMenu am;

    private void Awake()
    {
        sfx= GameObject.FindGameObjectWithTag("GameAudio").GetComponent<SFXaudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
    }

    [SerializeField] public GameObject PauseMenuPanel;
    public static bool GameIsPaused = false;


    public void Pause()
    {
        sfx.PlaySFX(sfx.select);
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        sfx.PlaySFX(sfx.select);
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
   
}
