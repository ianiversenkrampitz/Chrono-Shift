using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/* 
 * Iversen-Krampitz, Ian
 * Monaghan, Devin
 * 10/30/2024
 * Controls ice climber mechanics. 
 */

public class SwingingV2 : MonoBehaviour
{
    public float sphereRadius;
    public float sphereMaxDistance;
    public float ropeLength;
    public float distanceFromPoint;
    public float springiness;
    public float dampening;
    public float maxPushForce;

    // position of attached grapple point
    // if no attached grapple point, transform.position
    public Vector3 pointPosition;
    public Vector3 pointDirection;

    // hold references
    public LineRenderer lineRenderer;
    public SpringJoint joint;
    public PlayerController controller;
    // material of rope
    public Material ropeMat;

    // Start is called before the first frame update
    void Start()
    {
        // set color of debug drawings
        Gizmos.color = Color.red;

        // add line renderer component to player & set reference
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        // set line material to look like rope
        lineRenderer.material = ropeMat;
        // make rope thicker
        lineRenderer.widthMultiplier = .1f;
        // turn line renderer off so we're only drawing the rope when needed
        lineRenderer.enabled = false;

        // add spring joint component to player & set reference
        joint = gameObject.AddComponent<SpringJoint>();
    }

    // called every fixed frame
    public void FixedUpdate()
    {
        // update distance from point and direction to point
        distanceFromPoint = Vector3.Distance(pointPosition, transform.position);
        pointDirection = (pointPosition - transform.position);

        // only detect input when in the air
        if (!controller.onGround)
        {
            if (controller.swingInput && !controller.swinging)
            {
                FindPoint();
            }
        }




        if (!controller.swinging)
        {
            pointPosition = transform.position;
        }

        // can only be used in midair 
        if (!controller.onGround)
        {
            Pushback();
        }

        // draw line to grapple point when attached
        DrawLine();

        // ensure swing input is off
        controller.swingInput = false;
    }

    // find a grapple point to attach to
    private void FindPoint()
    {
        // Create a sphere around the player’s position with a given radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);

        // Check each collider within the sphere
        foreach (Collider hitCollider in hitColliders)
        {
            // if the collided object is a grapple point, select it to attach to and dismiss the rest
            // begin swinging
            if (hitCollider.CompareTag("Grapple"))
            {
                //set pointPosition to this grapple point's position
                pointPosition = hitCollider.transform.position;

                //set pointdistance to collider position - transform position 
                ropeLength = (hitCollider.transform.position - transform.position).magnitude;

                //start grappling coroutine 
                StartCoroutine(Grappling());

                //break so it doesnt choose multiple points if more than one is detected
                Debug.Log("Grapple point detected");
                break;
            }
            else
            {
                controller.swinging = false;
                Debug.Log("No grapple point detected");
            }
        }
    }

    /// <summary>
    /// pushes character in opposite direction of swinging
    /// </summary>
    private void Pushback()
    {
        //pushes the character in the opposite direction of movement,
        //multiplied by distance from center point 
        Debug.Log("normal pushback");
        float pushForce = Mathf.Clamp(distanceFromPoint, 0f, maxPushForce);
        controller.rigidBodyRef.AddForce(-controller.direction * pushForce, ForceMode.Impulse);

        //if not holding a direction, apply force towards center of point 

        if (!controller.Moving() && (distanceFromPoint > ropeLength))
        {
            Debug.Log("Pushing back" + pointDirection);
            controller.rigidBodyRef.AddForce(pointDirection * pushForce, ForceMode.Impulse);
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

    /// <summary>
    /// coroutine for grappling 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Grappling()
    {
        //swing if
        while (controller.swinging)
        {
            //swing towards center of object with fixed momentum 
            //check if the player's distance is farther than rope length
            if (ropeLength > sphereMaxDistance)
            {
                controller.swinging = false;
                Debug.Log("distance is " + ropeLength);
                yield return null;
            }
            else
            {
                yield return null;
            }
        }

        Debug.Log("no longer grappling");
        controller.swinging = false;
    }

    // gets inputs
    public void Input()
    {
        // if the player has pressed the swing button and isn't already mid swing
        if (controller.swingInput && !controller.swinging)
        {
            controller.swinging = true;
            print("player has input swing");
        }
    }

    // draws line
    public void DrawLine()
    {
        // if attached to a grapple point, draw a rope to it 
        if (controller.swinging)
        {
            // turn on line renderer since we're using it
            lineRenderer.enabled = true;

            //draws line from player to grapple point
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, pointPosition);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }


    /*
            //sets springiness and damper to null to turn it off
            joint.spring = 0f;
            joint.damper = 0f;

    */
}