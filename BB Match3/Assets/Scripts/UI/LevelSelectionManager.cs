using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    public Button[] levelObjects;
    public int islandNum = 0;
    public static int currLevel;
    public static int UnlockedLevels;
    internal static int totalLevels;

    private void Start()
    {
        // Load unlocked levels from PlayerPrefs
        UnlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 0);

        Debug.Log($"Unlocked Levels: {UnlockedLevels}");

        // Initialize level buttons
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].interactable = i <= UnlockedLevels - 10 * islandNum;
        }
    }

    public void OnClickLevel(int levelNum)
    {
        print(levelNum);
        currLevel = levelNum;
        GridGenerator.levelNum = levelNum;
        if(currLevel < 10)
            SceneManager.LoadScene("Tent");
        else if(currLevel > 10 && currLevel < 20)
            SceneManager.LoadScene("Volcano");
        else if(currLevel == 20)
        {
            if(Random.Range(0, 2) > 0)
                SceneManager.LoadScene("Tent");
            else
                SceneManager.LoadScene("Volcano");

        }
    }
}





//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;

//public class LevelSelectionManager : MonoBehaviour
//{
//    public Button[] levelObjects;

//    public static int currLevel;
//    public static int UnlockedLevels;


//    public int test;

//    public void OnClickLevel(int levelNum)
//    {
//        currLevel = levelNum;

//        GridGenerator.levelNum = levelNum;
//        SceneManager.LoadScene("Tent");
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        UnlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 0);
//        test = UnlockedLevels;
//        for (int i = 0; i < UnlockedLevels + 1; i++)
//        {
//            if (i < levelObjects.Length)
//            {
//                levelObjects[i].interactable = true;
//            }
//        }
//    }


//}
