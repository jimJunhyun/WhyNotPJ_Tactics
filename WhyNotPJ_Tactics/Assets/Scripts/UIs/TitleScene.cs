using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    private AutoFlip bookFlipper;

    private void Awake()
    {
        bookFlipper = transform.GetComponentInChildren<AutoFlip>();
    }

    private void Update()
    {
        if (Input.anyKeyDown || Input.touchCount > 0)
        {
            bookFlipper.FlipRightPage();
        }
    }
}
