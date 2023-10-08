using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    private readonly int[] _timeMilestones = { 1, 4, 7, 10, 25};

    private readonly int _maxDifficulty = 10;

    /// <summary>
    /// The current difficulty tier, which is incremented when a new time milestone is reached, and added to the current difficulty
    /// </summary>
    /// </summary>
    private int _currentDifficultyTier = 0;

    /// <summary>
    /// The current difficulty, which is used to determine the traits of a spawned target
    /// </summary>
    private float _currentDifficulty = 0;

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

        _currentDifficulty = 0;

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
        // prepare google play in-app review request
        StartCoroutine(GameController.PrepareReviewRequest());

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
        _currentDifficultyTier = 0;

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
        if (_currentDifficultyTier < _timeMilestones.Length)
        {
            if (_levelTimer > _timeMilestones[_currentDifficultyTier])
            {
                _currentDifficultyTier++;

                
                //Debug.Log("Difficulty = " + _currentDifficultyTier + "\n" +
                //    "Last time milestone = " + _levelTimer + " seconds");
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
        if (_currentDifficulty != _maxDifficulty)
        {
            float baseDifficulty = 1;

            float temporalDifficulty = (int)_levelTimer / 20f; // cast to int to reduce significant figures and optimize calculation

            _currentDifficulty = baseDifficulty + _currentDifficultyTier + temporalDifficulty;

            if (_currentDifficulty > _maxDifficulty)
            {
                _currentDifficulty = _maxDifficulty;
            }
        }

        //test
        //_currentDifficulty = _maxDifficulty;


        Debug.Log("difficulty = " + _currentDifficulty);

        // calculate target traits based on difficulty
        float fallSpeed = 60f + (_currentDifficulty * 2.7f);
        float size = 2f - (_currentDifficulty * 0.05f);
        float spawnInterval = 3f - (_currentDifficulty * 0.24f); //* Mathf.Pow(0.85f, _currentDifficultyTier);

        float animationSpeed = ((fallSpeed / 120f) + (.1f * _currentDifficulty));

        // set target traits
        target.GetComponent<Rigidbody2D>().velocity = new Vector2 (0, -fallSpeed * Random.Range(0.5f, 1.5f));
        target.GetComponent<Animator>().speed = animationSpeed * Random.Range(0.5f, 1.5f);
        target.transform.localScale = Vector3.one * size * Random.Range(0.5f, 1.5f);
        _targetSpawnTimeInterval = spawnInterval;




        // randomize starting direction
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
