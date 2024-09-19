using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public int totalLevels = 10; // Default value; ensure this is set correctly

    private void Start()
    {
        // Ensure totalLevels is set correctly
        Debug.Log($"TotalLevels initialized to: {totalLevels}");

        if (totalLevels <= 0)
        {
            Debug.LogError("TotalLevels not set or is invalid.");
        }
    }

    public void OnLevelComplete()
    {
        print("complete");
        Debug.Log($"OnLevelComplete called. Current Level: {LevelSelectionManager.currLevel}, Unlocked Levels: {LevelSelectionManager.UnlockedLevels}, Total Levels: {totalLevels}");

        // Increment the number of unlocked levels if the current level is the highest unlocked
        if(GridGenerator.levelNum < 20) GridGenerator.levelNum++;
        // Increment level number for grid generation

        // Determine the appropriate scene to load
        if (GridGenerator.levelNum >= totalLevels)
        {
            Debug.Log("All levels completed. Loading Islands scene.");
            if(LevelSelectionManager.UnlockedLevels < 20)
                SceneManager.LoadScene("LevelSele 2");
            else
                SceneManager.LoadScene("Islands");
        }
        else if(GridGenerator.levelNum < 10)
        {
            Debug.Log("Loading Tent scene.");
            SceneManager.LoadScene("Tent");
        }
        else if(GridGenerator.levelNum >= 10 && GridGenerator.levelNum < 20)
        {
            Debug.Log("Loading Volcano scene.");
            SceneManager.LoadScene("Volcano");
        }
        else
            onReplay();
    }

    public void onReplay()
    {
        Debug.Log("Replay button clicked");

        // Replay logic - simply load the current level scene

        if (GridGenerator.levelNum < 10)
        {
            Debug.Log("Loading Tent scene.");
            SceneManager.LoadScene("Tent");
        }
        else if (GridGenerator.levelNum >= 10 && GridGenerator.levelNum < 20)
        {
            Debug.Log("Loading Volcano scene.");
            SceneManager.LoadScene("Volcano");
        }
        else
        {
            if(Random.Range(0, 2) == 0)
            {
                SceneManager.LoadScene("Tent");
            }
            else
            {
                SceneManager.LoadScene("Volcano");
            }
        }
    }

    public void onBack()
    {
        if(GridGenerator.levelNum < 10)
        {
            SceneManager.LoadScene("LevelSele 1");
        }
        else if (GridGenerator.levelNum >= 10 && GridGenerator.levelNum < 20)
        {
            SceneManager.LoadScene("LevelSele 2");
        }
        else
        {
            SceneManager.LoadScene("Islands");
        }
    }

 
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelComplete : MonoBehaviour
//{

//    public void OnLevelComplete()
//    {
//        if (LevelSelectionManager.currLevel == LevelSelectionManager.UnlockedLevels)
//        {
//            LevelSelectionManager.UnlockedLevels++;
//            LevelSelectionManager.currLevel = LevelSelectionManager.UnlockedLevels;
//            PlayerPrefs.SetInt("UnlockedLevels", LevelSelectionManager.UnlockedLevels);
//        }
//        GridGenerator.levelNum++;

//        SceneManager.LoadScene("Tent");
//    }

//    public void onReplay()
//    {
//        if (LevelSelectionManager.currLevel == LevelSelectionManager.UnlockedLevels)
//        {
//            LevelSelectionManager.UnlockedLevels++;

//            PlayerPrefs.SetInt("UnlockedLevels", LevelSelectionManager.UnlockedLevels);
//        }

//        SceneManager.LoadScene("Tent");
//    }
//}
