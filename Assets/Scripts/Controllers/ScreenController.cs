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

    [SerializeField] private Transform _spawnPoint;

    public ScreenController Init()
    {
        FitScreen();

        SetSpawnPoint();

        //QualitySettings.vSyncCount = 1;
        //Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        // Task: try getting screen refresh rate and rounding to nearest multiple of 10

        Application.targetFrameRate = 60;

        return this;
    }

    /// <param name="screenPosition"> A position in screen space coordinates e.g. Touch.position </param>
    /// <returns> The corresponding global world position </returns>
    public static Vector3 GetGlobalPosition(Vector2 screenPosition)
    {
        // multiply this by Touch.position to get the global position of the touch
        float globalToScreenPositionRatio = OrthographicHalfWidth * 2 / Screen.width; // Optimize: calculate this at initialization

        // calculate position from ratio
        float xPosition = screenPosition.x * globalToScreenPositionRatio;
        float yPosition = screenPosition.y * globalToScreenPositionRatio;

        // reposition origin to "center"
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

    /// <summary>
    /// Set the spawn point for targets using Screen.width and Screen.height
    /// </summary>
    private void SetSpawnPoint()
    {
        // screen space origin is in bottom left, and we want top middle
        Vector2 screenPosition = new Vector2(Screen.width / 2f, Screen.height);

        // get global position
        Vector2 spawnPosition = GetGlobalPosition(screenPosition);

        // add buffer so target can't be seen
        Vector2 buffer = new Vector2(0, 10f);

        // reposition
        _spawnPoint.position = spawnPosition + buffer;
    }    
}
