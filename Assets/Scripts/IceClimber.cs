using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/* 
 * Iversen-Krampitz, Ian
 * 11/5/2024
 * Controls ice climber mechanics. 
 */

public class IceClimber : MonoBehaviour
{
    public float sphereRadius;
    public float sphereMaxDistance;
    public float ropeLength;
    public float distanceFromPoint;
    public float springiness;
    public float dampening;
    public float maxPushForce;
    public float pushForce;
    public Vector3 pointPosition;
    public Vector3 pointDirection;
    public Material ropeMat;
    public bool isGrappling;
    public bool grappleToggle;
    public bool canGrapple;
    public LineRenderer lineRenderer;
    public SpringJoint joint;

    public PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        canGrapple = true;
        grappleToggle = false;
        //for debug sphere drawing
        Gizmos.color = Color.red;
        //for drawing line from point to player 
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.material = ropeMat;
        lineRenderer.widthMultiplier = .2f;
        //sets up spring joints 
        joint = gameObject.AddComponent<SpringJoint>();
        joint = GetComponent<SpringJoint>();      
    }
    
    public void FixedUpdate()
    {
        pushForce = Mathf.Clamp(distanceFromPoint, 0f, maxPushForce);
        distanceFromPoint = (pointPosition - transform.position).magnitude;
        pointDirection = (pointPosition - transform.position);
        //can only be used in midair 
        if (!controller.onGround)
        {
            if (controller.swingInput && canGrapple)
            {
                StartCoroutine(GrappleToggle());
            }
            if (grappleToggle)
            {
                Pushback();
            }
        }
        if (isGrappling)
        {
            //draws line 
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
            pointPosition = transform.position;
            //disables line 
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            //sets springiness and damper to null to turn it off
            joint.spring = 0f;
            joint.damper = 0f;
        }
        controller.swingInput = false;
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
        Debug.Log("normal pushback");
        
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
        //swing if pressed 
        while (grappleToggle)
        {
            isGrappling = true;
            //swing towards center of object with fixed momentum 
            //check if the player's distance is farther than rope length
            if (ropeLength > sphereMaxDistance)
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

        Debug.Log("no longer grappling");
        isGrappling = false;
    }
    /// <summary>
    /// toggles grapple on and off 
    /// </summary>
    /// <returns></returns>
    public IEnumerator GrappleToggle()
    {
        canGrapple = false;
        if (!grappleToggle)
        {
            grappleToggle = true;
            Grapple();
        }
        else
        {
            grappleToggle = false;
            Grapple();
        }
        controller.swingInput = false;
        yield return new WaitForSeconds(.5f);
        canGrapple = true;
    }
}