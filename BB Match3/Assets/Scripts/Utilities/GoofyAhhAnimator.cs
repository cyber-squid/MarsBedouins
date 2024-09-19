using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoofyAhhAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] spritesToFlipThrough;
    SpriteRenderer spriteRenderer;
    float lerper = 1f;
    [SerializeField] float waitTimeToIterate;
    float timePassed;
    [SerializeField] int currentIteration;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        lerper += Time.deltaTime;
        if (timePassed > waitTimeToIterate)
        {
            Iterate();
            timePassed = 0;
        }
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, lerper);
    }

    void Iterate()
    {
        currentIteration++;

        if (currentIteration > spritesToFlipThrough.Length - 1)
            currentIteration = 1;

        spriteRenderer.sprite = spritesToFlipThrough[currentIteration];
    }
    public void Pose()
    {
        spriteRenderer.sprite = spritesToFlipThrough[0];
        timePassed = -0.2f;
        lerper = 0;
        transform.localScale = transform.localScale * 1.05f;
    }
}
