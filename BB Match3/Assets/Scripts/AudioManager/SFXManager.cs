using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource sfxSource;
    public AudioClip buttonClickClip; // Regular button click sound

    [Range(0f, 1f)]
    public float buttonClickVolume = 1f; // Volume for button clicks

    private void Awake()
    {
        // Ensure only one instance of SFXManager exists
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

    public void PlayButtonClickSound()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            sfxSource.PlayOneShot(buttonClickClip, buttonClickVolume);
        }
        else
        {
            Debug.LogWarning("SFXManager is missing the required components or audio clips.");
        }
    }
}
