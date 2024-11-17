using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Iversen-Krampitz, Ian
 * Monaghan, Devin
 * 11/5/2024
 * Controls collision
 * handles health calculation
*/

public class Collision : MonoBehaviour
{
    // reference spawnpoint
    public Vector3 spawnPoint;

    // reference playercontroller
    public PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        //creates spawnpoint
    }

    public void FixedUpdate()
    {
        // die when the player's health reaches 0 or less
        if (controller.health <= 0f)
        {
            Die();
        }
    }

    // is called when object passes through a trigger
    public void OnTriggerEnter(Collider other)
    {
        // set respawn point on trigger collision with checkpoint
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            //changes spawnpoint to checkpoint's parent transform 
            spawnPoint = other.transform.parent.position;
            Debug.Log("hit new checkpoint");
        }

        // die upon trigger collision with objects tagged death
        if (other.gameObject.CompareTag("Death"))
        {
            Die();
        }

        if (other.gameObject.CompareTag("Collectable"))
        {
            other.gameObject.SetActive(false);
            //put code for coin counter here 
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
        if (other.gameObject.CompareTag("Breakable") && controller.dashing)
        {
            other.gameObject.SetActive(false);
            Debug.Log("broke a wall");
        }

        // die upon trigger collision with objects tagged death
        if (other.gameObject.CompareTag("Death"))
        {
            Die();
        }

        // subtract health upon collision
        if (other.gameObject.CompareTag("Hurt") && !controller.damageCooldown)
        {
            // subtract health
            controller.health--;
            // begin damage cooldown
            StartCoroutine(DamageCooldown());
        }
    }

    public void Die()
    {
        // teleport to spawn position
        transform.position = spawnPoint;
        // reset rotation
        transform.rotation = Quaternion.Euler(Vector3.zero);
        // reset momentum
        controller.rigidBodyRef.velocity = Vector3.zero;
        // ensure damage cooldown is off
        controller.damageCooldown = false;
        // reset player health
        controller.health = controller.maxHealth;
    }

    // timer for damage cooldown
    IEnumerator DamageCooldown()
    {
        controller.damageCooldown = true;
        yield return new WaitForSeconds(1f);
        controller.damageCooldown = false;
    }
}