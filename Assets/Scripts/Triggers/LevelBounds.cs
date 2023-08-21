using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    // Deactivate every object that leaves level bounds
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Collides");

        collision.gameObject.SetActive(false);
    }
}
