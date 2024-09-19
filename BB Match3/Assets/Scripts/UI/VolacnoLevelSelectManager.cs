using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VolcanoLevelSelectionManager : MonoBehaviour
{
    public Button[] levelObjects;

    public static int currLevel;
    public static int UnlockedLevels;
    internal static int totalLevels;

    private void Start()
    {
        // Load unlocked levels from PlayerPrefs
        UnlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 10);

        Debug.Log($"Unlocked Levels: {UnlockedLevels}");

        // Initialize level buttons
        for (int i = 10; i < levelObjects.Length; i++)
        {
            levelObjects[i].interactable = i <= UnlockedLevels;
        }
    }

    public void OnClickLevel(int levelNum)
    {
        currLevel = levelNum;
        GridGenerator.levelNum = levelNum;
        SceneManager.LoadScene("Volcano");
    }
}


