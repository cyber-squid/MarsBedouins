using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI counterText; // to be set in inspector
    [SerializeField] TextMeshProUGUI levelText; // to be set in inspector
    public void UpdateCounter(Conditions con)
    {
        if (counterText != null) { counterText.text = con.loseCon.ToString(); }
    }
    public void UpdateLevel(int levelNum)
    {
        if (levelText != null) { levelText.text = levelNum<20?(levelNum+1).ToString():"INFINITY"; }
    }
}
