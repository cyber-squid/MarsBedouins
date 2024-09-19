using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Conditions
{
    public int winCon = 0;
    public int loseCon;

    // change this to be called from gridgen at end of func.
    public void SetLoseCon(int l)
    {
        loseCon = l;
    }

    public void SetScore()
    {
        winCon++;
    }

    public void SetCount(int l)
    {
        loseCon += l;
        Debug.Log("lose " + loseCon);

        //if (counter != null) { counter.UpdateCounter(this); }
    }
}