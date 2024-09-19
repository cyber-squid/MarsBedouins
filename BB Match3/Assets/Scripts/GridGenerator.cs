using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridGenerator : MonoBehaviour
{
    public GridTile tilePrefab;
    public Sprite obstacleTile;
    private TileSelector selector;
    public GameObject holder;
    public static int levelNum = 0;
    public int currentLevelNum;
    //statics
    public static float tileSize;
    public static int boardSize; // X*X grid
    public static int colorNum = 5; // number of colors
    //status checks
    public static bool hint = false;
    public static int move = 0;
    public static int combo = 0;
    private static bool m1 = false;
    private static bool tileChange = false;
    public static bool[] stagger;
    private bool swapped = false;
    private bool gameEnd = false;
    static bool matched = false;
    static bool unswapped = false;
    public static int swapEnd = 1;
    public static int toWin;
    private int toLose;
    public static GridTile solution = null;
    //definitions
    private static DataHolder[,] gridDB;
    private static GridTile[,] tiles;
    private static bool[,] matches;
    private static bool[,] PUDestroy;
    //private LevelSystem levels = new LevelSystem();
    private static int[,] levelLayout;

    public static Conditions con = new Conditions();
    static MoveCounter counter; 
    GridTile swapStartTile;
    GridTile swapEndTile;

    public static SFXaudio sfx;

    //Family
    public GameObject a;
    public GameObject b;
    public GameObject c;
    public GameObject d;
    public GameObject e;
    public static GameObject boy;
    public static GameObject dad;
    public static GameObject mom;
    public static GameObject girl;
    public static GameObject grandma;

    public Camera cam;
    private static float shake = 0;

    // Start is called before the first frame update
    void Awake()
    {
        boy = a; dad = b; mom = c; girl = d; grandma = e;
        sfx = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<SFXaudio>();
        LevelSystem.StartGame();
        boardSize = LevelSystem.GetBoardSize(levelNum);
        toWin = LevelSystem.GetWinCon(levelNum);
        toLose = LevelSystem.GetLosCon(levelNum);
        levelLayout = new int[boardSize, boardSize];
        if (levelNum == 20)
        {
            LevelSystem.ResetLayout(levelNum);
            LevelSystem.GetLayout(levelNum, 2);
            LevelSystem.GetLayout(levelNum);
        }
        levelLayout = LevelSystem.GetLevel(levelNum);

        gridDB = new DataHolder[boardSize, boardSize];
        tiles = new GridTile[boardSize, boardSize];
        matches = new bool[boardSize, boardSize];
        PUDestroy = new bool[boardSize, boardSize];
        stagger = new bool[boardSize];

        con.SetLoseCon(toLose);
        tileSize = 0.5f/((boardSize/6)); 
        selector = gameObject.GetComponent<TileSelector>();
        if (selector == null)
        selector = gameObject.AddComponent<TileSelector>();

        counter = GetComponent<MoveCounter>();
        counter.UpdateCounter(con);
        counter.UpdateLevel(levelNum);
        selector.Initialise(this);
        DataHolder.normalTileSprites = DataHolder.InitSpriteList("Normal Tiles");
        DataHolder.highlightTileSprites = DataHolder.InitSpriteList("Highlight Tiles");
        DataHolder.powerupTileSprites = DataHolder.InitSpriteList("Powerup Tiles");
        DataHolder.powerupHighlightTileSprites = DataHolder.InitSpriteList("Highlight Powerup Tiles");
        DataHolder.obstacleTile = DataHolder.InitSpriteList("Obstacles")[0];




        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                gridDB[col, row] = new DataHolder();
                int[] pos = new int[] { col, row };
                tiles[col, row] = Instantiate(tilePrefab, new Vector3(0,0,0), Quaternion.identity, holder.transform);
                tiles[col, row].transform.localScale = new Vector3(tileSize,tileSize, 0.0f);
                tiles[col, row].transform.position = new Vector3(col*tileSize - ((boardSize-1)*tileSize / 2.0f), row* tileSize - (4*tileSize+(boardSize-1)*tileSize / 2.0f), 0.0f);
                if (levelLayout[col, row] != 1)
                {
                    tiles[col, row].MakeOrb();
                }
            }
        }
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                gridDB[col, row].SetColor(-1 * levelLayout[col, row] - 1);
            }
        }
        GenerateScript();

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                tiles[col, row].SetUpBackground();
            }
        }
        solution = Reshuffle.ReShuffleScript(gridDB, tiles, boardSize);
    }
    private float shakeAmount = 0.05f;
    // Update is called once per frame
    void Update()
    {
        if (shake > 0)
        {
            Vector3 shakeVector = UnityEngine.Random.insideUnitSphere * shakeAmount;
            Camera.main.transform.position = new(shakeVector.x, shakeVector.y, -1);
            shake -= Time.deltaTime;
        } else {
            shake = 0f;
            Camera.main.transform.position = new(0,0,-1);
        }
        if(move < 0)
            move = 0;
        if (move == 0 && tileChange && swapEnd == 1 && !gameEnd)
        {
            if (!matched)
                ProcessMatch();
            if (move == 0 && swapEnd == 1 && matched)
            {
                if (m1)
                {
                    if (combo == 0)
                        sfx.PlaySFX(sfx.match);
                    else if (combo == 1)
                        sfx.PlaySFX(sfx.match01);
                    else if (combo == 2)
                        sfx.PlaySFX(sfx.match02);
                    else if (combo >= 3)
                        sfx.PlaySFX(sfx.match03);
                }
                ProcessGravity();
                counter.UpdateCounter(con);
            }
            if (move == 0 && swapped && !matched && swapEnd == 1)
            {
                ProcessUnswap();
            }
            if (move == 0 && swapEnd == 1)
            {
                solution = Reshuffle.ReShuffleScript(gridDB, tiles, boardSize);
                if (solution == null)
                {
                    for(int row = 0; row < boardSize; row++)
                    {
                        for(int col = 0; col < boardSize; col++)
                        {
                            StartCoroutine(tiles[col, row].Indicate(Mathf.PI / 12));
                        }
                    }
                    ProcessReshuffle();
                    unswapped = true;
                }
            }
            for (int col = 0; col < boardSize; col++)
            {
                for (int row = 0; row < boardSize; row++)
                {
                    // for updating any changes
                    tiles[col, row].tileProp = gridDB[col, row];
                    tiles[col, row].SetColor();
                    tiles[col, row].SetHighlight(false);
                }
            }
            swapped = false;
        }
        else if(move != 0 || swapEnd != 1)
            print("move: " + move + " swapEnd: " + swapEnd);
        if (!matched && !unswapped && swapEnd == 1 && move == 0)
        {
            tileChange = false;
        }
        else if (swapEnd == 1 && move == 0)
        {
            if(Indicator.waiting > 0) Indicator.waiting = 10;
            unswapped = false;
            matched = false;
        }
        if (swapEnd == 1 && !tileChange && move == 0 && !gameEnd) // if there are no swaps currently happening, check for win/loss conditions
        {
            if (con.winCon >= toWin)
                ProcessWin();
            else if (con.loseCon <= 0)
                ProcessLoss();
        }
    }
    public void ProcessMatch()
    {
        matches = Match.MatchScript(tiles, boardSize);
        move++;
        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize; row++)
            {
                // for matching
                if (matches[col, row] || PUDestroy[col,row])
                {
                    if (gridDB[col, row].powerUp == 1 && (!gridDB[col,row].newPower || gridDB[col, row].wasPower))
                    {
                        sfx.PlaySFX(sfx.p1);
                        if (combo > 0)
                            sfx.PlaySFX(sfx.p102);
                        if (combo > 1)
                            sfx.PlaySFX(sfx.p103);
                        if (combo > 2)
                            sfx.PlaySFX(sfx.p104);
                    }
                    con.SetScore();
                    shake = 0.2f;
                    m1 = true;
                    matched = true;
                    if (tiles[col, row].tileProp.powerUp < 1)
                        tiles[col, row].oldTileProp.currentColor = tiles[col, row].tileProp.currentColor;
                    else
                        tiles[col, row].oldTileProp.currentColor = TileColors.empty;
                    if (!gridDB[col, row].newPower)
                    {
                        gridDB[col, row].powerUp = 0;
                        gridDB[col, row].wasPower = false;
                    }
                    if (tiles[col, row].proc)
                    {
                        move++;
                        tiles[col, row].postproc = true;
                    }
                    tiles[col, row].oldPosition = tiles[col, row].transform.position;
                    gridDB[col, row].SetColor(-1);
                    matches[col, row] = false;
                    PUDestroy[col,row] = false;

                    tiles[col, row].SetHighlight(false);
                    tiles[col, row].proc = false;
                    tiles[col, row].preproc = false;
                }
                if (tiles[col, row].preproc)
                {
                    tiles[col, row].preproc = false;
                    tiles[col, row].proc = true;
                    tiles[col, row].SetHighlight(true);
                }
                if (gridDB[col, row].newPower)
                {
                    gridDB[col, row].newPower = false;
                    gridDB[col, row].wasPower = true;
                }
            }
        }
        move--;
        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize; row++)
            {
                // for matching
                if (tiles[col, row].proc && !gridDB[col, row].newPower)
                {
                    ProcessPowerUp(tiles[col, row], null);
                    PUDestroy[col, row] = true;
                    tiles[col, row].tileProp = gridDB[col, row];
                    matched = true;
                }
            }
        }
    }
    public void ProcessGravity()
    {
        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize; row++)
            {
                int newRow = Gravity.GravityScriptA(boardSize, gridDB, tiles, colorNum, tiles[col, row]);
                if (newRow == row)
                {
                    continue;
                }
                else if (newRow < 0 && gridDB[col, row].currentColor != TileColors.invisibleObstacle)
                {
                    Gravity.RegenerateScriptA(gridDB, colorNum, tiles[col, row]);
                    tiles[col, row].tileProp = gridDB[col, row];
                    Vector3 dropPos = tiles[col, row].transform.position;
                    StartCoroutine(tiles[col, row].MoveTile(dropPos));
                }
                else
                {
                    if (gridDB[col, row].currentColor != TileColors.invisibleObstacle)
                    {
                        ProcessGravityA(tiles[col, row], tiles[col, newRow]);
                    }
                }
            }
        }
        if (m1)
        {
            if (combo == 0)
                sfx.PlaySFX(sfx.gravity);
            else if (combo == 1)
                sfx.PlaySFX(sfx.gravity01);
            else if (combo == 2)
                sfx.PlaySFX(sfx.gravity02);
            else if (combo >= 3)
                sfx.PlaySFX(sfx.gravity03);
            m1 = false;
            combo++;
        }
    }
    public IEnumerator Hinter()
    {
        yield return new WaitForSeconds(10f);
        if (solution != null && !hint)
        {
            hint = true;
            StartCoroutine(solution.Indicate());
        }
        StartCoroutine(Hinter());
    }
    public void ProcessUnswap()
    {
        ProcessSwap(swapStartTile, swapEndTile); // unswap the swapped tiles
        con.SetCount(2); // regenerate the moves lost from the swap and the unswap
        unswapped = true; // make sure the tileChange doesn't turn off
        sfx.PlaySFX(sfx.deny);
    }
    public void ProcessReshuffle()
    {
        // set it up for reset by setting all tiles empty, then pass the board data to regeneratescript, which will set random color for every empty tile
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (gridDB[col, row].currentColor > TileColors.empty)
                    gridDB[col, row].SetColor(-1);
            }
        }
        GenerateScript();
    }
    public void ProcessWin()
    {
        gameEnd = true;
        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize; row++)
            {
                // for updating any changes
                if (gridDB[col, row].currentColor != TileColors.invisibleObstacle)
                {
                    gridDB[col, row].SetColor((row + col) % colorNum);
                    gridDB[col, row].powerUp = 0;
                    tiles[col, row].tileProp = gridDB[col, row];
                    tiles[col, row].SetColor();
                    tiles[col, row].SetHighlight(false);
                }
            }
        }
        WinAndLose.Instance.win();
    }
    public void ProcessLoss()
    {
        gameEnd = true;
        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize; row++)
            {
                // for updating any changes
                gridDB[col, row].SetColor(-1);
                gridDB[col, row].powerUp = 0;
                tiles[col, row].tileProp = gridDB[col, row];
                tiles[col, row].SetColor();
                tiles[col, row].SetHighlight(false);
            }
        }
        WinAndLose.Instance.lose();
    }
    public void ProcessPowerUp(GridTile tile, GridTile target)
    {
        if (tile.tileProp.powerUp < 2)
            return;
        tileChange = true;
        bool[,] toDestroy = new bool[boardSize, boardSize];
        switch (tile.tileProp.powerUp)
        {
            case 2:
                bool xDir = false;
                bool pDir = false;
                if (target != null)
                {
                    if (target.pos[0] != tile.pos[0])
                        xDir = true;
                    if (target.pos[0] > tile.pos[0] || target.pos[1] > tile.pos[1])
                        pDir = true;
                }
                else
                    xDir = UnityEngine.Random.Range(0,2) != 0;
                toDestroy = PowerUp.RowColTrigger(new int[] { tile.pos[0], tile.pos[1] }, xDir, boardSize, gridDB);
                StartCoroutine(tile.PUOrb((xDir ? 0 : 1)+(pDir ? 0 : 2)));
                break;
            case 3:
                toDestroy = PowerUp.BombTrigger(new int[] { tile.pos[0], tile.pos[1] }, 2, boardSize, gridDB);
                toDestroy = Match.AddSlots(toDestroy, PowerUp.BishopTrigger(new int[] { tile.pos[0], tile.pos[1] }, 2, boardSize, gridDB),boardSize);
                StartCoroutine(tile.PUOrb(0));
                break;
            case 4:
                toDestroy = PowerUp.BombTrigger(new int[] { tile.pos[0], tile.pos[1] }, 4, boardSize, gridDB);
                StartCoroutine(tile.PUOrb(0));
                break;
            case 5:
                TileColors currColor = TileColors.empty;
                if(target != null)
                    currColor = target.tileProp.currentColor;
                if (currColor <= TileColors.empty)
                    currColor = PowerUp.GetMaxColor(colorNum, boardSize, gridDB);
                toDestroy = PowerUp.ColorTrigger(currColor, colorNum, boardSize, gridDB);
                StartCoroutine(tile.PUOrb((int)currColor, tiles, toDestroy));
                break;
            default:
                move--;
                break;
        }
        // destroy self to stop endless recursion
        m1 = true;
        matched = true;
        tiles[tile.pos[0], tile.pos[1]].proc = true;
        PUDestroy[tile.pos[0],tile.pos[1]] = true;
        for (int col = 0; col < boardSize; col++)
        {
            for (int row = 0; row < boardSize; row++)
            {
                if (toDestroy[col, row] && tiles[col, row].tileProp.currentColor != TileColors.invisibleObstacle)
                {
                    if (tiles[col, row].tileProp.powerUp > 1)
                    {
                        if (!PUDestroy[col, row])
                            tiles[col, row].preproc = true;
                    }
                    else
                        PUDestroy[col,row] = true;
                }
            }
        }
    }
    
    // called from tileselector when the player taps on two adjacent tiles. 
    public void ProcessSwap(GridTile startTile, GridTile targetTile)
    {
        if (gameEnd || swapEnd != 1)
            return;
        tileChange = true;
        // if the tiles are adjacent to each other (should have an adjacency check func)
        combo = 0;
        if (Swap.SwapAdjacentCheck(startTile, targetTile, tilePrefab.transform.localScale.x, tilePrefab.transform.localScale.y))
        {
            con.SetCount(-1);
            Swap.SwapScript(tiles, gridDB, startTile, targetTile);
            swapStartTile = startTile;
            swapEndTile = targetTile;
            swapped = true;
            StartCoroutine(startTile.SwapMoveTile(targetTile));
            StartCoroutine(targetTile.SwapMoveTile(startTile));
        }
        if (startTile.tileProp.powerUp > 1 || targetTile.tileProp.powerUp > 1)
        {
            ProcessPowerUp(startTile, targetTile);
            ProcessPowerUp(targetTile, startTile);
        }
    }
    public void ProcessPUP(GridTile tile)
    {
        combo = 0;
        ProcessPowerUp(tile, null);
    }

    public void ProcessGravityA(GridTile startTile, GridTile targetTile)
    {
        Swap.SwapScript(tiles, gridDB, startTile, targetTile);
        StartCoroutine(targetTile.MoveTile(startTile));
        startTile.transform.position = targetTile.transform.position;
    }
    public GridTile GetGridTile(int x, int y)
    {
        return tiles[x, y];
    }
    public void GenerateScript()
    {
        Gravity.RegenerateScript(boardSize, gridDB, colorNum); // set random colors to the database
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                int[] pos = new int[] { col, row };
                tiles[col, row].SetDH(gridDB[col, row], pos); // initialise dataholders
            }
        }
        bool matched = true;
        while(matched)
        {
            matched = false; 
            bool[,] matches = Match.SetupMatchScript(gridDB, boardSize);
            for(int row = 0; row < boardSize; row++)
            {
                for(int col = 0; col < boardSize; col++)
                {
                    if(matches[col, row])
                    {
                        matched = true;
                        gridDB[col, row].SetColor(-1);
                    }
                }
            }
            Gravity.RegenerateScript(boardSize, gridDB, colorNum);
        }
        tileChange = true;
    }
}