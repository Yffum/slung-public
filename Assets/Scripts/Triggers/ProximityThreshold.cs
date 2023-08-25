using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityThreshold : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Pouch")
        {
            // detach ball
            GameController.Game.Level.SlingshotInputHandler.GetComponent<SlingshotInputHandler>().DetachBall();

            this.gameObject.SetActive(false);
        }
    }
}
