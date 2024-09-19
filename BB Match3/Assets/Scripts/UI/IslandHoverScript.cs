using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandHoverScript : MonoBehaviour
{

    public float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float ypos = (float)Mathf.Sin(time * 2f) * .2f;
        transform.localPosition = new(transform.localPosition.x, transform.localPosition.y + ypos, transform.localPosition.z);
    }
}
