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

    private float _targetSpawnTimer = 0f;
    private float _targetSpawnDelay = 1.15f;

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

        GameController.Gui.CloseStartMenu();
     
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

        // freeze level
        Time.timeScale = 0f;

        DisableUserInput();
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

        ResetPlayerScore();
    }

    private void Update()
    {
        UpdateSpawnTimer();
    }

    /// <summary>
    /// Update timer with deltaTime and spawn target if timer is done
    /// </summary>
    private void UpdateSpawnTimer()
    {
        _targetSpawnTimer += Time.deltaTime;

        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (_targetSpawnTimer > _targetSpawnDelay)
        {

            // Remove the recorded 2 seconds.
            _targetSpawnTimer -= _targetSpawnDelay;

            //Time.timeScale = scrollBar;

            SpawnTarget();
        }
    }

    private void SpawnTarget()
    {
        Spawner targetSpawner = GameController.Spawn.TargetSpawner.GetComponent<Spawner>();

        // the amount by which the target position varies in the animation
        // (this prevents target from spawning offscreen)
        float edgeDisplacement = 15;

        // subtract range from half the width of the level
        float spawnRange = ScreenController.OrthographicHalfWidth - edgeDisplacement; 

        // get random x position
        float xPosition = Random.Range(-spawnRange, spawnRange);

        Vector3 spawnPosition = new Vector3(xPosition, _spawnPoint.position.y); 

        targetSpawner.SpawnAt(spawnPosition);
    }
}
