using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If targets enter this area, it's game over
/// </summary>
public class DefenseArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Target")
        {
            GameController.Level.EndRun();
        }
    }
}
