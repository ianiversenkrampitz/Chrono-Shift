using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
 * Iversen-Krampitz, Ian 
 * 10/27/2024
 * Controls ice climber mechanics. 
 */

public class IceClimber : PlayerController
{
    public float sphereRadius;
    public float sphereMaxDistance;
   
    // Start is called before the first frame update
    void Start()
    {
        //for debug sphere drawing
        Gizmos.color = Color.red;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //can only be used in midair 
        if (!onGround)
        {
            if (playerInputActions.PlayerActions.Swing.IsPressed())
            {
                //Debug.Log("Pressed swing");
                CastSphere();
            }
        }
    }
    /// <summary>
    /// casts a sphere to detect grapple points 
    /// </summary>
    private void CastSphere()
    {
        // Create a sphere around the player’s position with a given radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);

        bool grapplePointDetected = false;

        // Check each collider within the sphere
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Grapple"))
            {
                grapplePointDetected = true;
                Debug.Log("Grapple point detected");
                // Put code for swinging here
            }
        }

        if (!grapplePointDetected)
        {
            Debug.Log("No grapple point detected");
        }
    }
    /// <summary>
    /// debug for showing sphere
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
