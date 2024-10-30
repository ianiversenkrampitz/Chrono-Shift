using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float ropeLength;
    public float distanceFromPoint;
    public float springiness;
    public float dampening;
    public Vector3 pointPosition;
    public Material lineMat;
    public bool isGrappling;
    public LineRenderer lineRenderer;
    public SpringJoint joint;

    // Start is called before the first frame update
    void Start()
    {
        //for debug sphere drawing
        Gizmos.color = Color.red;
        //for drawing line from point to player 
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.material = lineMat;
        lineRenderer.widthMultiplier = .2f;
        joint = gameObject.AddComponent<SpringJoint>();
        joint = GetComponent<SpringJoint>();      
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        distanceFromPoint = (pointPosition - transform.position).magnitude;
        //can only be used in midair 
        if (!onGround)
        {
            if (playerInputActions.PlayerActions.Swing.IsPressed())
            {
                //Debug.Log("Pressed swing");
                Grapple();
            }
        }
        if (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, pointPosition);
            //sets anchor point to point position
            joint.connectedAnchor = pointPosition;
            //sets springiness and damper
            joint.spring = springiness;
            joint.damper = dampening;
            joint.maxDistance = ropeLength;
            joint.minDistance = ropeLength;
        }
        else
        {
            Debug.Log("no longer grappling");
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            //sets springiness and damper to null to turn it off
            joint.spring = 0f;
            joint.damper = 0f;
        }
    }
    /// <summary>
    /// does grappling code 
    /// </summary>
    private void Grapple()
    {
        // Create a sphere around the player’s position with a given radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);

        // Check each collider within the sphere
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Grapple"))
            {
                //set pointposition to this point's position 
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
        //rigidBodyRef.AddForce(((pointPosition - transform.position).magnitude)-(ropeLength))*(direction * walkSpeed), ForceMode.Acceleration);
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
        //swing if pressed 
        while (playerInputActions.PlayerActions.Swing.IsPressed())
        {
            isGrappling = true;
            //swing towards center of object with fixed momentum 
            //check if the player's distance is farther than rope length
            if (ropeLength > distanceFromPoint)
            {
                isGrappling = false;
                Debug.Log("distance is " + ropeLength);
                yield return null;
            }
            else
            {
                yield return null;
            }
        }
        Debug.Log("broke");
        isGrappling = false;
    }
}
