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
    /// <summary> The upper bound of the area in which touch input is received when in level</summary>
    [SerializeField] private Transform _touchUpperBoundInLevel;

    /// <summary>
    /// The upper bound of touch input
    /// </summary>
    private float _touchUpperBound;

    /// <summary> The upper bound of the pouch position </summary>
    [SerializeField] private Transform _pouchUpperBound;

    /// <summary> The lower bound of the pouch position </summary>
    [SerializeField] private Transform _pouchLowerBound;

    /// <summary> The position the pouch returns to </summary>
    [SerializeField] private Transform _pouchRestingSpot;

    /// <summary>
    /// The slingshot's pouch
    /// </summary>
    [SerializeField] private GameObject _pouch;

    /// <summary>
    /// The maximum speed to which the pouch is set when snapping back. This must be limited to prevent slipping
    /// past resting point, where the ball is detached
    /// </summary>
    private readonly int _maxPouchSpeed = 1100;

    /// <summary>
    /// The collider positioned at _pouchRestingSpot, which detaches the ball when the pouch exits, and then disables self
    /// </summary>
    [SerializeField] private GameObject _proximityThreshold;

    /// <summary>
    /// The initial global scale of the player ball
    /// </summary>
    private readonly Vector3 _playerBallMinScale = new Vector3(3, 3, 1);

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
    /// The factor which deteremines the speed at which balls launch 
    /// </summary>
    private float _ballSpeedFactor = 11f;

    /// <summary>
    /// True if pouch is snapping back to resting position
    /// </summary>
    private bool _snappingBack = false;

    /// <summary>
    /// If Level.IsRunning, touch input upper bound is set higher, otherwise it's set below the logo
    /// </summary>
    public void SetTouchInputBounds()
    {
        if (GameController.Game.Level.IsRunning)
        {
            _touchUpperBound = ScreenController.GetScreenPosition(_touchUpperBoundInLevel.position).y;
        }
        else
        {
            _touchUpperBound = ScreenController.GetScreenPosition(_pouchUpperBound.position).y;
        }
    }
    
    /// <summary>
    /// Diminish overall ball speed for tablet gameplay
    /// </summary>
    public void SetTabletBallSpeed()
    {
        _ballSpeedFactor = 8f;
    }

    public void ResetState()
    {
        _snappingBack = false;

        _pouch.transform.position = _pouchRestingSpot.position;
        
        // must set directly because level is frozen, but it shouldn't be an issue because level isn't running
        _pouch.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        _proximityThreshold.SetActive(false);

        _playerBall = null;
    }

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
            _playerBall.transform.localScale = _playerBallMinScale;

            // attach ball to pouch and turn off ball physics
            _playerBall.transform.SetParent(_pouch.transform);
            _playerBall.GetComponent<Rigidbody2D>().simulated = false;
        }
    }

    /// <returns> Calculated distance from slingshot pouch to resting position </returns>
    /// <remarks> For optimization, do not use repeatedly. Instantiate a new float and use that.</remarks>
    public float GetPouchDisplacement()
    {
        return Vector3.Distance(_pouch.transform.position, _pouchRestingSpot.position);
    }

    /// <summary>
    /// Stops snapping back _pouch and transfers _pouch velocity to _playerBall, which is detached from the pouch.
    /// _playerBall is derefenced after velocity is set in FixedUpdate()
    /// </summary>
    public void DetachBall()
    {
        _snappingBack = false;

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
            if (_playerBall == null)
            {
                _newPlayerBallVelocity = Vector3.zero;
                _playerBallVelocityUpdateQueued = false;
            }   
            else
            {
                _playerBall.GetComponent<Rigidbody2D>().velocity = _newPlayerBallVelocity;
                _playerBallVelocityUpdateQueued = false;

                _playerBall = null;
            }         
        }
    }

    private void HandleUserInput()
    {
        float pouchProximityThreshold = _proximityThreshold.GetComponent<CircleCollider2D>().radius; // maximum distance from the resting spot in which playerBall is in proximity

        if (_snappingBack)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            // get touch world position
            Touch touch = Input.GetTouch(0);

            // get pouch displacement from resting position
            Vector3 pouchDisplacement = _pouchRestingSpot.position - _pouch.transform.position;

            bool touchIsInBounds = touch.position.y < _touchUpperBound;

            // if touch is stationary
            if (touch.phase == TouchPhase.Stationary && touchIsInBounds)
            {
                // do nothing
            }

            // if touch is within bounds
            else if (touch.phase == TouchPhase.Moved && touchIsInBounds)
            {
                // move pouch with user touch position
                MovePouchToPosition(ScreenController.GetGlobalPosition(touch.position));
            }

            // if touch ends and pouch is not in proximity of the resting spot
            else if (touch.phase == TouchPhase.Ended
                && pouchDisplacement.magnitude > pouchProximityThreshold
                /*&& _initialTouchPosition != touch.position // redundant?*/)
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
        // limit vertical position to bounds
        if (position.y > _pouchUpperBound.position.y)
        {
            position = new Vector3(position.x, _pouchUpperBound.position.y);
        }
        else if (position.y < _pouchLowerBound.position.y)
        {
            position = new Vector3(position.x, _pouchLowerBound.position.y);
        }

        // move pouch to touch
        _pouch.transform.position = position;
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
        // prepare _proximityThreshold, which detaches ball on exit
        _proximityThreshold.SetActive(true);
        _snappingBack = true;

        // start level if it hasn't started already
        if (!GameController.Game.Level.IsRunning)
        {
            GameController.Game.Level.StartLevel();
        }
        // only play sound if level is running, so it doesn't overlap level start sound
        else
        {
            GameController.Sound.PlaySlingshotSound();
        }

        // get pouch displacement from resting position
        Vector3 pouchDisplacement = _pouchRestingSpot.position - _pouch.transform.position;

        // set ball speed based on displacement magnitude
        float ballSpeedFactor = _ballSpeedFactor;
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


        if (_newPouchVelocity.magnitude > _maxPouchSpeed)
        {
            float slowDownFactor = ((float)_maxPouchSpeed)/_newPouchVelocity.magnitude;

            _newPouchVelocity *= slowDownFactor;
        }

        _pouchVelocityUpdateQueued = true;

        // Debug.Log(_newPouchVelocity.magnitude);
    }

    /// <summary>
    /// Scale the ball towards full size--run every Update()
    /// </summary>
    private void GrowPlayerBall()
    {

        // set ball final scale ( local! )
        Vector3 ballFullScale = new Vector3(6f, 6f, 1);

        float speed = 8f;
        float step = speed * Time.deltaTime; // calculate amount to increase scale
        _playerBall.transform.localScale = Vector3.MoveTowards(_playerBall.transform.localScale, ballFullScale, step);
    }    
}
