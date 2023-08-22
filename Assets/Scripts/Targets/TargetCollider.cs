using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    // target is hit by player ball
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // disable entire target

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player Ball")
        {
            // disable GameObject
            this.gameObject.SetActive(false);

            // spawn explosion effect
            Spawner explosionSpawner = GameController.Spawn.ExplosionSpawner.GetComponent<Spawner>();
            GameObject explosion = explosionSpawner.SpawnAt(this.transform.position);

            // set explosion to same size as target collider
            explosion.transform.localScale = (Vector2)this.transform.localScale * (Vector2)this.transform.parent.localScale;

            // slow down animation by scale of target (parent) so that speed of the waves is always the same
            explosion.GetComponent<Animator>().speed = 1f / this.transform.parent.localScale.x;

            // invert player ball horizontal velocity so it bounces sideways 
            collision.GetComponent<PlayerBall>().InvertVelocityQueued = true;//////////////////////DEPRECATED 
        }
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
