using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private GameObject leftPoleAnchor;
    [SerializeField] private GameObject rightPoleAnchor;

    [SerializeField] private GameObject pouch;


    void FixedUpdate()
    {

    }

    private void AdjustLeftBand()
    {
        // number of bead child objects
        int beadCount = 4;

        // get anchor positions 
        Vector3 leftAnchorPosition = leftPoleAnchor.transform.position; //Optimize: this is constant
        Vector3 rightAnchorPosition = pouch.transform.position;


        
    }    
}
