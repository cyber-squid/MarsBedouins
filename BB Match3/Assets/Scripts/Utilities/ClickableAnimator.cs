using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickableAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] imagesToFlipThrough;
    Image imgRenderer;
    [SerializeField] GameObject background;
    [SerializeField] Button skipButton;

    static bool shouldPlay = true;

    int currentIteration;

    void Start()
    {
        if (!shouldPlay)
        {
            Deactivate();
            return;
        }

        imgRenderer = GetComponent<Image>();

        skipButton.onClick.AddListener(Deactivate);
    }

    void Update()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Ended)
        {
            Iterate();
        }
    }

    void Iterate()
    {
        currentIteration++;

        if (currentIteration > imagesToFlipThrough.Length - 1)
        {
            currentIteration = 0;
            Deactivate();
        }

        imgRenderer.sprite = imagesToFlipThrough[currentIteration];
    }

    void Deactivate()
    {
        this.gameObject.SetActive(false);
        if (background != null) (background.gameObject).SetActive(false);

        shouldPlay = false;
    }
}
