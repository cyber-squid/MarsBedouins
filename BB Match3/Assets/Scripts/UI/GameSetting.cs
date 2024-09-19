using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example of setting totalLevels in a setup script
public class GameSettings : MonoBehaviour
{
    public LevelComplete levelCompleteScript; // Reference to the LevelComplete script

    void Start()
    {
        // Ensure totalLevels is set correctly
        if (levelCompleteScript != null)
        {
            levelCompleteScript.totalLevels = 10; // Set the total number of levels
        }
        else
        {
            Debug.LogError("LevelComplete script reference not set in GameSettings.");
        }
    }
}

