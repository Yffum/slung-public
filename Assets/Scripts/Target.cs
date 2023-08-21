using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // the speed at which the target descends
    [SerializeField] float _speed;

    private void OnEnable()
    {
        //Debug.LogError("target made");
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -_speed);

        this.transform.GetChild(0).gameObject.SetActive(true);
        this.GetComponent<Animator>().enabled = true;
    }

}
