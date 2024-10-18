using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Iversen-Krampitz, Ian 
 * 10/13/2024
 * Controls collision. 
*/ 

public class Collision : MonoBehaviour
{
    public Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        //creates spawnpoint
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            //changes spawnpoint to checkpoint's parent transform 
            spawnPoint = other.transform.parent.position;
            Debug.Log("hit new checkpoint");
        }
        if (other.gameObject.CompareTag("Death"))
        {
            transform.position = spawnPoint;
            Debug.Log("died from bottomless pit");
            //add stuff to subtract health/lives and reset other variables here
        }
    }
}
