using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    /// <summary>
    /// True if ball should invert x velocity next FixedUpdate
    /// </summary>
    public bool InvertVelocityQueued = false;

    /// <summary>
    /// The number of targets this ball has hit since spawning. Set by TargetCollider, and
    /// then used to determine sound to play
    /// </summary>
    public int TargetHitCount { get; private set; }

    public void HandleTargetHit()
    {
        TargetHitCount++;
    }

    private void OnDisable()
    {
        TargetHitCount = 0;
    }

    /* // Doesn't sound good and is distracting
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameController.Sound.PlayThudSound();
    }
    */

    private void FixedUpdate()
    {
        /*
        if (InvertVelocityQueued)
        {
            InvertVelocityX();
            InvertVelocityQueued = false;
        }
        */
    }

    /// <summary>
    /// Inverts the x velocity of the Rigidbody2D. Only use on FixedUpdate()
    /// </summary>
    private void InvertVelocityX()
    {
        this.GetComponent<Rigidbody2D>().velocity *= new Vector2(-0.5f, 1);
    }
}
