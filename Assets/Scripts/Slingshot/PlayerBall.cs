using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    /// <summary>
    /// True if ball should invert x velocity next FixedUpdate
    /// </summary>
    public bool InvertVelocityQueued = false;

    private void FixedUpdate()
    {
        if (InvertVelocityQueued)
        {
            InvertVelocityX();
            InvertVelocityQueued = false;
        }
    }

    /// <summary>
    /// Inverts the x velocity of the Rigidbody2D. Only use on FixedUpdate()
    /// </summary>
    private void InvertVelocityX()
    {
        this.GetComponent<Rigidbody2D>().velocity *= new Vector2(-0.5f, 1);
    }
}
