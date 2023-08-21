using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Handle the user's touch input when the slingshot is active by moving the pouch with the user's touch
/// </summary>
public class SlingshotInputHandler : MonoBehaviour
{
    /// <summary> The upper bounds of the area in which touch input is received </summary>
    [SerializeField] private Transform _touchUpperBound;

    /// <summary> The upper bounds of the pouch position </summary>
    [SerializeField] private Transform _pouchUpperBound;

    /// <summary> The position the pouch returns to </summary>
    [SerializeField] private Transform _pouchRestingSpot;

    /// <summary>
    /// The slingshot's pouch
    /// </summary>
    [SerializeField] private GameObject _pouch;

    //-------for adjusting RigidBody2D velocity
    private bool _pouchVelocityUpdateQueued = false;
    private Vector3 _newPouchVelocity = Vector3.zero;

    /// <summary>
    /// The ball that is launched by the slingshot
    /// </summary>
    private GameObject _playerBall = null;

    //-------for adjusting RigidBody2D velocity
    private bool _playerBallVelocityUpdateQueued = false;
    private Vector3 _newPlayerBallVelocity = Vector3.zero;

    /// <summary>
    /// True if pouch is snapping back to resting position
    /// </summary>
    private bool _snappingBack = false;

    /// <summary>
    /// True if pouch is snapping back and is in proximity of rest position (i.e. almost finished _snappingBack)
    /// </summary>
    private bool _finishingSnapBack = false;

    /// <summary>
    /// If there is no _playerBall attached to the pouch, spawn a new _playerBall
    /// </summary>
    public void SpawnPlayerBall()
    {
        if (_playerBall == null)
        {
            // get spawner
            Spawner ballSpawner = GameController.Spawn.PlayerBallSpawner.GetComponent<Spawner>();

            // spawn ball at pouch position
            _playerBall = ballSpawner.SpawnAt(_pouch.transform.position);

            // set ball's initial scale (global!)
            Vector3 playerBallMinScale = new Vector3(3, 3, 1);
            _playerBall.transform.localScale = playerBallMinScale;

            // attach ball to pouch and turn off ball physics
            _playerBall.transform.SetParent(_pouch.transform);
            _playerBall.GetComponent<Rigidbody2D>().simulated = false;
        }
    }

    private void Update()
    {
        HandleUserInput();

        SpawnPlayerBall();

        GrowPlayerBall();
    }

    private void FixedUpdate()
    {
        // update pouch velocity
        if (_pouchVelocityUpdateQueued)
        {
            _pouch.GetComponent<Rigidbody2D>().velocity = _newPouchVelocity;
            _pouchVelocityUpdateQueued = false;
        }

        // update ball velocity and detach ball
        if (_playerBallVelocityUpdateQueued)
        {
            _playerBall.GetComponent<Rigidbody2D>().velocity = _newPlayerBallVelocity;
            _playerBallVelocityUpdateQueued = false;

            _playerBall = null;
        }
    }

