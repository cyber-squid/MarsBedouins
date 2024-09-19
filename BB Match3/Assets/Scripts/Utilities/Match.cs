using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Match : MonoBehaviour
{
    public static bool[,] AddSlots(bool[,] firstArr, bool[,] secondArr, int gridSize)
    {
        for (int row = 0; row < gridSize; row++) // combines the values of all the bool arrays
        {
            for (int col = 0; col < gridSize; col++)
            {
                firstArr[col, row] = firstArr[col, row] || secondArr[col, row];
            }
        }
        return firstArr;
    }
    static bool PearlScript(GridTile[,] tiles, int boardSize, bool xDir, int prog, TileColors prevColor, int[] coord)
    {
        if (coord[0] >= boardSize || coord[1] >= boardSize) // if out of bounds
            return prog >= 3;
        GridTile currTile = tiles[coord[0], coord[1]];
        if (currTile.tileProp.powerUp == 1)
        {
            currTile.tileProp.currentColor = prevColor;
            if (currTile.tileProp.currentColor <= TileColors.empty)
                currTile.tileProp.currentColor = (TileColors)0;
        }
        if(currTile.tileProp.currentColor <= TileColors.empty)
            return prog >= 3;
        if (currTile.tileProp.currentColor != prevColor && prog > 0 && currTile.tileProp.powerUp != 1 && prevColor > TileColors.empty) // if the color is different
            return prog >= 3;
        else
        {

            prog++;
            int[] nextCoord = new int[2];
            nextCoord[0] = xDir ? coord[0] + 1 : coord[0];
            nextCoord[1] = xDir ? coord[1] : coord[1] + 1;
            prevColor = currTile.tileProp.currentColor;
            if (PearlScript(tiles, boardSize, xDir, prog, currTile.tileProp.powerUp == 1?TileColors.empty:prevColor, nextCoord))
            { 
                if (currTile.tileProp.powerUp == 1)
                {
                    if (nextCoord[0] < boardSize && nextCoord[1] < boardSize)
                    {
                        if (tiles[nextCoord[0], nextCoord[1]].tileProp.currentColor > TileColors.empty)
                            currTile.tileProp.currentColor = tiles[nextCoord[0], nextCoord[1]].tileProp.currentColor;
                    }
                }
                return true;
            }
            return false;
        }
    }
    static int PowerUpHandler(GridTile[,] tiles, bool[,] matches, int boardSize, int[] coord, int prog) // first cast to all directions (because this is where the intersection is)
    {
        int power = prog - 2;
        int maxpup = 0;
        for (int dir = 0; dir < 4; dir++)
        {
            int[] nextCoord = new int[2];
            switch (dir) // assign the next 
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
            maxpup = power;
            if (nextCoord[0] < boardSize && nextCoord[0] >= 0 && nextCoord[1] < boardSize && nextCoord[1] >= 0)
            {
                TileColors currColor = tiles[coord[0], coord[1]].tileProp.powerUp == 1 ? TileColors.empty : tiles[coord[0], coord[1]].tileProp.currentColor;
                maxpup = PowerUpHandler(tiles, matches, boardSize, maxpup, nextCoord, tiles[coord[0], coord[1]].tileProp.currentColor, dir);
            }
            if (maxpup > power)
            {
                prog = 0;
            }
        }
        return prog;
    }
    static int PowerUpHandler(GridTile[,] tiles, bool[,] matches, int boardSize, int maxpup, int[] coord, TileColors prevColor, int dir)
    {
        TileColors currColor = tiles[coord[0], coord[1]].tileProp.powerUp == 1 ? TileColors.empty : tiles[coord[0], coord[1]].tileProp.currentColor;
        if (!matches[coord[0], coord[1]] || (currColor != prevColor && currColor != TileColors.empty && prevColor != TileColors.empty))
            return maxpup;
        if (maxpup >= tiles[coord[0], coord[1]].tileProp.powerUp && tiles[coord[0], coord[1]].tileProp.newPower)
            tiles[coord[0], coord[1]].tileProp.powerUp = 0;
        else if(tiles[coord[0], coord[1]].tileProp.newPower)
            maxpup = tiles[coord[0], coord[1]].tileProp.powerUp;
        int[] nextCoord = new int[2];
        switch (dir) // assign the next 
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
        if (nextCoord[0] < boardSize && nextCoord[0] >= 0 && nextCoord[1] < boardSize && nextCoord[1] >= 0)
        {
            currColor = tiles[coord[0], coord[1]].tileProp.powerUp == 1 ? TileColors.empty : tiles[coord[0], coord[1]].tileProp.currentColor;
            maxpup = Mathf.Max(PowerUpHandler(tiles, matches, boardSize, maxpup, nextCoord, tiles[coord[0], coord[1]].tileProp.currentColor, dir), maxpup);
        }
        if (maxpup > tiles[coord[0], coord[1]].tileProp.powerUp && tiles[coord[0], coord[1]].tileProp.newPower)
        {
            tiles[coord[0], coord[1]].tileProp.powerUp = 0;
            tiles[coord[0], coord[1]].tileProp.newPower = false;
        }
        return maxpup;
    }
    static int IntersectionCounter(GridTile[,] tiles, bool[,] matches, int boardSize, int[] coord) // first cast to all directions (because this is where the intersection is)
    {
        int prog = 0;
        prog++;
        for (int dir = 0; dir < 4; dir++)
        {
            int[] nextCoord = new int[2];
            switch (dir) // assign the next 
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
            if (nextCoord[0] < boardSize && nextCoord[0] >= 0 && nextCoord[1] < boardSize && nextCoord[1] >= 0)
            {
                TileColors currColor = tiles[coord[0], coord[1]].tileProp.powerUp == 1 ? TileColors.empty : tiles[coord[0], coord[1]].tileProp.currentColor;
                prog = IntersectionCounter(tiles, matches, boardSize, prog, nextCoord, tiles[coord[0], coord[1]].tileProp.currentColor, dir);
            }
        }
        PowerUpHandler(tiles, matches, boardSize, coord, prog);
        return PowerUpHandler(tiles, matches, boardSize, coord, prog);
    }
    static int IntersectionCounter(GridTile[,] tiles, bool[,] matches, int boardSize, int prog, int[] coord, TileColors prevColor, int dir)
    {
        TileColors currColor = tiles[coord[0], coord[1]].tileProp.powerUp == 1 ? TileColors.empty : tiles[coord[0], coord[1]].tileProp.currentColor;
        if (!matches[coord[0], coord[1]] || (currColor != prevColor && currColor != TileColors.empty && prevColor != TileColors.empty))
            return prog;
        prog++;
        int[] nextCoord = new int[2];
        switch (dir) // assign the next 
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
        if (nextCoord[0] < boardSize && nextCoord[0] >= 0 && nextCoord[1] < boardSize && nextCoord[1] >= 0)
        {
            prog = IntersectionCounter(tiles, matches, boardSize, prog, nextCoord, currColor, dir);
        }
        return prog;
    }
    static bool[,] MatchScript(GridTile[,] tiles, int boardSize, bool xDir, int prog, TileColors prevColor, int[] coord)
    {
        bool[,] toDestroy = new bool[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                toDestroy[col, row] = false;
            }
        }
        GridTile currTile = tiles[coord[0], coord[1]];
        if (!xDir) // check if in yDir, bool set to true if you are checking in the x direction through this branch
        {
            if (currTile.tileProp.powerUp == 1)
            {
                int[] next = new int[] { coord[0], coord[1] + 1 };
                if (PearlScript(tiles, boardSize, xDir, 0, prevColor, coord) && prog < 2)
                {
                    if (next[0] < boardSize &&  next[1] < boardSize)
                    {
                        if (tiles[next[0], next[1]].tileProp.powerUp != 1 && prevColor != tiles[next[0], next[1]].tileProp.currentColor)
                        {
                            prog--;
                        }
                    }
                }
            }
            if ((currTile.tileProp.currentColor == prevColor || currTile.tileProp.powerUp == 1) && prevColor > TileColors.empty)
            {
                prog++;
            }
            else if (prog >= 2) // if 3 or more of the same color are in a row
            {
                if (currTile.tileProp.currentColor == TileColors.breakableObstacle) // if the breakable obstacle is vertically in line with match
                {
                    toDestroy[coord[0], coord[1]] = true;
                }
                for (int i = 0; i <= prog; i++)
                {
                    for (int dir = -1; dir <= 1; dir++) // check the horizontal pieces
                    {
                        if (coord[0] + dir < boardSize && coord[0] + dir >= 0) // if in range 
                            if (dir == 0 || tiles[coord[0] + dir, coord[1] - i - 1].tileProp.currentColor == TileColors.breakableObstacle) // if main tile or breakable next to it
                                toDestroy[coord[0] + dir, coord[1] - i - 1] = true;
                    }
                }
                if (coord[1] - prog - 2 >= 0) // check the other endof the vertical line
                {
                    if (tiles[coord[0], coord[1] - prog - 2].tileProp.currentColor == TileColors.breakableObstacle)
                        toDestroy[coord[0], coord[1] - prog - 2] = true;
                }
                // powerUp application
                if(prog >= 5)
                {
                    tiles[coord[0], coord[1] - 1].tileProp.powerUp = 5;
                    tiles[coord[0], coord[1] - 1].tileProp.newPower = true;
                }
                else if (prog >= 4)
                {
                    tiles[coord[0], coord[1] - 1].tileProp.powerUp = 2;
                    tiles[coord[0], coord[1] - 1].tileProp.newPower = true;
                }
                else if (prog >= 3)
                {
                    tiles[coord[0], coord[1] - 1].tileProp.powerUp = 1;
                    tiles[coord[0], coord[1] - 1].tileProp.newPower = true;
                }
                prog = 0; // reset because it's a different color
            }
            else // if it's a different color but not enough matches
            {
                prog = 0;
            }
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0];
            nextCoord[1] = coord[1] + 1;
            if (nextCoord[1] < boardSize) // if the target is not out of bounds
            {
                bool[,] rexArr = MatchScript(tiles, boardSize, xDir, prog, currTile.tileProp.currentColor, nextCoord);
                toDestroy = AddSlots(toDestroy, rexArr, boardSize);
            }
            else if (prog >= 2) // exception for if the piece is at the edge of the board to force the connected tiles to the destroy box
            {
                for (int i = 0; i <= prog; i++)
                {
                    for (int dir = -1; dir <= 1; dir++) // check the horizontal pieces
                    {

                        if (coord[0] + dir < boardSize && coord[0] + dir >= 0) // if in range 
                            if (dir == 0 || tiles[coord[0] + dir, coord[1] - i].tileProp.currentColor == TileColors.breakableObstacle) // if main tile or breakable next to it
                                toDestroy[coord[0] + dir, coord[1] - i] = true;
                    }
                }
                if (coord[1] - prog - 1 >= 0) // check the other endof the vertical line
                {
                    if (tiles[coord[0], coord[1] - prog - 1].tileProp.currentColor == TileColors.breakableObstacle)
                        toDestroy[coord[0], coord[1] - prog - 1] = true;
                }
                // powerUp application
                if (prog >= 5)
                {
                    currTile.tileProp.powerUp = 5;
                    currTile.tileProp.newPower = true;
                }
                else if (prog >= 4)
                {
                    currTile.tileProp.powerUp = 2;
                    currTile.tileProp.newPower = true;
                }
                else if (prog >= 3)
                {
                    currTile.tileProp.powerUp = 1;
                    currTile.tileProp.newPower = true;
                }
                prog = 0;
            }
        }
        else // if checking the xDir
        {
            if (currTile.tileProp.powerUp == 1)
            {
                int[] next = new int[] { coord[0] + 1, coord[1] };
                if (PearlScript(tiles, boardSize, xDir, 0, prevColor, coord) && prog < 2)
                {
                    if (next[0] < boardSize && next[1] < boardSize)
                    {
                        if (tiles[next[0], next[1]].tileProp.powerUp != 1 && prevColor != tiles[next[0], next[1]].tileProp.currentColor)
                        {
                            prog--;
                        }
                    }
                }
            }
            if ((currTile.tileProp.currentColor == prevColor || currTile.tileProp.powerUp == 1) && prevColor > TileColors.empty)
            {
                prog++;
            }
            else if (prog >= 2) // if 3 or more of the same color are in a column
            {
                if (currTile.tileProp.currentColor == TileColors.breakableObstacle) // if the breakable obstacle is horizontally in line with match
                {
                    toDestroy[coord[0], coord[1]] = true;
                }
                for (int i = 0; i <= prog; i++)
                {
                    for (int dir = -1; dir <= 1; dir++) // check the vertical pieces
                    {
                        if (coord[1] + dir < boardSize && coord[1] + dir >= 0) // if in range 
                            if (dir == 0 || tiles[coord[0] - i - 1, coord[1] + dir].tileProp.currentColor == TileColors.breakableObstacle) // if main tile or breakable next to it
                                toDestroy[coord[0] - i - 1, coord[1] + dir] = true;
                    }
                }
                if (coord[0] - prog - 2 >= 0) // check the other end of the horizontal line
                {
                    if (tiles[coord[0] - prog - 2, coord[1]].tileProp.currentColor == TileColors.breakableObstacle)
                        toDestroy[coord[0] - prog - 2, coord[1]] = true;
                }
                // powerUp application
                if (prog >= 5)
                {
                    tiles[coord[0] - 1, coord[1]].tileProp.powerUp = 5;
                    tiles[coord[0], coord[1] - 1].tileProp.newPower = true;
                }
                else if (prog >= 4)
                {
                    tiles[coord[0] - 1, coord[1]].tileProp.powerUp = 2;
                    tiles[coord[0] - 1, coord[1]].tileProp.newPower = true;
                }
                else if (prog >= 3)
                {
                    tiles[coord[0] - 1, coord[1]].tileProp.powerUp = 1;
                    tiles[coord[0] - 1, coord[1]].tileProp.newPower = true;
                }
                    prog = 0; // reset because it's a different color
            }
            else // if it's a different color but not enough matches
            {
                prog = 0;
            }
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0] + 1;
            nextCoord[1] = coord[1];
            if (nextCoord[0] < boardSize) // if the target is not out of bounds
            {
                bool[,] reyArr = MatchScript(tiles, boardSize, xDir, prog, currTile.tileProp.currentColor, nextCoord);
                toDestroy = AddSlots(toDestroy, reyArr, boardSize);
            }
            else if (prog >= 2) // exception for if the piece is at the edge of the board to force the connected tiles to the destroy box
            {
                for (int i = 0; i <= prog; i++)
                {
                    for (int dir = -1; dir <= 1; dir++) // check the vertical pieces
                    {
                        if (coord[1] + dir < boardSize && coord[1] + dir >= 0) // if in range 
                            if (dir == 0 || tiles[coord[0] - i, coord[1] + dir].tileProp.currentColor == TileColors.breakableObstacle) // if main tile or breakable next to it
                                toDestroy[coord[0] - i, coord[1] + dir] = true;
                    }
                }
                if (coord[1] - prog - 1 >= 0) // check the other end of the horizontal line
                {
                    if (tiles[coord[0] - prog - 1, coord[1]].tileProp.currentColor == TileColors.breakableObstacle)
                        toDestroy[coord[0] - prog - 1, coord[1]] = true;
                }
                // powerUp application
                if (prog >= 4)
                {
                    currTile.tileProp.powerUp = 2;
                    currTile.tileProp.newPower = true;
                }
                else if (prog >= 3)
                {
                    currTile.tileProp.powerUp = 1;
                    currTile.tileProp.newPower = true;
                }
                prog = 0;
            }
        }
        if (!xDir && coord[0] == 0) // if the first row, also send recursions to the other direction
        {
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0] + 1;
            nextCoord[1] = coord[1];
            if (tiles[coord[0], coord[1]].tileProp.powerUp == 1)
                currTile.tileProp.currentColor = tiles[nextCoord[0], nextCoord[1]].tileProp.currentColor;
            if (nextCoord[0] < boardSize) // if the target is not out of bounds
            {
                bool[,] reyArr = MatchScript(tiles, boardSize, !xDir, 0, currTile.tileProp.currentColor, nextCoord);
                for (int row = 0; row < boardSize; row++)
                    for (int col = 0; col < boardSize; col++)
                    {
                        if (reyArr[col, row] && toDestroy[col, row] && tiles[col, row].tileProp.currentColor >= TileColors.empty)
                        {
                            int iCount = IntersectionCounter(tiles, AddSlots(toDestroy, reyArr, boardSize), boardSize, new int[] { col, row });
                            if(iCount >= 7)
                                tiles[col, row].tileProp.powerUp = 5;
                            else if (iCount > 5)
                                tiles[col, row].tileProp.powerUp = 4;
                            else if(iCount == 5)
                                tiles[col, row].tileProp.powerUp = 3;
                            if(iCount >= 5)
                                tiles[col, row].tileProp.newPower = true;
                        }
                    }
                toDestroy = AddSlots(toDestroy, reyArr, boardSize);
            }
        }
        else if (xDir && coord[1] == 0) // if the first column, also send recursions to the other direction
        {
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0];
            nextCoord[1] = coord[1] + 1;
            if (tiles[coord[0], coord[1]].tileProp.powerUp == 1)
                currTile.tileProp.currentColor = tiles[nextCoord[0], nextCoord[1]].tileProp.currentColor;
            if (nextCoord[1] < boardSize) // if the target is not out of bounds
            {
                bool[,] rexArr = MatchScript(tiles, boardSize, !xDir, 0, currTile.tileProp.currentColor, nextCoord);
                for (int row = 0; row < boardSize; row++)
                    for (int col = 0; col < boardSize; col++)
                    {
                        if (rexArr[col, row] && toDestroy[col, row] && tiles[col,row].tileProp.currentColor >= TileColors.empty)
                        {
                            int iCount = IntersectionCounter(tiles, AddSlots(toDestroy, rexArr, boardSize), boardSize, new int[] { col, row });
                            if (iCount >= 7)
                                tiles[col, row].tileProp.powerUp = 5;
                            else if (iCount > 5)
                                tiles[col, row].tileProp.powerUp = 4;
                            else if(iCount == 5)
                                tiles[col, row].tileProp.powerUp = 3;
                            if(iCount >= 5)
                                tiles[col, row].tileProp.newPower = true;
                        }
                    }
                toDestroy = AddSlots(toDestroy, rexArr, boardSize);
            }
        }
        return toDestroy;
    }
    public static bool[,] MatchScript(GridTile[,] tiles, int boardSize) // for initial call
    {
        int[] coord = new int[2];
        coord[0] = 0;
        coord[1] = 0;
        return MatchScript(tiles, boardSize, true, 0, (TileColors)(-1), coord);
    }
    static bool[,] SetupMatchScript(DataHolder[,] tiles, int boardSize, bool xDir, int prog, DataHolder prevTile, int[] coord) // used in Reshuffle
    {
        bool[,] toDestroy = new bool[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                toDestroy[col, row] = false;
            }
        }
        DataHolder currTile = tiles[coord[0], coord[1]];
        if (!xDir) // check if in yDir, bool set to true if you are checking in the x direction through this branch
        {
            if ((currTile.currentColor == prevTile.currentColor && prevTile.currentColor > TileColors.empty) || currTile.powerUp == 1 || prevTile.powerUp == 1)
            {
                prog++;
            }
            else if (prog >= 2) // if 3 or more of the same color are in a row
            {
                for (int i = 0; i <= prog; i++) // add them to the destroy array
                {
                    toDestroy[coord[0], coord[1] - i - 1] = true;
                }
                prog = 0; // reset because it's a different color
            }
            else // if it's a different color but not enough matches
            {
                prog = 0;
            }
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0];
            nextCoord[1] = coord[1] + 1;
            if (nextCoord[1] < boardSize) // if the target is not out of bounds
            {
                bool[,] rexArr = SetupMatchScript(tiles, boardSize, xDir, prog, currTile, nextCoord);
                toDestroy = AddSlots(toDestroy, rexArr, boardSize);
            }
            else if (prog >= 2) // exception for if the piece is at the edge of the board to force the connected tiles to the destroy box
            {
                for (int i = 0; i <= prog; i++) // add them to the destroy array
                {
                    toDestroy[coord[0], coord[1] - i] = true;
                }
                prog = 0; // reset because it's a different color
            }
        }
        else // if checking the xDir
        {
            if ((currTile.currentColor == prevTile.currentColor && prevTile.currentColor > TileColors.empty) || currTile.powerUp == 1 || prevTile.powerUp == 1)
            {
                prog++;
            }
            else if (prog >= 2) // if 3 or more of the same color are in a column
            {
                for (int i = 0; i <= prog; i++) // add them to the destroy array
                {
                    toDestroy[coord[0] - i - 1, coord[1]] = true;
                }
                prog = 0; // reset because it's a different color
            }
            else // if it's a different color but not enough matches
            {
                prog = 0;
            }
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0] + 1;
            nextCoord[1] = coord[1];
            if (nextCoord[0] < boardSize) // if the target is not out of bounds
            {
                bool[,] reyArr = SetupMatchScript(tiles, boardSize, xDir, prog, currTile, nextCoord);
                toDestroy = AddSlots(toDestroy, reyArr, boardSize);
            }
            else if (prog >= 2) // exception for if the piece is at the edge of the board to force the connected tiles to the destroy box
            {
                for (int i = 0; i <= prog; i++) // add them to the destroy array
                {
                    toDestroy[coord[0] - i, coord[1]] = true;
                }
                prog = 0; // reset because it's a different color
            }
        }
        if (!xDir && coord[0] == 0) // if the first row, also send recursions to the other direction
        {
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0] + 1;
            nextCoord[1] = coord[1];
            if (nextCoord[0] < boardSize) // if the target is not out of bounds
            {
                bool[,] reyArr = SetupMatchScript(tiles, boardSize, !xDir, 0, currTile, nextCoord);
                toDestroy = AddSlots(toDestroy, reyArr, boardSize);
            }
        }
        else if (xDir && coord[1] == 0) // if the first column, also send recursions to the other direction
        {
            int[] nextCoord = new int[coord.Length];
            nextCoord[0] = coord[0];
            nextCoord[1] = coord[1] + 1;
            if (nextCoord[1] < boardSize) // if the target is not out of bounds
            {
                bool[,] rexArr = SetupMatchScript(tiles, boardSize, !xDir, 0, currTile, nextCoord);
                toDestroy = AddSlots(toDestroy, rexArr, boardSize);
            }
        }
        return toDestroy;
    }
    public static bool[,] SetupMatchScript(DataHolder[,] tiles, int boardSize) // for initial call in Reshuffle
    {
        int[] coord = new int[2];
        coord[0] = 0;
        coord[1] = 0;
        DataHolder empty = new DataHolder();
        empty.currentColor = TileColors.empty;
        empty.powerUp = 0;
        return SetupMatchScript(tiles, boardSize, true, 0, empty, coord);
    }
}