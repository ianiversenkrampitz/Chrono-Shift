using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// Monaghan, Devin
/// 11/2/2024
/// handles moving platform behavior
/// </summary>

public class MovingPlatform : MonoBehaviour
{
    // speed platform moves
    public float speed = 10f;
    // time platform pauses moving when having reached a position
    public float pause = 1f;

    // holds position references 
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;

    // is the platform moving towards position 1
    private bool towards1 = true;
    // is the platform allowed to move
    private bool moving = true;

    // Update is called once per frame
    void FixedUpdate()
    {
        // only allow the player to move outside of pause
        if (moving)
        {
            // move platform towards pos1 when towards1 is true
            if (towards1)
            {
                transform.position = Vector3.MoveTowards(transform.position, pos1.position, speed * Time.deltaTime);

                // if platform is approximately at pos1's position, move towards pos2
                if (Vector3.Distance(transform.position, pos1.position) < 0.001f)
                {
                    StartCoroutine(PauseTimer());
                    towards1 = false;
                }
            }
            // move platform towards pos2 when towards1 is false
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pos2.position, speed * Time.deltaTime);

                // if platform is approximately at pos2's position, move towards pos1
                if (Vector3.Distance(transform.position, pos2.position) < 0.001f)
                {
                    StartCoroutine(PauseTimer());
                    towards1 = true;
                }
            }
        }
    }

    // if player steps on platform, make them a child so that they move with the platform
    void OnCollisionEnter(UnityEngine.Collision collision)
    {
       if (collision.gameObject.CompareTag("PlayerTag"))
       {
            collision.transform.parent = transform;
        }
    }

    // if player gets off platform, unchild them so they stop moving with the platform
    void OnCollisionExit(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerTag"))
        {
            collision.transform.parent = null;
        }
    }

    // timer for move pause
    IEnumerator PauseTimer()
    {
        moving = false;
        yield return new WaitForSeconds(pause);
        moving = true;
    }
}