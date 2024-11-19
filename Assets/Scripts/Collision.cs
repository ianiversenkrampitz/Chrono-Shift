using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Iversen-Krampitz, Ian
 * Monaghan, Devin
 * 11/18/2024
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
        // set initial spawn point
        spawnPoint = transform.position;
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
            // changes spawnpoint to checkpoint's parent transform 
            spawnPoint = other.transform.parent.position;
            // turn off the checkpoint
            other.transform.parent.gameObject.SetActive(false);
            Debug.Log("hit new checkpoint");
        }

        // die upon trigger collision with objects tagged death
        if (other.gameObject.CompareTag("Death"))
        {
            Die();
        }

        // on collision with a collectable, delete the collectable and add to score
        if (other.gameObject.CompareTag("Collectable"))
        {
            other.gameObject.SetActive(false);
            // add to player score
            controller.score++;
        }

        // if the player collides with the level end object, move them to the next level
        if (other.gameObject.CompareTag("Level End"))
        {
            other.GetComponent<MenuManager>().SelectScene(other.GetComponent<MenuManager>().thisScene++);
        }
    }

    // is called when the object collides with a non-trigger
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
        // subtract 3 from score, disallowing it to go below 0
        controller.score -= 3f;
        if (controller.score < 0f)
        {
            controller.score = 0f;
        }
    }

    // timer for damage cooldown
    IEnumerator DamageCooldown()
    {
        controller.damageCooldown = true;
        yield return new WaitForSeconds(1f);
        controller.damageCooldown = false;
    }
}