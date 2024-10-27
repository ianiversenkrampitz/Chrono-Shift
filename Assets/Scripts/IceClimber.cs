using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Iversen-Krampitz, Ian 
 * 10/26/2024
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
                Debug.Log("Pressed swing");
                CastSphere();
            }
        }
    }
    /// <summary>
    /// casts a sphere to detect grapple points 
    /// </summary>
    private void CastSphere()
    {
        RaycastHit hit;
        //casts a sphere around player to detect grapplepoints
        bool isHit = Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, sphereMaxDistance);
        if (isHit)
        {
            if (hit.collider.CompareTag("Grapple"))
            {
                Debug.Log("Grapple point detected");
            }
        }
        else
        {
            Debug.Log("No grapple point detected");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, sphereRadius);
        Gizmos.DrawWireSphere(transform.position * sphereMaxDistance, sphereRadius);
        Gizmos.DrawLine(transform.position, transform.position * sphereMaxDistance);
    }
}
