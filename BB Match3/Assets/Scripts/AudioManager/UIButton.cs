using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIButton : MonoBehaviour
{
    public Button button; // Assign this in the Unity Inspector
    public string sceneToLoad; // The target scene name to load
    public float delayBeforeTransition = 0.5f; // Delay before starting transition, adjust as needed

    private void Start()
    {
        // Add a listener to the button click event
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        // Play the button click sound effect
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayButtonClickSound();
        }

        // Start the transition after a short delay
        StartCoroutine(StartTransitionAfterDelay(delayBeforeTransition));
    }

    private IEnumerator StartTransitionAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Trigger transition
        if (TransitionManager.instance != null)
        {
            TransitionManager.instance.StartTransition(sceneToLoad);
        }
        
    }
}
