using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class used to process tile tapping.
/// Holds reference to the current selected tile.
/// </summary>
public class TileSelector : MonoBehaviour
{
    GridTile tileWeAreCurrentlyOn;
    public GameObject gridGen;
    public GridTile clickingSelectedTile { get; private set; } // for other classes (eg tile matching) to use.
    GridTile savedTileForSwipe;
    GridTile lastClickedTile;
                                                              // should return null when no tile selected.

    GridTile swappingSelectedTile; // for swap setups
    GridGenerator generator;

    bool tileWasNulledOut;
    Vector2 startingFingerPos;


    public void Initialise(GridGenerator generator)
    {
        this.generator = generator;
    }

    void Update()
    {
        ProcessMobileInput();
    }

    void ProcessMobileInput()
    {
        if (Input.touchCount > 0 && GridGenerator.swapEnd == 1)
        {
            GridGenerator.hint = false;
            // touch status is equivalent to mouse click or button press
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) // update our current tile each frame.
                ClickCheck(touch); 

            if (touch.phase == TouchPhase.Moved)
                SwipeCheck(touch);

            // should check if the player clicked a tile after selecting an initial one.
            if (touch.phase == TouchPhase.Ended)
                FinishClickCheck(touch);
        }
    }


    // register the current tile that we're holding on to with our finger, for a sliding swap, or for a tap swap if lastClickedTile has a reference.
    void ClickCheck(Touch touch)
    {
        tileWeAreCurrentlyOn = GetCurrentTile(touch, false);

        if (tileWeAreCurrentlyOn == null) { tileWasNulledOut = true; return; }

        if (savedTileForSwipe == null)
            savedTileForSwipe = tileWeAreCurrentlyOn;

        if (lastClickedTile != null) // if we've already registered a tile by clicking
        {
            lastClickedTile.SetHighlight(false);
            // check here if the last clicked tile was close enough to what we have our finger on that we should swap
            if (lastClickedTile != tileWeAreCurrentlyOn)
                ProcessSelectionClick();
            else
                lastClickedTile = null;
        }
        else
        {
            lastClickedTile = tileWeAreCurrentlyOn;
            lastClickedTile.SetHighlight(true);
        }
    }

    void SwipeCheck(Touch touch)
    {
        if (savedTileForSwipe == null) return;

        if (tileWeAreCurrentlyOn == null) { savedTileForSwipe = null; return; }

        if (savedTileForSwipe != tileWeAreCurrentlyOn && !tileWasNulledOut)
        {
            CheckForValidSwap(savedTileForSwipe, tileWeAreCurrentlyOn);
            savedTileForSwipe = null;
        }
    }

    void FinishClickCheck(Touch touch)
    {
        if (tileWasNulledOut || tileWeAreCurrentlyOn == null) { tileWasNulledOut = false; return; }

        if (tileWeAreCurrentlyOn.tileProp.powerUp > 1)
        {
            generator.ProcessPUP(tileWeAreCurrentlyOn);
            GridGenerator.con.SetCount(-1);
            tileWeAreCurrentlyOn = null;

            //SetTileTintForSelection(Color.white, lastClickedTile);
            if (lastClickedTile != null)
            {
                lastClickedTile.SetHighlight(false);
                lastClickedTile = null;
            }
            return;
        }

        // set this as current selected tile.

        //GridTile actualCurrentTile = GetCurrentTile(touch, false);

        //if (actualCurrentTile == null) { tileWeAreCurrentlyOn = savedTileForSwipe = null; return; }

        tileWeAreCurrentlyOn = null;
        savedTileForSwipe = null;
    }


    void ProcessSelectionClick()
    {
        float distFromNewTile = Mathf.Abs(Vector2.Distance(lastClickedTile.transform.position, tileWeAreCurrentlyOn.transform.position));

        if (GridGenerator.tileSize >= distFromNewTile)
        {
            CheckForValidSwap(lastClickedTile, tileWeAreCurrentlyOn);
            lastClickedTile = null;
        }
        else // they probably clicked somewhere else to select another tile. highlight that one instead.
        {
            lastClickedTile = tileWeAreCurrentlyOn;
            lastClickedTile.SetHighlight(true); 
        }
    }


    void CheckForValidSwap(GridTile startingTile, GridTile endingTile)
    {
        if (startingTile.tileProp.currentColor >= TileColors.empty && 
            Swap.SwapAdjacentCheck(startingTile, endingTile, startingTile.transform.localScale.x, startingTile.transform.localScale.y))
        {
            // let GridGenerator handle the rest, and reset to default state.
            generator.ProcessSwap(startingTile, endingTile);

            tileWeAreCurrentlyOn = null;
            savedTileForSwipe = null; // make sure we return to default state!
        }
    }

    //should return null if nothing was clicked
    GridTile GetCurrentTile(Touch touch, bool setHighlight)
    {
        // check what was clicked by sending a ray out.
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);

        // if we get a successful hit, get the tile data. 
        // generally have to check if it's null first! also must not select empty or obstacle tiles.
        if (hit.collider == null)
            return null;


        GridTile newTile = hit.collider.GetComponent<GridTile>();
        if (newTile == null)
            return null;


        if (newTile.tileProp.currentColor >= TileColors.empty)
        {
            //if (setHighlight)
                //SetTileTintForSelection(Color.grey, newTile);

            return newTile;
        }
        else return null;
    }

    /*void CheckForSelectionClick(Touch touch)
    {
        // if we already have a tile by tapping on it,
        // tapping on it should unselect, and tapping on another should select it for swap.
        if (clickingSelectedTile != null)
        {
            GridTile newTile = GetCurrentTile(touch, true);
            if (newTile == null) { return; }


            if (newTile.tileProp.powerUp > 1)
            {
                GridGenerator.ProcessPowerUp(newTile, null);
                GridGenerator.con.SetCount(-1);
                newTile = null;
                clickingSelectedTile = null;
                return;
            }
            float distFromNewTile = Mathf.Abs(Vector2.Distance(clickingSelectedTile.transform.position, newTile.transform.position));

            if (newTile == clickingSelectedTile)
            {
                // they probably clicked same tile to deselect it, so reset the currentselectedtile.
                SetTileTintForSelection(Color.white, clickingSelectedTile);
                clickingSelectedTile = null;
            }
            // make sure the tile that was tapped isn't so far from the current selected tile that it wouldn't make sense to swap to it
            else if (GridGenerator.tileSize >= distFromNewTile)
            {
                //CheckForValidSwap(newTile, touch);
            }
            else
            {
                SetTileTintForSelection(Color.white, clickingSelectedTile);
                clickingSelectedTile = GetCurrentTile(touch, true);
            }
        }
        else
        {
            clickingSelectedTile = GetCurrentTile(touch, true);

            if (clickingSelectedTile == null)
                return;

            startingFingerPos = touch.position;

            if (clickingSelectedTile.tileProp.powerUp > 1)
            {
                GridGenerator.ProcessPowerUp(clickingSelectedTile, null);
                GridGenerator.con.SetCount(-1);
                clickingSelectedTile = null;
                return;
            }
        }
    }


    void CheckForSelectionSwipe(Touch touch)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);

        if (hit.collider == null) {
            SetTileTintForSelection(Color.white, swappingSelectedTile);
            swappingSelectedTile = null; // probably tried to drag off grid? which is invalid, so reset
        }

        GridTile newTile = hit.collider.GetComponent<GridTile>();

        if (newTile != null)
        {
            if (newTile == swappingSelectedTile) // if finger is still on the same tile
            {
                // leave the current tile as is, do not set tiletoswitchto yet. should add a highlight effect here
                CheckForSelectionClick(touch);
                swappingSelectedTile = null;
            }
            else // check which tile they dragged to
            {
                //CheckForValidSwap(newTile, touch);
            }
        }
        else
        {
            SetTileTintForSelection(Color.white, swappingSelectedTile);
            swappingSelectedTile = null; // probably tried to drag off grid? which is invalid, so reset
        }

        


    void SetTileTintForSelection(Color color, GridTile tile)
    {
        //tile.selected = true;
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        renderer.color = color;
    }

    }



    GridTile GetNearestTile(GridTile tileToSwitchFrom, Vector2 endingFingerPos)
    {
        // get the position of the finger where it lifted, calculate a vector that gives us the distance from where it touched,
        // use that to figure out which direction we're going, and get the grid tile respective to that direction.

        // ik that this is poorly written but there are more pressing things to do than make this less crappy, maybe i'll come back to it later

        bool movingHoriz = false;
        bool movingBack = false;

        Vector2 positionFromStart = endingFingerPos - startingFingerPos;
        float absPosFromStartX = Mathf.Abs(positionFromStart.x);
        float absPosFromStartY = Mathf.Abs(positionFromStart.y);

        if (absPosFromStartX > absPosFromStartY)
        {
            movingHoriz = true;
        }

        if ((movingHoriz && absPosFromStartX != positionFromStart.x) ^
            (!movingHoriz && absPosFromStartY != positionFromStart.y))
        {
            movingBack = true;
        }

        if (absPosFromStartX >= tileToSwitchFrom.transform.localScale.x ||
         absPosFromStartY >= tileToSwitchFrom.transform.localScale.y)
        {
            if (movingHoriz && movingBack)
            {
                return generator.GetGridTile(tileToSwitchFrom.pos[0] - 1, tileToSwitchFrom.pos[1]);
            }
            if (!movingHoriz && movingBack)
            {
                return generator.GetGridTile(tileToSwitchFrom.pos[0], tileToSwitchFrom.pos[1] - 1);
            }
            if (movingHoriz && !movingBack)
            {
                return generator.GetGridTile(tileToSwitchFrom.pos[0] + 1, tileToSwitchFrom.pos[1]);
            }
            if (!movingHoriz && !movingBack)
            {
                return generator.GetGridTile(tileToSwitchFrom.pos[0], tileToSwitchFrom.pos[1] + 1);
            }
        }

        return null;
    }*/
}
