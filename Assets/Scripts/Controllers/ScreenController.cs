using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    /// <summary>
    /// Half the width of the game world.
    /// </summary>
    public const float OrthographicHalfWidth = 100f;

    private float _originalOrthographicSize;

    /// <summary>
    /// The distance by which the camera is moved down to accomodate screen ratio
    /// </summary>
    private float _cameraDeltaYPosition = 0;
  

    public ScreenController Init()
    {
        FitScreen();

        return this;
    }

    /// <param name="screenPosition"> A position in screen space coordinates e.g. Touch.Position </param>
    /// <returns> The corresponding global world position </returns>
    public Vector3 GetGlobalPosition(Vector2 screenPosition)
    {
        // multiply this by touch.Position to get the global position of the touch
        float globalToScreenPositionRatio = OrthographicHalfWidth * 2 / Screen.width; // Optimize: calculate this at initialization

        // calculate position from ratio
        float xPosition = screenPosition.x * globalToScreenPositionRatio;
        float yPosition = screenPosition.y * globalToScreenPositionRatio;

        // reposition origin to center
        xPosition -= OrthographicHalfWidth;
        const float _orthographicHalfHeight = 150f; // this is not the actual center
        yPosition -= _orthographicHalfHeight;

        return new Vector3(xPosition, yPosition);
    }

    /// <summary>
    /// Resize the camera window according to screen ratio, and align to bottom
    /// </summary>
    private void FitScreen()
    {
        _originalOrthographicSize = Camera.main.orthographicSize;

        Camera.main.orthographicSize = OrthographicHalfWidth * Screen.height / Screen.width;

        _cameraDeltaYPosition = Camera.main.orthographicSize - _originalOrthographicSize;

        Camera.main.transform.position += new Vector3(0f, _cameraDeltaYPosition, 0f);
    }
}
