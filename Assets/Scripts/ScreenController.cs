using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    private float orthographicHalfWidth = 100f;

    private float originalOrthographicSize;

    /// <summary>
    /// The distance by which the camera is moved down to accomodate screen ratio
    /// </summary>
    private float cameraDeltaYPosition = 0;
  

    public ScreenController Init()
    {
        FitScreen();



        return this;
    }

    /// <summary>
    /// Resize the camera window according to screen ratio, and align to bottom
    /// </summary>
    private void FitScreen()
    {
        originalOrthographicSize = Camera.main.orthographicSize;

        Camera.main.orthographicSize = orthographicHalfWidth * Screen.height / Screen.width;

        cameraDeltaYPosition = Camera.main.orthographicSize - originalOrthographicSize;

        Camera.main.transform.position += new Vector3(0f, cameraDeltaYPosition, 0f);
    }
}
