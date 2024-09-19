using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileColors{blueDrink, greenOxygen, yellowFood, purpleFun, fushciaTech, empty = -1, invisibleObstacle = -2, breakableObstacle = -3}

[System.Serializable]
public class DataHolder
{
    public static Sprite[] normalTileSprites;
    public static Sprite[] highlightTileSprites;
    public static Sprite[] powerupTileSprites;
    public static Sprite[] powerupHighlightTileSprites;
    public static Sprite obstacleTile;
    public Sprite sprite;

    public TileColors currentColor;
    public int powerUp = 0;
    public bool newPower = false;
    public bool wasPower = false;


    public static Sprite[] InitSpriteList(string pathName)
    {
        var allSprites = Resources.LoadAll(pathName, typeof(Sprite));
        Sprite[] listToInit = new Sprite[allSprites.Length];

        for (int i = 0; i < allSprites.Length; i++)
        {
            listToInit[i] = (Sprite)allSprites[i]; // get array of all sprites saved in resources folder.
        }

        return listToInit;
    }

    public void SetColor(int col)
    {
        currentColor = (TileColors)col;
    }
}
