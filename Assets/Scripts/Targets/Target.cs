using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The uppermost parent of the target object
/// </summary>
public class Target : MonoBehaviour
{
    // the speed at which the target descends
    [System.NonSerialized] public float Speed = 100;

    private void OnEnable()
    {
        // descend
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -Speed);

        // activate child and enable oscillating animator
        this.transform.GetChild(0).gameObject.SetActive(true);
        //this.GetComponent<Animator>().enabled = true;
    }

}
