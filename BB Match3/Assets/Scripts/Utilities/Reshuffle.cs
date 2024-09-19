using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Reshuffle : MonoBehaviour
{
    public static GridTile ReShuffleScript(DataHolder[,] gridDB,GridTile[,] tiles, int boardSize)
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (tiles[col, row].tileProp.powerUp > 1) // if there was a triggerable powerup, no need to reshuffle
                    return tiles[col, row];
                if (tiles[col, row].tileProp.currentColor < TileColors.empty)
                    continue;

                for(int dir = 0; dir <= 1; dir++) // check for 2 distinct swap directions on each tile
                {
                    // setting up the facilitator
                    int[] dirFac = new int[2];
                    dirFac[dir] = 1; // add 1 to the correct direction
                    dirFac[(dir + 1) % 2] = 0; // add 0 to the other
                    // swapping in the upper and right directions because swapping is commutative
                    if (row + dirFac[1] < boardSize && col + dirFac[0] < boardSize) // if not out of bounds
                    {
                        int[] startPos = new int[2] { col, row };
                        int[] targetPos = new int[2]; // set target position using the dirFac array as a facilitator

                        
                        targetPos[0] = col + dirFac[0];
                        targetPos[1] = row + dirFac[1];

                        if (tiles[targetPos[0], targetPos[1]].tileProp.currentColor < TileColors.empty)
                            continue;
                        
                        DataHolder[,] tempBoard = Swap.TempSwapScript(gridDB, boardSize,startPos, targetPos); // make swap in a new array
                        bool[,] matchings = Match.SetupMatchScript(tempBoard,boardSize);
                        if (matchings[targetPos[0], targetPos[1]])
                            return tiles[col, row]; // if there was a match, you do not need to reshuffle
                        if (matchings[col, row])
                            return tiles[targetPos[0], targetPos[1]];
                    }
                }
            }
        }
        return null; // else, you do
    }
}
