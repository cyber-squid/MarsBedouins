using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ResourceBar : MonoBehaviour
{
    public Image bar;
    public int maximum;
    public int current;

    // Start is called before the first frame update
    void Start()
    {
        GridGenerator.con.winCon = 0;
    }

    // Update is called once per frame
    void Update()
    {
        current = GridGenerator.con.winCon;
        float fillamount = (float)current / (float)GridGenerator.toWin;
        bar.fillAmount = fillamount;
    }
}
