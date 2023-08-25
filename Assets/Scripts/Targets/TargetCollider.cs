using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player Ball")
        {
            DestroyThisTarget();
        }
        else if (collision.tag == "Defense Area")
        {
            EndRun();
        }
    }

    private void DestroyThisTarget()
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

        GameController.Game.Level.IncrementPlayerScore();

        // invert player ball horizontal velocity so it bounces sideways 
        //collision.GetComponent<PlayerBall>().InvertVelocityQueued = true;//////////////////////DEPRECATED 
    }    

    /// <summary>
    /// Ends the run and triggers animation which triggers game over menu
    /// </summary>
    private void EndRun()
    {
        // spawn game over explosion animation at target position and set scale
        GameObject solidExplosion = GameController.Game.Level.SolidExplosion;

        solidExplosion.transform.position = this.transform.position;
        solidExplosion.transform.localScale = (Vector2)this.transform.localScale * (Vector2)this.transform.parent.localScale;
        solidExplosion.SetActive(true);

        GameController.Game.Level.EndLevel();
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
