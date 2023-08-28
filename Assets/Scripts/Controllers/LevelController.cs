using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns level objects. Disables self on CleanUpLevel() to stop spawning.
/// </summary>
public class LevelController : MonoBehaviour
{
    /// <summary>
    /// The graphic effect for game over
    /// </summary>
    /*Serialize*/
    public GameObject SolidExplosion;

    /// <summary>
    /// The object which spawns balls and controls user level input
    /// </summary>
    [SerializeField] public GameObject SlingshotInputHandler;

    /// <summary>
    /// True if user is currently playing a running level
    /// </summary>
    public bool IsRunning { get; private set; }

    public int PlayerScore { get; private set; }

    /// <summary>
    /// The vertical position at which targets spawn. This Transform is adjusted by ScreenController
    /// on startup such that is just above the screen.
    /// </summary>
    [SerializeField] private Transform _spawnPoint;

    /// <summary>
    /// The times (in seconds) since stsarting the level at which the game increases in difficulty
    /// </summary>
    private readonly int[] _timeMilestones = { 2, 5, 10, 20, 30, 40, 50, 60};

    /// <summary>
    /// The current difficulty level, which is incremented when a new time milestone is reached
    /// </summary>
    /// </summary>
    private int _currentDifficulty = 8;

    /// <summary>
    /// The amount of time (in seconds) since the last target was spawned
    /// </summary>
    private float _targetSpawnTimer = 0f;

    /// <summary>
    /// The amount of time (in seconds) between target spawns
    /// </summary>
    private float _targetSpawnTimeInterval = 1.15f;

    /// <summary>
    /// The amount of time (in seconds) since the level started
    /// </summary>
    private float _levelTimer = 0f;

    public LevelController Init()
    {
        return this;
    }

    public void IncrementPlayerScore()
    {
        PlayerScore++;

        GameController.Gui.IncrementScoreGraphic();
    }

    private void ResetPlayerScore()
    {
        PlayerScore = 0;
        GameController.Gui.UpdateHighScoreGraphic();
    }    

    public void EnableUserInput()
    {
        SlingshotInputHandler.SetActive(true);
    }

    public void DisableUserInput()
    {
        SlingshotInputHandler.SetActive(false);
    }    

