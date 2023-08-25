using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    // Deactivate every object that leaves level bounds
    private void OnTriggerExit2D(Collider2D collision)
    {
        // if pouch is out of bounds, reset slingshot input handler
        if (collision.tag == "Pouch")
        {
            GameController.Game.Level.SlingshotInputHandler.GetComponent<SlingshotInputHandler>().ResetState();
        }
        else // otherwise despawn game object ("Player Ball"s and "Target"s)
        {
            collision.gameObject.SetActive(false);
        }
    }
}
