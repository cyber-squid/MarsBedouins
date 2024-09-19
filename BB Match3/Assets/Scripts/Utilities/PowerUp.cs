using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.StickyNote;
using UnityEngine.UIElements;

public class PowerUp : MonoBehaviour
{
    public static bool[,] BombTrigger(int[] coord, int size, int boardSize, DataHolder[,] gridDB)
    {
        bool[,] toDestroy = new bool[boardSize, boardSize];
        size--;
        if (size < 0)
            return toDestroy;
        toDestroy[coord[0], coord[1]] = true;
        for(int dir = 0; dir <= 3; dir++)
        {
            int[] nextCoord = new int[2];
            switch(dir) // assign the next 
            {
                case 0:
                    nextCoord[0] = coord[0] - 1;
                    nextCoord[1] = coord[1];
                    break;
                case 1:
                    nextCoord[0] = coord[0];
                    nextCoord[1] = coord[1] - 1;
                    break;
                case 2:
                    nextCoord[0] = coord[0] + 1;
                    nextCoord[1] = coord[1];
                    break;
                case 3:
                    nextCoord[0] = coord[0];
                    nextCoord[1] = coord[1] + 1;
                    break;
                default:
                    nextCoord[0] = coord[0];
                    nextCoord[1] = coord[1];
                    break;
            }
            bool[,] nextDestroy = new bool[boardSize, boardSize];
            if (nextCoord[0] >= 0 && nextCoord[0] < boardSize && nextCoord[1] >= 0 && nextCoord[1] < boardSize)
            {
                nextDestroy = BombTrigger(nextCoord, size, boardSize, gridDB);
                toDestroy = Match.AddSlots(nextDestroy, toDestroy, boardSize);
            }
        }
        return toDestroy;
    }
    public static bool[,] BishopTrigger(int[] coord, int size, int boardSize, DataHolder[,] gridDB)
    {
        bool[,] toDestroy = new bool[boardSize, boardSize];
        size--;
        if (size < 0)
            return toDestroy;
        toDestroy[coord[0], coord[1]] = true;
        for (int dir = 0; dir <= 3; dir++)
        {
            int[] nextCoord = new int[2];
            switch (dir) // assign the next 
            {
                case 0:
                    nextCoord[0] = coord[0] - 1;
                    nextCoord[1] = coord[1] - 1;
                    break;
                case 1:
                    nextCoord[0] = coord[0] + 1;
                    nextCoord[1] = coord[1] - 1;
                    break;
                case 2:
                    nextCoord[0] = coord[0] - 1;
                    nextCoord[1] = coord[1] + 1;
                    break;
                case 3:
                    nextCoord[0] = coord[0] + 1;
                    nextCoord[1] = coord[1] + 1;
                    break;
                default:
                    nextCoord[0] = coord[0];
                    nextCoord[1] = coord[1];
                    break;
            }
            bool[,] nextDestroy = new bool[boardSize, boardSize];
            if (nextCoord[0] >= 0 && nextCoord[0] < boardSize && nextCoord[1] >= 0 && nextCoord[1] < boardSize)
            {
                nextDestroy = BishopTrigger(nextCoord, size, boardSize, gridDB);
                toDestroy = Match.AddSlots(nextDestroy, toDestroy, boardSize);
            }
        }
        return toDestroy;
    }
    public static bool[,] RowColTrigger(int[] coord, bool xDir, int boardSize, DataHolder[,] gridDB)
    {
        bool[,] toDestroy = new bool[boardSize, boardSize];
        for (int tile=0; tile<boardSize; tile++)
        {
            if(xDir)
                toDestroy[tile, coord[1]] = true;
            else
                toDestroy[coord[0], tile] = true;
        }
        return toDestroy;
    }

    public static bool[,] ColorTrigger(TileColors tileColor, int colorNum, int boardSize, DataHolder[,] gridDB)
    {
        bool[,] toDestroy = new bool[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (gridDB[col, row].currentColor == tileColor)
                    toDestroy[col, row] = true;
            }
        }
        return toDestroy;
    }
    public static TileColors GetMaxColor(int colorNum, int boardSize, DataHolder[,] gridDB)
    {
        TileColors tileColor = TileColors.empty;
        int[] colorCount = new int[colorNum];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
               if (gridDB[col, row].currentColor > TileColors.empty)
               colorCount[(int)gridDB[col, row].currentColor]++;
            }
        }
        int maxCount = 0;
        for (int i = 0; i < colorNum; i++)
        {
            if (colorCount[i] > maxCount)
                tileColor = (TileColors)i;
            maxCount = (int)Mathf.Max(colorCount[i], maxCount);
        }
        return tileColor;
    }
}
