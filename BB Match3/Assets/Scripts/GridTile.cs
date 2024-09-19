using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GridTile : MonoBehaviour
{
    public DataHolder oldTileProp = new DataHolder();
    public Vector3 oldPosition = Vector3.zero;
    public DataHolder tileProp;
    public int[] pos;
    public GameObject tileBackground; // the "boxing" to be added behind the tiles.
    GameObject orb = null;
    public GameObject orbPrefab;
    public GameObject ResourceBar;
    public bool proc = false;
    public bool preproc = false;
    public bool postproc = false;
    private int wait = 0;
    private bool stagger = false;
    //public Vector3 dropPos = Vector3.zero;
    //public bool drop = false;
    public void MakeOrb()
    {
        orb = GameObject.Instantiate(orbPrefab);
        orb.transform.position = this.transform.position;
        orb.transform.localScale = new Vector3(GridGenerator.tileSize*0.25f, GridGenerator.tileSize*0.25f, GridGenerator.tileSize * 0.25f);
        SpriteRenderer renderer = orb.GetComponent<SpriteRenderer>();
        Color orbColor = Color.white;
        orbColor.a = 0.4f;
        renderer.color = orbColor;
        renderer.enabled = false;
    }
    public GameObject MakePUOrb()
    {
        GameObject puOrb;
        puOrb = GameObject.Instantiate(orbPrefab);
        puOrb.transform.position = this.transform.position;
        puOrb.transform.localScale = new Vector3(GridGenerator.tileSize * 0.25f, GridGenerator.tileSize * 0.25f, GridGenerator.tileSize * 0.25f);
        SpriteRenderer renderer = puOrb.GetComponent<SpriteRenderer>();
        Color orbColor = Color.white;
        orbColor.a = 0.4f;
        renderer.color = orbColor;
        renderer.enabled = false;
        return puOrb;
    }
    private void ResetOrb()
    {
        SpriteRenderer renderer = orb.GetComponent<SpriteRenderer>();
        Color orbColor = Color.white;
        orbColor.a = 0.4f;
        renderer.color = orbColor;
        renderer.enabled = false;
    }


    public void SetUpBackground()
    {
        if (tileProp.currentColor != TileColors.invisibleObstacle)
        {
            Instantiate(tileBackground, transform.position, Quaternion.identity);
        }
    }

    public void SetHighlight(bool isHighlighted)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if ((isHighlighted || proc) && tileProp.currentColor >= TileColors.empty)
        {
            if (tileProp.powerUp >= 1)
                tileProp.sprite = DataHolder.powerupHighlightTileSprites[(int)tileProp.powerUp%5];
            else if (tileProp.currentColor >= 0)
                tileProp.sprite = DataHolder.highlightTileSprites[(int)tileProp.currentColor];
        }
        else if (tileProp.currentColor >= 0)
        {
            if (tileProp.powerUp >= 1)
                tileProp.sprite = DataHolder.powerupTileSprites[(int)tileProp.powerUp%5];
            else
                tileProp.sprite = DataHolder.normalTileSprites[(int)tileProp.currentColor];
        }

        renderer.sprite = tileProp.sprite;
    }

    public void SetDH(DataHolder dh, int[] posInGrid)
    {
        this.tileProp = dh;
        pos = posInGrid;
        this.SetColor();
    }
    public IEnumerator UnsquashTile(Vector3 squashScale, Vector3 oldScale) // currently unused, for matching animation
    {
        float delta = 5;
        float t = 0f;
        while (t < 1.0f)
        {
            t = t + Time.deltaTime * delta;
            this.transform.localScale = Vector3.Lerp(squashScale, oldScale, t);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        this.transform.localScale = oldScale;
    }
    public IEnumerator MoveTile(GridTile that) // tile movement animation
    {
        GridGenerator.swapEnd--; // counter for checking if the animation ended
        while (GridGenerator.move != 0)
            yield return new WaitForSeconds(Time.deltaTime);
        Vector3 oldPos = this.transform.position;
        Vector3 newPos = that.transform.position;
        Vector3 bouncePos = newPos;
        bouncePos.y = bouncePos.y + 0.1f * GridGenerator.tileSize;
        Vector3 oldScale = new Vector3(GridGenerator.tileSize, GridGenerator.tileSize);
        Vector3 squashScale = new Vector3(oldScale.x * 1.05f, oldScale.y * 0.95f);
        Vector3 stretchScale = new Vector3(oldScale.x * 0.95f, oldScale.y * 1.05f);
        yield return new WaitForSeconds(Time.deltaTime);
        int yDiff = that.pos[1] - this.pos[1];
        float delta = 2.0f + Mathf.Min(GridGenerator.combo, 4) * 0.5f;
        float i = 0.0f;
        bool unstagger = false;
        if (yDiff > 1)
        {
            delta = delta * (1 + yDiff * 0.1f);
        }
        while(i < 1.0f)
        {
            if (GridGenerator.stagger[pos[0]] && !unstagger)
                yield return new WaitForSeconds(Time.deltaTime);
            else
            {
                if (!unstagger)
                    GridGenerator.stagger[pos[0]] = true;
                delta += 0.01f * (Time.deltaTime / 0.01f);
                i += Time.deltaTime * delta;
                this.transform.position = Vector3.Lerp(oldPos, newPos, i);
                this.transform.localScale = Vector3.Lerp(oldScale, squashScale, i);
                yield return new WaitForSeconds(Time.deltaTime);
                if (!unstagger)
                {
                    unstagger = true;
                }
                else if (GridGenerator.stagger[pos[0]])
                {
                    GridGenerator.stagger[pos[0]] = false;
                }
            }
        }
        i = 0;
        float t = 0;
        delta = 10;
        do // bounce code
        {
            i += Time.deltaTime * delta;
            t = -Mathf.Pow(i - 1, 2) + 1;
            yield return new WaitForSeconds(Time.deltaTime);
            this.transform.position = Vector3.Lerp(newPos, bouncePos, t);
            this.transform.localScale = Vector3.Lerp(squashScale, stretchScale, t);
        } while (t > 0.0f);
        StartCoroutine(UnsquashTile(squashScale,oldScale));
        this.transform.position = newPos;
        GridGenerator.swapEnd++; // recover counter to indicate end of this animation
    }

    public IEnumerator MoveTile(Vector3 dropPos) // tile movement animation
    {
        Vector3 newPos = dropPos;
        GridGenerator.swapEnd--; // counter for checking if the animation ended
        while (GridGenerator.move != 0)
            yield return new WaitForSeconds(Time.deltaTime*10);
        StartCoroutine(MoveOrb());
        Vector3 oldPos = new Vector3(this.transform.position.x, (4+pos[1])*0.5f*GridGenerator.tileSize, 0);
        Vector3 bouncePos = newPos;
        bouncePos.y = bouncePos.y + 0.1f*GridGenerator.tileSize;
        Vector3 oldScale = new Vector3(GridGenerator.tileSize, GridGenerator.tileSize);
        Vector3 squashScale = new Vector3(oldScale.x * 1.05f, oldScale.y * 0.95f);
        Vector3 stretchScale = new Vector3(oldScale.x * 0.95f, oldScale.y * 1.05f);
        yield return new WaitForSeconds(Time.deltaTime);
        float delta = 2.0f + Mathf.Min(GridGenerator.combo, 4) * 0.5f;
        float i = 0.0f;
        bool unstagger = false;
        while (i < 1.0f)
        {
            if (GridGenerator.stagger[pos[0]] && !unstagger)
                yield return new WaitForSeconds(Time.deltaTime);
            else
            {
                if (!unstagger)
                    GridGenerator.stagger[pos[0]] = true;
                delta += 0.01f * (Time.deltaTime / 0.01f);
                i += Time.deltaTime * delta;
                this.transform.position = Vector3.Lerp(oldPos, newPos, i);
                this.transform.localScale = Vector3.Lerp(oldScale, squashScale, i);
                yield return new WaitForSeconds(Time.deltaTime);
                if (!unstagger)
                {
                    unstagger = true;
                }
                else if(GridGenerator.stagger[pos[0]])
                {
                    GridGenerator.stagger[pos[0]] = false;
                }
            }
        }
        i = 0;
        float t = 0;
        delta = 10;
        do
        {
            i += Time.deltaTime * delta;
            t = -Mathf.Pow(i - 1, 2) + 1;
            yield return new WaitForSeconds(Time.deltaTime);
            this.transform.position = Vector3.Lerp(newPos, bouncePos, t);
            this.transform.localScale = Vector3.Lerp(squashScale, stretchScale, t);
        } while (t > 0.0f);
        this.transform.position = newPos;
        StartCoroutine(UnsquashTile(squashScale, oldScale));
        GridGenerator.swapEnd++; // recover counter to indicate end of this animation
    }
    public IEnumerator SwapMoveTile(GridTile that) // tile movement animation
    {
        GridGenerator.swapEnd--; // counter for checking if the animation ended
        while (GridGenerator.move != 0)
            yield return new WaitForSeconds(Time.deltaTime);
        Vector3 oldPos = this.transform.position;
        Vector3 newPos = that.transform.position;
        yield return new WaitForSeconds(Time.deltaTime);
        int yDiff = that.pos[1] - this.pos[1];
        float delta = 7;
        float i = 0.0f;
        while (i < 1.0f)
        {
            i += Time.deltaTime * delta;
            if (i > 1.0f)
                break;
            yield return new WaitForSeconds(Time.deltaTime);
            this.transform.position = Vector3.Lerp(oldPos, newPos, i);
        }
        this.transform.position = newPos;
        GridGenerator.swapEnd++; // recover counter to indicate end of this animation
    }
    private void StrikeAPose(TileColors code)
    {
        GameObject familyMember = null;
        switch (code)
        {
            case (TileColors.blueDrink):
                familyMember = GridGenerator.boy;
                break;
            case (TileColors.purpleFun):
                familyMember = GridGenerator.mom;
                break;
            case (TileColors.yellowFood):
                familyMember = GridGenerator.grandma;
                break;
            case (TileColors.greenOxygen):
                familyMember = GridGenerator.dad;
                break;
            case (TileColors.fushciaTech):
                familyMember = GridGenerator.girl;
                break;
            default:
                break;
        }
        if (familyMember != null)
            familyMember.GetComponent<GoofyAhhAnimator>().Pose();
    }
    public IEnumerator MoveOrb() // tile movement animation
    {
        StrikeAPose(oldTileProp.currentColor);
        Color oldColor = ConvertColor(oldTileProp.currentColor);
        Vector3 oldPos = oldPosition;
        Vector3 newPos = Vector3.zero;//ResourceBar.transform.position;
        newPos.y = newPos.y+1.5f*GridGenerator.tileSize;
        Vector3 oldScale = new Vector3(GridGenerator.tileSize * 0.25f, GridGenerator.tileSize * 0.25f);
        Vector3 newScale = new Vector3(GridGenerator.tileSize * 0.15f, GridGenerator.tileSize * 0.35f);
        orb.transform.rotation = Quaternion.LookRotation(Vector3.forward,newPos - oldPos);
        orb.transform.position = oldPos;
        oldColor.a = 0.8f;
        Color newColor = oldColor;
        newColor.a = 0f;
        SpriteRenderer renderer = orb.GetComponent<SpriteRenderer>();
        renderer.color = oldColor;
        renderer.enabled = true;
        yield return new WaitForSeconds(Time.deltaTime);
        float delta = 1 + UnityEngine.Random.Range(-5,6)*0.01f;
        delta = delta + 0.04f * Mathf.Pow(pos[1], 2) + 0.01f * Mathf.Abs(pos[0]-Mathf.Floor((GridGenerator.boardSize+1)/2));
        float i = 0.0f;
        float t = 0.0f;
        while (i < 1.0f)
        {
            delta = delta + 0.01f + UnityEngine.Random.Range(-1, 2) * 0.005f;
            i += Time.deltaTime * delta;
            t = Mathf.Pow(1.0001f, 1+i) - 1;
            yield return new WaitForSeconds(Time.deltaTime);
            orb.transform.position = Vector3.Lerp(oldPos, newPos, i);
            orb.transform.localScale = Vector3.Lerp(oldScale, newScale, i);
            renderer.color = Color.Lerp(oldColor, newColor, i);
        } 
        ResetOrb();
    }
    public IEnumerator PUOrb(int v)
    {
        GameObject puOrb = MakePUOrb();
        int pu = tileProp.powerUp;
        if(pu == 4 && v == 0)
            StartCoroutine(PUOrb(1));
        while (!postproc)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Vector3 oldScale = new Vector3(GridGenerator.tileSize * 0.5f, GridGenerator.tileSize * 0.5f);
        Vector3 newScale = new Vector3(GridGenerator.tileSize * 0.5f, GridGenerator.tileSize * 0.5f);
        Color orbColor = Color.white;
        orbColor.a = 0.7f;
        Color newColor = Color.white;
        newColor.a = 0.7f;
        float delta = 3;
        bool animate = true;
        Vector3 oldPos = this.transform.position;
        Vector3 newPos = this.transform.position;
        switch (pu)
        {
            case 2:
                GridGenerator.sfx.PlaySFX(GridGenerator.sfx.p2);
                switch (v)
                {
                    case 0:
                        oldPos.x += GridGenerator.tileSize * (GridGenerator.boardSize - pos[0]);
                        newPos.x -= GridGenerator.tileSize * pos[0];
                        break;
                    case 1:
                        oldPos.y += GridGenerator.tileSize * (GridGenerator.boardSize - pos[1]);
                        newPos.y = newPos.y - (GridGenerator.tileSize * (pos[1] + 2));
                        break;
                    case 2:
                        newPos.x += GridGenerator.tileSize * (GridGenerator.boardSize - pos[0]);
                        oldPos.x -= GridGenerator.tileSize * pos[0];
                        break;
                    case 3:
                        newPos.y += GridGenerator.tileSize * (GridGenerator.boardSize - pos[1]);
                        oldPos.y = oldPos.y - (GridGenerator.tileSize * (pos[1] + 2));
                        break;
                }
                orbColor.r = 0.4f;
                orbColor.g = 0.4f;
                orbColor.b = 0.4f;
                orbColor.a = 1.0f;
                newColor = orbColor;
                newScale = new Vector3(GridGenerator.tileSize * 0.35f, GridGenerator.tileSize * 0.7f);
                v = 0;
                break;
            case 3:
                GridGenerator.sfx.PlaySFX(GridGenerator.sfx.p3);
                newScale = new Vector3(GridGenerator.tileSize * 2f, GridGenerator.tileSize * 2f);
                delta += 3;
                newColor.a = 0.1f;
                v = 0;
                break;
            case 4:
                delta += 2;
                if(v==0)
                {
                    GridGenerator.sfx.PlaySFX(GridGenerator.sfx.p4);
                    oldScale = new Vector3(GridGenerator.tileSize * 5f, GridGenerator.tileSize * 5f);
                    newScale = new Vector3(GridGenerator.tileSize * 3f, GridGenerator.tileSize * 3f);
                    orbColor = Color.black;
                    orbColor.a = 0.5f;
                    newColor = orbColor;
                }
                else
                {
                    oldScale = new Vector3(GridGenerator.tileSize * 2.5f, GridGenerator.tileSize * 2.5f);
                    newScale = new Vector3(GridGenerator.tileSize * 2f, GridGenerator.tileSize * 3.5f);
                    orbColor = Color.black;
                    orbColor.r = 0.4f;
                    orbColor.g = 0.2f;
                    newColor = orbColor;
                    oldPos = new Vector3(5f,5f);
                }
                break;
            default:
                animate = false;
                break;
        }
        puOrb.transform.rotation = Quaternion.LookRotation(Vector3.forward, newPos - oldPos);
        puOrb.transform.position = oldPos;
        puOrb.transform.localScale = oldScale;
        SpriteRenderer renderer = puOrb.GetComponent<SpriteRenderer>();
        renderer.color = orbColor;
        float i = 0.0f;
        if(animate)
            renderer.enabled = true;
        bool moved = false;
        while (animate && i < 1.0f)
        {
            delta = delta + 0.02f;
            i += Time.deltaTime * delta;
            yield return new WaitForSeconds(Time.deltaTime);
            puOrb.transform.position = Vector3.Lerp(oldPos, newPos, i);
            puOrb.transform.localScale = Vector3.Lerp(oldScale, newScale, i);
            renderer.color = Color.Lerp(orbColor, newColor, i);
            if (v == 0 && i > 0.8f && !moved)
            {
                GridGenerator.move--;
                moved = true;
            }
        }
        if(v == 0 && !moved)
            GridGenerator.move--;
        postproc = false;
        Destroy(puOrb);
    }
    public IEnumerator ColorTile() // currently unused, for matching animation
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        GridGenerator.swapEnd--;
        yield return new WaitForSeconds(Time.deltaTime * 3);
        Color c = renderer.color;
        float i = 0f;
        while(i < 1f)
        {
            i+= Time.deltaTime * 3;
            c.a = i;
            renderer.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        GridGenerator.swapEnd++;
    }
    public IEnumerator PUOrb(int v, GridTile[,] tiles, bool[,] targets)
    {
        int pu = tileProp.powerUp;
        while (!postproc)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //GridGenerator.sfx.PlaySFX(GridGenerator.sfx.p5);
        PULaser(v, tiles, targets);
        yield return new WaitForSeconds(Time.deltaTime*10);
        while (wait != 0)
            yield return new WaitForSeconds(Time.deltaTime*3);
        GridGenerator.move--;
        postproc = false;
    }
    private void PULaser(int color, GridTile[,] tiles, bool[,] targets)
    {
        for (int row = 0; row < GridGenerator.boardSize; row++)
        {
            for(int col = 0; col < GridGenerator.boardSize; col++)
            {
                if (targets[col,row])
                {
                    wait++;
                    StartCoroutine(ShootLaser(color, tiles[col, row]));
                }
            }
        }
    }
    private IEnumerator ShootLaser(int color, GridTile that)
    {
        stagger = true;
        GameObject puOrb = MakePUOrb();
        Vector3 oldPos = this.transform.position;
        Vector3 newPos = that.transform.position;
        Vector3 oldScale = new Vector3(GridGenerator.tileSize * 0.3f, GridGenerator.tileSize * 0.5f);
        Vector3 newScale = new Vector3(GridGenerator.tileSize * 0.05f, GridGenerator.tileSize * 1f);
        Color orbColor = ConvertColor((TileColors)color);
        orbColor.a = 0.8f;
        float delta = 2f + UnityEngine.Random.Range(-5,6)*0.1f;
        puOrb.transform.rotation = Quaternion.LookRotation(Vector3.forward, newPos - oldPos);
        puOrb.transform.position = oldPos;
        puOrb.transform.localScale = oldScale;
        SpriteRenderer renderer = puOrb.GetComponent<SpriteRenderer>();
        renderer.enabled = true;
        renderer.color = orbColor;
        float i = 0f;
        bool unstagger = false;
        while (i < 1.0f)
        {
            delta = delta + 0.1f + 0.01f* UnityEngine.Random.Range(-2, 3);
            i += Time.deltaTime * delta;
            yield return new WaitForSeconds(Time.deltaTime);
            puOrb.transform.position = Vector3.Lerp(oldPos, newPos, i);
            puOrb.transform.localScale = Vector3.Lerp(oldScale, newScale, i);
            if(!unstagger)
            {
                stagger = false;
                unstagger = true;
            }
        }
        wait--;
        Destroy(puOrb);
    }
    Color ConvertColor(TileColors code = TileColors.invisibleObstacle)
    {
        Color newColor = Color.white;
        if (code == TileColors.invisibleObstacle)
            code = tileProp.currentColor;

        switch (code)
        {
            case (TileColors.blueDrink):
                newColor.r = 86f / 255f;
                newColor.g = 126f / 255f;
                newColor.b = 255f / 255f;

                break;
            case (TileColors.purpleFun):
                newColor.r = 191f / 255f;
                newColor.g = 73f / 255f;
                newColor.b = 221f / 255f; 
                break;
            case (TileColors.yellowFood):
                newColor.r = 235f / 255f;
                newColor.g = 196f / 255f;
                newColor.b = 90f / 255f;
                break;
            case (TileColors.greenOxygen):
                newColor.r = 32f / 255f;
                newColor.g = 196f / 255f;
                newColor.b = 52f / 255f;
                break;
            case (TileColors.fushciaTech):
                newColor.r = 255f / 255f;
                newColor.g = 106f / 255f;
                newColor.b = 168f / 255f;
                break;
            case (TileColors.breakableObstacle):
                newColor.r = 255f / 255f;
                newColor.g = 116f / 255f;
                newColor.b = 128f / 255f;
                break;
            default:
                break;
        }
        return newColor;
    }
    public IEnumerator Indicate(float d = 0.5f * Mathf.PI)
    {
        Vector3 diag1 = new Vector3(0.3f, 1f);
        Vector3 diag2 = new Vector3(-0.3f, 1f);
        Quaternion rotation1 = Quaternion.LookRotation(Vector3.forward, diag1);
        Quaternion rotation2 = Quaternion.LookRotation(Vector3.forward, diag2);
        float i = 0f;
        float t = 0f;
        while ((GridGenerator.hint || d != 0.5f * Mathf.PI) && i <= d)
        {
            i += Time.deltaTime;
            t = 0.5f-Mathf.Sin(i*12)/2;
            yield return new WaitForSeconds(Time.deltaTime);
            this.transform.rotation = Quaternion.Lerp(rotation1, rotation2, t);
        }
        GridGenerator.hint = false;
        this.transform.rotation = Quaternion.identity;
    }
    public void SetColor()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if(tileProp.powerUp >= 1) // is a powerup
        {
            renderer.color = Color.white;
            tileProp.sprite = DataHolder.powerupTileSprites[(int)tileProp.powerUp%5];
            renderer.sprite = tileProp.sprite;
        }
        else if ((int)tileProp.currentColor >= 0) // normal tiles
        {
            renderer.color = Color.white;
            tileProp.sprite = DataHolder.normalTileSprites[(int)tileProp.currentColor];
            renderer.sprite = tileProp.sprite;
        }
        else if (tileProp.currentColor == TileColors.breakableObstacle) // breakable obstacles
        {
            renderer.color = Color.white;
            tileProp.sprite = DataHolder.obstacleTile;
            renderer.sprite = tileProp.sprite;
        }
        else // invisible (empty space)
        {
            Color alpha = renderer.color;
            alpha.a = 0.0f;
            renderer.color = alpha;
        }
    }
}
