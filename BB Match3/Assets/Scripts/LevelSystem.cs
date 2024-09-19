using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    static int numLevels = 21;
    static int[] levelSizes = new int[numLevels];
    static int[] levelLos = new int[numLevels];
    static int[] levelWin = new int[numLevels];
    static int[,,] levels;
    public static LevelSystem instance;
    public static void StartGame()
    {
        for (int i = 0; i < numLevels; i++)
        {
            levelSizes[i] = 9;
        }
        for (int i = 0; i < numLevels; i++)
        {
            levelLos[i] = 20;
        }
        levels = new int[numLevels, levelSizes[numLevels - 1], levelSizes[numLevels - 1]];
        // setting up stages
        // level 0 - 0
        levelWin[0] = 50;
        for (int i = 0; i < levelSizes[0]; i++)
        {
            levels[0, 0, i] = 1;
            levels[0, i, 0] = 1;
            levels[0, 8, i] = 1;
            levels[0, i, 8] = 1;
        }
        // level 0 - 1
        levelWin[1] = 100;
        levels[1, 0, 0] = levels[1, 8, 0] = levels[1, 0, 8] = levels[1, 8, 8] = 1;
        // level 0 - 2
        levelWin[2] = 100;
        for (int i = 0; i < levelSizes[2]; i++)
        {
            levels[2, 4, i] = 1;
        }
        // level 0 - 3
        levelWin[3] = 100;
        GetLayout(3, 1, 4);
        // level 0 - 4
        levelWin[4] = 150;
        GetLayout(4, 1, 1);
        // level 0 - 5
        levelWin[5] = 150;
        GetLayout(5, 1, 8);
        // level 0 - 6
        levelWin[6] = 100;
        GetLayout(6, 1, 6);
        levels[6, 0, 0] = levels[6, 8, 0] = levels[6, 0, 8] = levels[6, 8, 8] = 1;
        // level 0 - 7
        levelWin[7] = 100;
        GetLayout(7, 1, 9);
        // level 0 - 8
        levelWin[8] = 125;
        GetLayout(8, 1, 11);
        // level 0 - 9
        levelWin[9] = 120;
        GetLayout(9, 1, 8);
        levels[9, 4, 4] = levels[9, 3, 4] = levels[9, 4, 3] = levels[9, 5, 4] = levels[9, 4, 5] = 1;
        // level 1 - 0
        levelWin[10] = 100;
        for (int i = 0; i < levelSizes[10]; i++)
        {
            levels[10, i, 1] = 2;
            levels[10, 1, i] = 2;
            levels[10, i, 0] = 2;
            levels[10, 0, i] = 2;
            levels[10, i, 7] = 2;
            levels[10, 7, i] = 2;
            levels[10, i, 8] = 2;
            levels[10, 8, i] = 2;
        }
        levels[10, 0, 0] = levels[10, 8, 0] = levels[10, 0, 8] = levels[10, 8, 8] = 1;
        // level 1 - 1
        levelWin[11] = 150;
        for (int i = 0; i < levelSizes[11]; i++)
        {
            levels[11, 3, i] = 2;
            levels[11, 4, i] = 2;
            levels[11, 5, i] = 2;
        }
        // level 1 - 2
        levelWin[12] = 100;
        for (int i = 0; i < levelSizes[12]; i++)
        {
            levels[12, 0, i] = 2;
            levels[12, 8, i] = 2;
        }
        GetLayout(12, 1, 2);
        // level 1 - 3
        levelWin[13] = 150;
        GetLayout(13, 1, 1);
        GetLayout(13, 2, 4);
        // level 1 - 4
        levelWin[14] = 150;
        GetLayout(14, 2, 10);
        GetLayout(14, 1, 1);
        levels[14, 4, 4] = levels[14, 3, 4] = levels[14, 4, 3] = levels[14, 5, 4] = levels[14, 4, 5] = 1;
        // level 1 - 5
        levelWin[15] = 150;
        GetLayout(15, 2, 2);
        GetLayout(15, 1, 3);
        // level 1 - 6
        levelWin[16] = 175;
        for (int i = 0; i < levelSizes[16]; i++)
        {
            levels[16, i, i] = 2;
            levels[16, i, 8 - i] = 2;
        }
        // level 1 - 7
        levelWin[17] = 175;
        GetLayout(17, 2, 2);
        GetLayout(17, 2, 3);
        // level 1 - 8
        levelWin[18] = 200;
        GetLayout(18, 1, 1);
        GetLayout(18, 2, 2);
        GetLayout(18, 2, 3);
        // level 1 - 9
        levelWin[19] = 125;
        GetLayout(19, 1, 5);
        GetLayout(19, 2, 12);
        // level 2
        levelWin[20] = 1000;
        levelLos[20] = 100;
    }
    public static int[,] GetLevel(int levelNum)
    {
        int[,] levelLayout = new int[levelSizes[levelNum], levelSizes[levelNum]];
        for(int row=0; row < levelSizes[levelNum]; row++)
        {
            for(int col=0; col < levelSizes[levelNum]; col++)
                levelLayout[col,row] = levels[levelNum, col, row];
        }
        return levelLayout;
    }
    public static int GetWinCon(int levelNum)
    {
        return levelWin[levelNum];
    }
    public static int GetLosCon(int levelNum)
    {
        return levelLos[levelNum];
    }
    public static int GetBoardSize(int levelNum)
    {
        return levelSizes[levelNum];
    }
    public static void ResetLayout(int levelNum)
    {
        for (int row = 0; row < levelSizes[levelNum]; row++)
        {
            for (int col = 0; col < levelSizes[levelNum]; col++)
            {
                levels[levelNum, col, row] = 0;
            }
        }
    }
    public static void GetLayout(int levelNum, int obstacleType = 1, int layoutNum = -1)
    {
        if (layoutNum < 0 && obstacleType == 1)
            layoutNum = Random.Range(0, 9);
        else if (layoutNum < 0 && obstacleType == 2)
            layoutNum = Random.Range(8, 14);
        switch (layoutNum)
        {
            case 1: // circle layout
                levels[levelNum, 0, 0] = levels[levelNum, 8, 0] = levels[levelNum, 0, 8] = levels[levelNum, 8, 8] = obstacleType;
                levels[levelNum, 1, 0] = levels[levelNum, 7, 0] = levels[levelNum, 1, 8] = levels[levelNum, 7, 8] = obstacleType;
                levels[levelNum, 0, 1] = levels[levelNum, 8, 1] = levels[levelNum, 0, 7] = levels[levelNum, 8, 7] = obstacleType;
                break;
            case 2: // vertical cut
                for (int i = 0; i < levelSizes[levelNum]; i++)
                {
                    levels[levelNum, 4, i] = obstacleType;
                }
                break;
            case 3: // horizontal cut
                for (int i = 0; i < levelSizes[levelNum]; i++)
                {
                    levels[levelNum, i, 4] = obstacleType;
                }
                break;
            case 4: // middle square cut
                levels[levelNum, 4, 4] = levels[levelNum, 3, 4] = levels[levelNum, 3, 3] = levels[levelNum, 4, 3] = levels[levelNum, 5, 4] = levels[levelNum, 5, 5] = levels[levelNum, 4, 5] = levels[levelNum, 3, 5] = levels[levelNum, 5, 3] = obstacleType;
                break;
            case 5: // cross layout
                levels[levelNum, 0, 3] = levels[levelNum, 0, 4] = levels[levelNum, 0, 5] = levels[levelNum, 1, 4] = obstacleType;
                levels[levelNum, 8, 3] = levels[levelNum, 8, 4] = levels[levelNum, 8, 5] = levels[levelNum, 7, 4] = obstacleType;
                levels[levelNum, 3, 0] = levels[levelNum, 4, 0] = levels[levelNum, 5, 0] = levels[levelNum, 4, 1] = obstacleType;
                levels[levelNum, 3, 8] = levels[levelNum, 4, 8] = levels[levelNum, 5, 8] = levels[levelNum, 4, 7] = obstacleType;
                break;
            case 6: // big - small plus cut
                GetLayout(levelNum, obstacleType, 9);
                GetLayout(levelNum, 0, 12);
                break;
            case 7:
                break;
            case 8: // diamond layout
                levels[levelNum, 0, 0] = levels[levelNum, 8, 0] = levels[levelNum, 0, 8] = levels[levelNum, 8, 8] = obstacleType;
                levels[levelNum, 1, 0] = levels[levelNum, 7, 0] = levels[levelNum, 1, 8] = levels[levelNum, 7, 8] = obstacleType;
                levels[levelNum, 0, 1] = levels[levelNum, 8, 1] = levels[levelNum, 0, 7] = levels[levelNum, 8, 7] = obstacleType;
                levels[levelNum, 1, 1] = levels[levelNum, 7, 1] = levels[levelNum, 1, 7] = levels[levelNum, 7, 7] = obstacleType;
                levels[levelNum, 2, 0] = levels[levelNum, 6, 0] = levels[levelNum, 2, 8] = levels[levelNum, 6, 8] = obstacleType;
                levels[levelNum, 0, 2] = levels[levelNum, 8, 2] = levels[levelNum, 0, 6] = levels[levelNum, 8, 6] = obstacleType;
                break;
            case 9: // big plus cut
                GetLayout(levelNum, obstacleType, 2);
                GetLayout(levelNum, obstacleType, 3);
                break;
            case 10: // border layout
                for (int i = 0; i < levelSizes[levelNum]; i++)
                {
                    levels[levelNum, 0, i] = obstacleType;
                    levels[levelNum, i, 0] = obstacleType;
                    levels[levelNum, 8, i] = obstacleType;
                    levels[levelNum, i, 8] = obstacleType;
                }
                break;
            case 11: // plus layout
                levels[levelNum, 0, 0] = levels[levelNum, 8, 0] = levels[levelNum, 0, 8] = levels[levelNum, 8, 8] = obstacleType;
                levels[levelNum, 1, 0] = levels[levelNum, 7, 0] = levels[levelNum, 1, 8] = levels[levelNum, 7, 8] = obstacleType;
                levels[levelNum, 0, 1] = levels[levelNum, 8, 1] = levels[levelNum, 0, 7] = levels[levelNum, 8, 7] = obstacleType;
                levels[levelNum, 1, 1] = levels[levelNum, 7, 1] = levels[levelNum, 1, 7] = levels[levelNum, 7, 7] = obstacleType;
                levels[levelNum, 2, 0] = levels[levelNum, 6, 0] = levels[levelNum, 2, 8] = levels[levelNum, 6, 8] = obstacleType;
                levels[levelNum, 0, 2] = levels[levelNum, 8, 2] = levels[levelNum, 0, 6] = levels[levelNum, 8, 6] = obstacleType;
                levels[levelNum, 2, 2] = levels[levelNum, 6, 2] = levels[levelNum, 2, 6] = levels[levelNum, 6, 6] = obstacleType;
                levels[levelNum, 2, 1] = levels[levelNum, 6, 1] = levels[levelNum, 2, 7] = levels[levelNum, 6, 7] = obstacleType;
                levels[levelNum, 1, 2] = levels[levelNum, 7, 2] = levels[levelNum, 1, 6] = levels[levelNum, 7, 6] = obstacleType;
                break;
            case 12: // plus cut
                levels[levelNum, 4, 4] = levels[levelNum, 3, 4] = levels[levelNum, 4, 3] = levels[levelNum, 5, 4] = levels[levelNum, 4, 5] = obstacleType;
                break;
            default:
                break;
        }
    }
}