    public void StartLevel()
    {
        IsRunning = true;

        GameController.Sound.PlayStartLevelSound();

        GameController.Gui.CloseStartMenu();
        GameController.Gui.EnableLevelHUD();
     
        EnableUserInput();

        // enable self to start spawners
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Disable user input and freeze level, while SolidExplosion (which is called by TargetCollider like 
    /// this method) expands to occlude level objects and then calls Level.CleanUpLevel() via animation event
    /// </summary>
    public void EndLevel()
    {
        IsRunning = false;

        GameController.Sound.PlayEndLevelSound();

        // freeze level
        Time.timeScale = 0f;

        DisableUserInput();

        GameController.Gui.DisableLevelHUD();
    }

    public void PauseLevel()
    {
        Time.timeScale = 0f;

        DisableUserInput();

        GameController.Gui.OpenPauseMenu();
    }

    public void UnpauseLevel()
    {
        Time.timeScale = 1f;

        EnableUserInput();

        GameController.Gui.EnablePauseButton();
    }

    /// <summary>
    /// This methos is called after Endlevel, by a SolidExplosion animation event, after the animation
    /// occludes game objects so they can be despawned
    /// </summary>
    public void CleanUpLevel()
    {
        // disable self so spawners stop spawning
        this.gameObject.SetActive(false);

        // unfreeze level
        Time.timeScale = 1f;

        // despawn level GameObjects
        GameController.Spawn.DespawnAll();

        // ready ball
        GameController.Game.Level.SlingshotInputHandler.GetComponent<SlingshotInputHandler>().ResetState();

        // reset level timer
        _levelTimer = 0f;
        _currentDifficulty = 0;

        ResetPlayerScore();
    }

    private void Update()
    {
        // update level timer
        _levelTimer += Time.deltaTime;
        _targetSpawnTimer += Time.deltaTime;

        UpdateDifficulty();

        UpdateSpawnTimerAndSpawnIfReady();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && IsRunning)
        {
            PauseLevel();
        }
    }

    private void UpdateDifficulty()
    {
        if (_currentDifficulty < _timeMilestones.Length)
        {
            if (_levelTimer > _timeMilestones[_currentDifficulty])
            {
                _currentDifficulty++;

                Debug.Log("Difficulty = " + _currentDifficulty + "\n" +
                    "Last time milestone = " + _levelTimer + " seconds");
            }
        }

    }

    /// <summary>
    /// Update timer with deltaTime and spawn target if timer is done
    /// </summary>
    private void UpdateSpawnTimerAndSpawnIfReady()
    {
        // Check if we have reached beyond the spwan time interval.
        // Subtracting two is more accurate over time than resetting to zero.
        if (_targetSpawnTimer > _targetSpawnTimeInterval)
        {

            // reset timer (Unity recommends subtracting timer limit, rather than reseting to zero)
            _targetSpawnTimer -= _targetSpawnTimeInterval;

            AdjustTargetBasedOnTimePassed(SpawnTarget());
        }
    }

    /// <summary>
    /// Spawn target in random position just above top of the screen
    /// </summary>
    /// <returns> The target spawned </returns>
    private Target SpawnTarget()
    {
        Spawner targetSpawner = GameController.Spawn.TargetSpawner.GetComponent<Spawner>();

        // the amount by which the target position varies in the animation
        // (this prevents target from moving offscreen)
        float edgeDisplacement = 15;

        // subtract range from half the width of the level
        float spawnRange = ScreenController.OrthographicHalfWidth - edgeDisplacement; 

        // get random x position
        float xPosition = Random.Range(-spawnRange, spawnRange);

        // set spawn position
        Vector3 spawnPosition = new Vector3(xPosition, _spawnPoint.position.y);

        // spawn and return target component
        Target target = targetSpawner.SpawnAt(spawnPosition).GetComponent<Target>();

        return target;
    }

    private void AdjustTargetBasedOnTimePassed(Target target)
    {
        int fallSpeed;
        float animationSpeed;
        float size;
        float spawnInterval;

        /*
        switch (_currentDifficulty)
        {
            case 0: // start
                fallSpeed = 50;
                size = 2;
                spawnInterval = 2f;
                break;
            case 1: // > 5 seconds
                fallSpeed = 65;
                size = 1.8f;
                spawnInterval = 1.8f;
                break;
            case 2: // > 15 seconds
                fallSpeed = 80;
                size = 1.8f;
                spawnInterval = 1.8f;
                break;
            case 3: // > 30 seconds
                fallSpeed = 95;
                size = 1.6f;
                spawnInterval = 1.6f;
                break;
            case 4: // > 60 seconds
                fallSpeed = 90;
                size = 1.4f;
                spawnInterval = 1.4f;
                break;
            case 5: // > 100 seconds
                fallSpeed = 100;
                size = 1.2f;
                spawnInterval = 1.2f;
                break;
            case 6: // > 200 seconds
                fallSpeed = 110;
                size = 1.1f;
                spawnInterval = 1.1f;
                break;
            case 7: // > 300 seconds
                fallSpeed = 120;
                size = 1f;
                spawnInterval = 1f;
                break;

            default:
                Debug.LogWarning("Settings not implemented for currentDiffculty");
                fallSpeed = 100;
                animationSpeed = 1f;
                size = 1;
                spawnInterval = 1f;
                break;
        }
        */

        // calculate target traits based on _currentDifficulty
        fallSpeed = 35 + (_currentDifficulty * 3);
        size = 2 - (_currentDifficulty * 0.12f);
        spawnInterval = 3f * Mathf.Pow(0.77f, _currentDifficulty);

        animationSpeed = ((fallSpeed / 100f) + (.1f * _currentDifficulty));

        target.GetComponent<Rigidbody2D>().velocity = new Vector2 (0, (float)-fallSpeed * Random.Range(0.7f, 1.3f));
        target.GetComponent<Animator>().speed = animationSpeed * Random.Range(0.7f, 1.3f);
        target.transform.localScale = Vector3.one * size * Random.Range(0.7f, 1.3f);
        _targetSpawnTimeInterval = spawnInterval;

        
        if (Random.Range(0,2) == 1)
        {
            target.GetComponent<Animator>().SetTrigger("Go Left");
        }
        else
        {
            target.GetComponent<Animator>().SetTrigger("Go Right");
        }
        
    }
}
