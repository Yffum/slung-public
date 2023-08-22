using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] private GameObject _spawnerPrefab;

    public GameObject PlayerBallSpawner { get; private set; }
    [SerializeField] private GameObject _playerBallPrefab;
    public GameObject TargetSpawner { get; private set; }
    [SerializeField] private GameObject _targetPrefab;
    public GameObject ExplosionSpawner { get; private set; }
    [SerializeField] private GameObject _explosionPrefab;


    public SpawnController Init()
    {
        InstantiateSpawners();
        return this;
    }

    private void InstantiateSpawners()
    {
        PlayerBallSpawner = Instantiate(_spawnerPrefab);
        PlayerBallSpawner.GetComponent<Spawner>().Init(_playerBallPrefab, 10);

        TargetSpawner = Instantiate(_spawnerPrefab);
        TargetSpawner.GetComponent<Spawner>().Init(_targetPrefab, 50);

        ExplosionSpawner = Instantiate(_spawnerPrefab);
        ExplosionSpawner.GetComponent<Spawner>().Init(_explosionPrefab, 10);
    }
}


