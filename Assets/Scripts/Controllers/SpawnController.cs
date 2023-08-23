using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public List<Spawner> AllSpawners {  get; private set; }

    public GameObject PlayerBallSpawner { get; private set; }
    [SerializeField] private GameObject _playerBallPrefab;
    public GameObject TargetSpawner { get; private set; }
    [SerializeField] private GameObject _targetPrefab;
    public GameObject ExplosionSpawner { get; private set; }
    [SerializeField] private GameObject _explosionPrefab;


    [SerializeField] private GameObject _spawnerPrefab;

    public SpawnController Init()
    {
        InstantiateAllSpawners();
        return this;
    } 

    /// <summary>
    /// Disable every GameObject spawned by each and every Spawner
    /// </summary>
    public void DespawnAll()
    {
        foreach (Spawner spawner in AllSpawners)
        {
            spawner.DeactivateObjects();
        }
    }    

    private void InstantiateAllSpawners()
    {
        AllSpawners = new List<Spawner>();

        PlayerBallSpawner = Instantiate(_spawnerPrefab);
        PlayerBallSpawner.GetComponent<Spawner>().Init(_playerBallPrefab, 10);
        AllSpawners.Add(PlayerBallSpawner.GetComponent<Spawner>());

        TargetSpawner = Instantiate(_spawnerPrefab);
        TargetSpawner.GetComponent<Spawner>().Init(_targetPrefab, 50);
        AllSpawners.Add(TargetSpawner.GetComponent<Spawner>());

        ExplosionSpawner = Instantiate(_spawnerPrefab);
        ExplosionSpawner.GetComponent<Spawner>().Init(_explosionPrefab, 10);
        AllSpawners.Add(ExplosionSpawner.GetComponent<Spawner>());
        

        /*/// Doesn't work, seems maybe spawner isn't passing by reference
        void InstantiateSpawner(GameObject spawner, GameObject prefab, int objectCount)
        {
            spawner = Instantiate(_spawnerPrefab);
            spawner.GetComponent<Spawner>().Init(prefab, objectCount);
            AllSpawners.Add(spawner.GetComponent<Spawner>());
        }

        /*
        InstantiateSpawner(PlayerBallSpawner, _playerBallPrefab, 10);
        InstantiateSpawner(TargetSpawner, _targetPrefab, 50);
        InstantiateSpawner(ExplosionSpawner, _explosionPrefab, 10);
        */
    }

    
}


