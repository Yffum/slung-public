using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    // target is hit by player ball
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // disable entire target
        this.gameObject.SetActive(false);


        this.transform.parent.GetComponent<Animator>().enabled = false;
        this.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 100);
    }

    // make sure parent target is disabled, whether it's from a collision
    // or from leaving the game bounds trigger
    private void OnDisable()
    {
        // can't disable parent on the same frame, so delay
        Invoke("DisableParent", 0f);
    }

    private void DisableParent()
    {
        this.transform.parent.gameObject.SetActive(false);
    }
}
