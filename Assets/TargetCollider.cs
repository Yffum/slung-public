using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.gameObject.SetActive(false);


        this.transform.parent.GetComponent<Animator>().enabled = false;
        this.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 100);
    }

    private void OnDisable()
    {
        Invoke("DisableParent", 0f);
    }

    private void DisableParent()
    {
        this.transform.parent.gameObject.SetActive(false);
    }
}
