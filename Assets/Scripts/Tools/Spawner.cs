using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Optimize: Maybe you can still use Spawners while their gameObject is inactive

public class Spawner : MonoBehaviour
{
    private Stack<GameObject> inactiveObjects;
    public Queue<GameObject> activeObjects;

    /// <summary>
    /// Number of objects stored in memory
    /// </summary>
    private int totalObjectCount;

    public Spawner Init(GameObject prefab, int _totalObjectCount)
    {
        inactiveObjects = new Stack<GameObject>();
        activeObjects = new Queue<GameObject>();

        totalObjectCount = _totalObjectCount;

        InstantiateObjects(prefab);

        return this;
    }

    private void InstantiateObjects(GameObject prefab)
    {
        for (int i = 0; i < totalObjectCount; i++)
        {
            GameObject instance = Instantiate(prefab);
            instance.SetActive(false);
            inactiveObjects.Push(instance);
        }
    }

    /// <summary>
    /// Spawn an object at position. If there are no inactive objects, an active one is dequeued.
    /// </summary>
    /// <returns>
    /// The spawned GameObject
    /// </returns>
    public GameObject SpawnAt(Vector3 position)
    {
        GameObject instance;

        // if there are no inactiveObjects, recycle active one
        if (inactiveObjects.Count > 0)
        {
            instance = inactiveObjects.Pop();

        }
        else
        {
            Debug.LogWarning("No inactive objects in memory. Active object recycled.");
            instance = activeObjects.Dequeue();
        }

        instance.transform.position = position;
        instance.SetActive(true);
        activeObjects.Enqueue(instance);

        return instance;
    }

    public void DeactivateObjects()
    {
        while (activeObjects.Count > 0)
        {
            GameObject instance = activeObjects.Dequeue();
            instance.SetActive(false);
            inactiveObjects.Push(instance);
        }
    }
}

