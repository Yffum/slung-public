using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenController : MonoBehaviour
{
    /// <summary>
    /// Half the width of the game world.
    /// </summary>
    public const float OrthographicHalfWidth = 100f;

    /// <summary>
    /// A vector with half the global width and half the global height
    /// </summary>
    public static readonly Vector2 OrthographicHalfBox = new Vector2(100, 150);

    /// <summary>
    /// True if the device the game is running on is a tablet
    /// </summary>
    public bool _isTablet = false;

    private float _originalOrthographicSize;

    /// <summary>
    /// The distance by which the camera is moved down to accomodate screen ratio
    /// </summary>
    private float _cameraDeltaYPosition = 0;

    /// <summary>
    /// The ratio of the distance between two transform positions to the distance between the two corresponding screen positions
    /// </summary>
    /// <remarks>
    /// Should be read-only, but it's calculated in Init() to make sure Screen.width is correct
    /// </remarks>
    private static float _globalToScreenPositionRatio = 1;

    /// <summary>
    /// The trigger which disables level objects that leave its bounds
    /// </summary>
    [SerializeField] private GameObject _levelBounds;

    /// <summary>
    /// The vertical position at which targets spawn
    /// </summary>
    [SerializeField] private Transform _spawnPoint;

    /// <summary>
    /// The spawn point is positioned this distance above the top of the screen
    /// </summary>
    /// <remarks> This must be manually increased if the maximum fall speed or scale of the targets is increased. Consider automating.</remarks>
    private readonly int spawnPointDisplacement = 25;

    /// <summary>
    /// The parent of menus which align with the top of the screen
    /// </summary>
    [SerializeField] private Transform _upperMenus;

    /// <summary>
    /// The parent of menus which align with the center of the screen
    /// </summary>
    [SerializeField] private Transform _centerMenus;

    /// <summary>
    /// If the height/width ratio of the display is less than this value, the device is
    /// assumed to be a tablet.
    /// </summary>
    private readonly float _maxTabletDisplayRatio = 1.45f;

    /// <summary>
    /// The start screen logo containing the highscore graphic
    /// </summary>
    /// <remarks>
    /// This is adjusted for tablets </remarks>
    [SerializeField] private Transform _logoBlob;

    public ScreenController Init()
    {
        _globalToScreenPositionRatio = OrthographicHalfWidth * 2 / Screen.width;

        AdjustCameraToScreen();

        FitLevelBoundsToScreen();

        SetSpawnPoint();
        SetUpperMenusPosition();
        SetCenterMenusPosition();

        AdjustLogoForTablets();

        //QualitySettings.vSyncCount = 1;
        //Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        // Testing: try getting screen refresh rate and rounding to nearest multiple of 10

        /*
        float rawRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;
        int roundedRefreshRate = 10 * Mathf.RoundToInt(rawRefreshRate / 10f); //rounded to nearest multiple of 10
        Application.targetFrameRate = roundedRefreshRate; //60;
        */

        Application.targetFrameRate = 999;

        return this;
    }

    public void Start()
    {
        Application.targetFrameRate = 999;
    }

    /// <param name="screenPosition"> A position in screen space coordinates e.g. Touch.position </param>
    /// <returns> The corresponding global world position </returns>
    public static Vector3 GetGlobalPosition(Vector2 screenPosition)
    {
        // change to global space
        Vector2 position = _globalToScreenPositionRatio * screenPosition;

        // reposition origin to (camera?) center
        position -= OrthographicHalfBox;

        return position;
    }

    /// <param name="globalPosition"> A global position e.g. a transform position </param>
    /// <returns> The screen position corresponding to globalPosition </returns>
     public static Vector2 GetScreenPosition(Vector3 globalPosition)
    {
        // reposition origin in global space
        Vector2 position = (Vector2)globalPosition + OrthographicHalfBox;

        // change to screen space
        position /= _globalToScreenPositionRatio;

        return position;
    }


    /// <summary>
    /// Resize the camera window according to screen ratio, and align to bottom
    /// </summary>
    private void AdjustCameraToScreen()
    {
        _originalOrthographicSize = Camera.main.orthographicSize;

        Camera.main.orthographicSize = OrthographicHalfWidth * Screen.height / Screen.width;

        _cameraDeltaYPosition = Camera.main.orthographicSize - _originalOrthographicSize;

        Camera.main.transform.position += new Vector3(0f, _cameraDeltaYPosition, 0f);
    }

    private void FitLevelBoundsToScreen()
    {
        // set width to screen width and height to 1.5x screen height so balls can go above the screen without despawning
        Vector2 boundsSize = _globalToScreenPositionRatio * new Vector2(Screen.width, 1.5f * Screen.height);

        _levelBounds.GetComponent<BoxCollider2D>().size = boundsSize;

        _levelBounds.transform.position += new Vector3(0, _cameraDeltaYPosition, 0);
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

        // displace spawn point so target can't be seen suddenly appearing
        Vector2 displacementVector = new Vector2(0, spawnPointDisplacement);

        // reposition
        _spawnPoint.position = spawnPosition + displacementVector;
    }

    /// <summary>
    /// Set the _upperMenus parent game object position to the top middle of the screen
    /// using Screen.width and Screen.height
    /// </summary>
    private void SetUpperMenusPosition()
    {
        // screen space origin is in bottom left, and we want top middle
        Vector2 screenPosition = new Vector2(Screen.width / 2f, Screen.height);

        // get global position
        Vector2 globalPosition = GetGlobalPosition(screenPosition);

        // reposition
        _upperMenus.position = globalPosition;
    }

    /// <summary>
    /// Set the _lowerMenus parent GameObject position to the center of the screen
    /// </summary>
    private void SetCenterMenusPosition()
    {
        // get center screen of screen
        Vector2 screenPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // get global position
        Vector2 globalPosition = GetGlobalPosition(screenPosition);

        //reposition
        _centerMenus.position = globalPosition;
    }

    /// <summary>
    /// Decrease the size of the logo if the display ratio is small enough to indicate a tablet is being used
    /// </summary>
    private void AdjustLogoForTablets()
    {
        float displayRatio = (float)Screen.height / (float)Screen.width;

        float smallLogoScale = 0.7f;
        float verticalDisplacement = 30f;

        // Debug.Log("Display Ratio:" + displayRatio);

        if (displayRatio < _maxTabletDisplayRatio)
        {
            // Debug.Log("is tablet");

            _isTablet = true;

            GameController.Game.Level.SlingshotInputHandler.GetComponent<SlingshotInputHandler>().SetTabletBallSpeed();

            _logoBlob.transform.localScale *= smallLogoScale;
            _logoBlob.transform.position += new Vector3(0, verticalDisplacement);
        }
    }
}
