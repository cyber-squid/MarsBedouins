using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Swap : MonoBehaviour
{
    public static void SwapScript(GridTile[,] tiles, DataHolder[,] gridDB, GridTile startTile, GridTile targetTile)
    {
        // use the position of both tiles in grid to get the dataholder value from them, then swap data holders
        // assign to temp to facilitate swap
        DataHolder tempTile = startTile.tileProp;
        int[] tempPos = new int[] { startTile.pos[0], startTile.pos[1] };
        gridDB[startTile.pos[0], startTile.pos[1]] = gridDB[targetTile.pos[0], targetTile.pos[1]];
        gridDB[targetTile.pos[0], targetTile.pos[1]] = tempTile;
        tiles[startTile.pos[0], startTile.pos[1]].SetDH(tiles[startTile.pos[0], startTile.pos[1]].tileProp, new int[] {targetTile.pos[0], targetTile.pos[1]});
        tiles[targetTile.pos[0], targetTile.pos[1]].SetDH(tiles[targetTile.pos[0], targetTile.pos[1]].tileProp,tempPos);
        tiles[startTile.pos[0], startTile.pos[1]] = startTile;
        tiles[targetTile.pos[0], targetTile.pos[1]] = targetTile;
    }
    public static DataHolder[,] TempSwapScript(DataHolder[,] gridDB, int boardSize, int[] startPos, int[] targetPos)
    {
        //make a temp array
        DataHolder[,] tempDB = new DataHolder[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if(startPos[0]==col && startPos[1] == row)
                {
                    tempDB[col, row] = gridDB[targetPos[0], targetPos[1]];
                }
                else if(targetPos[0] == col && targetPos[1] == row)
                {
                    tempDB[col, row] = gridDB[startPos[0], startPos[1]];
                }
                else 
                {
                    tempDB[col, row] = gridDB[col, row];
                }
            }
        }
        return tempDB;
    }

    public static bool SwapAdjacentCheck(GridTile chosenTile, GridTile tileToSwapTo, float tileSizeOnX, float tileSizeOnY)
    {
        // minusing one vector with another gives us the distance between the vectors.
        Vector2 direction = tileToSwapTo.transform.position - chosenTile.transform.position;
        direction.x = Mathf.Abs(direction.x);
        direction.y = Mathf.Abs(direction.y); // convert to absolute value since we only need to check if these are greater than a certain value.
        
        // check player isn't trying to go further than one tile for a swap
        if (direction.x <= tileSizeOnX && direction.y <= tileSizeOnY
            && direction.y != direction.x) // if we're moving on both x and y, it's a diagonal. make sure these don't happen either
        {
            return true;
        }

        return false;
    }
}