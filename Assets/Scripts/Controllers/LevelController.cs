using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;

    private float _targetSpawnTimer = 0f;
    private float _targetSpawnDelay = 1f;

    public LevelController Init()
    {
        return this;
    }

    private void Update()
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
