using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Iversen-Krampitz, Ian
 * Monaghan, Devin
 * 11/5/2024
 * Controls collision. 
*/ 

public class Collision : MonoBehaviour
{
    // reference spawnpoint
    public Vector3 spawnPoint;

    // reference playercontroller
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        //creates spawnpoint
    }

    // is called when object passes through a trigger
    public void OnTriggerEnter(Collider other)
    {
        // set respawn point on collision with checkpoint
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            //changes spawnpoint to checkpoint's parent transform 
            spawnPoint = other.transform.parent.position;
            Debug.Log("hit new checkpoint");
        }

        // respawn on collision with deathfloor
        if (other.gameObject.CompareTag("Death"))
        {
            // teleport to spawn position
            transform.position = spawnPoint;
            // reset rotation
            transform.rotation = Quaternion.Euler(Vector3.zero);
            // reset momentum
            playerController.rigidBodyRef.velocity = Vector3.zero;
            Debug.Log("died from bottomless pit");
            //add stuff to subtract health/lives and reset other variables here
        }

        if (other.gameObject.CompareTag("Level End"))
        {
            other.GetComponent<MenuManager>().NextScene();
        }
    }

    // is caled when the object collides into something
    public void OnCollisionEnter(UnityEngine.Collision other)
    {
        // if the collided object is a breakable and the player is dashing, set inactive the collided object
        if (other.gameObject.CompareTag("Breakable") && playerController.dashing)
        {
            other.gameObject.SetActive(false);
            Debug.Log("broke a wall");
        }
    }
}