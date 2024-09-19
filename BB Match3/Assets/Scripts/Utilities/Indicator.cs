using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public static int waiting = 0;
    void Update()
    {
        if(waiting == 0)
            StartCoroutine(Waiter());
    }
    private IEnumerator Waiter()
    {
        waiting = 10;
        while (waiting > 0)
        {
            yield return new WaitForSeconds(1f);
            waiting--;
        }
        if (GridGenerator.solution != null)
        {
            GridGenerator.hint = true;
            StartCoroutine(GridGenerator.solution.Indicate());
        }
    }
}
