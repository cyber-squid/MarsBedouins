using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public static int GravityScriptA(int boardSize, DataHolder[,] gridDB, GridTile[,] tiles, int colorNum, GridTile procTile)
    {
        if (procTile.tileProp.currentColor > TileColors.empty) // if the tile is filled, ignore
        {
            return procTile.pos[1];
        }
        for (int row = procTile.pos[1]; row < boardSize; row++)
        {
            // this is the bottom-most empty of the cluster, find the next non-empty item
            if (gridDB[procTile.pos[0], row].powerUp > 0 || gridDB[procTile.pos[0], row].currentColor > TileColors.empty || gridDB[procTile.pos[0], row].currentColor < TileColors.invisibleObstacle) // if the current tile is colored
            {
                return row;
            }
        }
        return -1;
    }
   
    public static void RegenerateScript(int boardSize, DataHolder[,] gridDB, int colorNum)
    {
        for(int row = 0;  row < boardSize; row++)
        {
            for(int col=0; col < boardSize; col++)
            {
                if (gridDB[col,row].currentColor == TileColors.empty)
                {
                    gridDB[col,row].currentColor = (TileColors)Random.Range(0,colorNum);
                }
            }
        }
    }
    public static void RegenerateScriptA(DataHolder[,] gridDB, int colorNum, GridTile procTile)
    {
        if (gridDB[procTile.pos[0], procTile.pos[1]].currentColor == TileColors.empty)
        {
            gridDB[procTile.pos[0], procTile.pos[1]].currentColor = (TileColors)Random.Range(0, colorNum);
        }
    }
}
