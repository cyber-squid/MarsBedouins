using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AudioMenu : MonoBehaviour
{
    [Header("----------- Audio Source --------------")]

    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip --------")]
    public AudioClip background;
    public AudioClip swipe;
    public AudioClip select;
    public AudioClip unswap;

    public static AudioMenu instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (SceneManager.GetActiveScene().name == "LevelSele 1")
        {
            AudioMenu.instance.GetComponent<AudioSource>().Pause();
        }
    }

    public void Start()
    {
        MusicSource.clip = background;
        MusicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}