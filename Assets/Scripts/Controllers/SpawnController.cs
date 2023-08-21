using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] private GameObject spawnerPrefab;

    [SerializeField] private GameObject playerBallPrefab;

    public GameObject PlayerBallSpawner;

    public SpawnController Init()
    {
        InstantiateSpawners();
        return this;
    }

    private void InstantiateSpawners()
    {
        PlayerBallSpawner = Instantiate(spawnerPrefab);
        PlayerBallSpawner.GetComponent<Spawner>().Init(playerBallPrefab, 10);
    }
}