    private void HandleUserInput()
    {
        float pouchProximityThreshold = 20f; // maximum distance from the resting spot in which playerBall is in proximity

        if (_snappingBack)
        {
            float restDisplacement = Vector3.Distance(_pouch.transform.position, _pouchRestingSpot.position); // pouch distance from resting position

            // if not finishing launch and pouch is near resting spot, start finishing launch
            if (!_finishingSnapBack && restDisplacement < pouchProximityThreshold)
            {
                _finishingSnapBack = true;
                return;
            }

            // if finishing launch and pouch exits threshold proximity to resting position
            else if (_finishingSnapBack && restDisplacement > pouchProximityThreshold)
            {
                DetachBall();
            }

            // if still _snappingBack, skip touch handling
            else
            {
                return;
            }
        }

        if (Input.touchCount > 0)
        {
            // get touch world position
            Touch touch = Input.GetTouch(Input.touchCount - 1);
            Vector3 touchPosition = GameController.Screen.GetGlobalPosition(touch.position);

            // get pouch displacement from resting position
            Vector3 pouchDisplacement = _pouchRestingSpot.position - _pouch.transform.position;

            // if touch is within bounds
            if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                && touchPosition.y < _touchUpperBound.position.y)
            {
                // move pouch with user touch position
                MovePouchToPosition(touchPosition);
            }

            // if touch ends and pouch is not in proximity of the resting spot
            else if (touch.phase == TouchPhase.Ended
                && pouchDisplacement.magnitude > pouchProximityThreshold)
            {
                SnapBack();
            }

            else
            {
                MovePouchTowardsRestPosition();
            }
        }
        else
        {
            MovePouchTowardsRestPosition();
        }
    }

    /// <summary>
    /// Instantly move pouch to position (with height bounded by _pouchUpperBound)
    /// </summary>
    private void MovePouchToPosition(Vector3 position)
    {
        // move pouch to touch
        _pouch.transform.position = position;

        // limit vertical position to _pouchUpperBound
        if (_pouch.transform.position.y > _pouchUpperBound.position.y)
        {
            _pouch.transform.position = new Vector3(_pouch.transform.position.x, _pouchUpperBound.position.y);
        }
    }

    private void MovePouchTowardsRestPosition()
    {
        float speed = 300f;
        float step = speed * Time.deltaTime; // calculate distance to move
        _pouch.transform.position = Vector3.MoveTowards(_pouch.transform.position, _pouchRestingSpot.position, step);
    }

    /// <summary>
    /// Set the pouch's velocity such that it snaps back towards resting position
    /// </summary>
    private void SnapBack()
    {
        _snappingBack = true;

        // get pouch displacement from resting position
        Vector3 pouchDisplacement = _pouchRestingSpot.position - _pouch.transform.position;

        // set ball speed based on displacement magnitude
        float ballSpeedFactor = 10f;
        float ballSpeed = pouchDisplacement.magnitude * ballSpeedFactor;

        // calculate velocity
        float launchVelocityY = ((ballSpeed * pouchDisplacement.y) / Mathf.Sqrt(Mathf.Pow(pouchDisplacement.x, 2f) + Mathf.Pow(pouchDisplacement.y, 2f)));
        float launchVelocityX = Mathf.Sqrt(Mathf.Pow(ballSpeed, 2f) - Mathf.Pow(launchVelocityY, 2f));
        if (pouchDisplacement.x < 0)
        {
            launchVelocityX = 0 - launchVelocityX;
        }    

        // set pouch velocity
        _newPouchVelocity = new Vector3(launchVelocityX, launchVelocityY);
        _pouchVelocityUpdateQueued = true;
    }

    /// <summary>
    /// Stops snapping back _pouch and transfers _pouch velocity to _playerBall, which is detached from the pouch.
    /// _playerBall is derefenced after velocity is set in FixedUpdate()
    /// </summary>
    private void DetachBall()
    {
        _snappingBack = false;
        _finishingSnapBack = false;

        // if ball is attached, launch
        if (_playerBall != null)
        {
            // detach ball from pouch and activate ball physics
            _playerBall.transform.SetParent(null, true);
            _playerBall.GetComponent<Rigidbody2D>().simulated = true;

            // pass velocity to ball
            _newPlayerBallVelocity = _pouch.GetComponent<Rigidbody2D>().velocity;
            _playerBallVelocityUpdateQueued = true;
        }

        // set pouch velocity to 0
        _newPouchVelocity = Vector3.zero;
        _pouchVelocityUpdateQueued = true;
    }

    /// <summary>
    /// Scale the ball towards full size--run every Update()
    /// </summary>
    private void GrowPlayerBall()
    {
        // set ball final scale ( local! )
        Vector3 ballFullScale = new Vector3(5f, 5f, 1);

        float speed = 14f;
        float step = speed * Time.deltaTime; // calculate amount to increase scale
        _playerBall.transform.localScale = Vector3.MoveTowards(_playerBall.transform.localScale, ballFullScale, step);
    }    
}
