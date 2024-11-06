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
    public float swingSpeed;
    public float correctSpeed;
    public Vector3 pointPosition;
    public Vector3 pointDirection;
    public Material ropeMat;
    public bool isGrappling;
    public bool grappleToggle;
    public bool canGrapple;
    public bool detectedPoint;
    public LineRenderer lineRenderer;
    public HingeJoint joint;
    public SpringJoint springJoint;

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
    }
    
    public void FixedUpdate()
    {
        distanceFromPoint = (pointPosition - transform.position).magnitude;
        pointDirection = (pointPosition - transform.position);
        //can only be used in midair 
        if (!controller.onGround)
        {
            //if input works and can grapple, toggle 
            if (controller.swingInput && canGrapple)
            {
                StartCoroutine(GrappleToggle());
            }
            //if toggle is true and a grapple point is detected, do swinging code
            if (grappleToggle && detectedPoint)
            {
                SwingMove();
            }
        }
        //if pressed again disconnect regardless 
        else if (controller.swingInput)
        {
            StopSwinging();
        }
        UpdateLineRenderer();
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
                ropeLength = Vector3.Distance(hitCollider.transform.position, transform.position);
                //start grappling coroutine 
                detectedPoint = true;
                //break so it doesnt choose multiple points if more than one is detected
                Debug.Log("Grapple point detected");
                break;
            }
            else
            {
                detectedPoint = false;
                Debug.Log("No grapple point detected");
            }
        }
    }
    /// <summary>
    /// movement while swinging 
    /// </summary>
    private void SwingMove()
    { 
        // Apply movement force based on input
        Vector3 moveDirection = new Vector3(controller.vectorWASD.x, 0, controller.vectorWASD.y);
        controller.rigidBodyRef.AddForce(moveDirection * swingSpeed, ForceMode.Force);

        // Calculate the vector from the player to the grapple point
        Vector3 toPoint = transform.position - pointPosition;
        float currentDistance = toPoint.magnitude;

        // If the player is farther than the rope length, apply a corrective force
        if (currentDistance > ropeLength)
        {
            // Calculate the direction back to the grapple point
            Vector3 correctiveDirection = toPoint.normalized;

            // Apply a corrective force toward the grapple point
            float excessDistance = currentDistance - ropeLength;
            controller.rigidBodyRef.AddForce(-correctiveDirection * excessDistance * correctSpeed, ForceMode.Force);
        }
    }


    /// <summary>
    /// stops swinging 
    /// </summary>
    private void StopSwinging()
    {
        grappleToggle = false;
        controller.swinging = false;
        detectedPoint = false;
    }
    /// <summary>
    /// debug for showing sphere
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }

    private void UpdateLineRenderer()
    {
        if (detectedPoint)
        {
            //draws line 
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, pointPosition);
        }
        else
        {
            //disables line 
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }
    /// <summary>
    /// toggles grapple on and off 
    /// </summary>
    /// <returns></returns>
    public IEnumerator GrappleToggle()
    {
        Grapple();
        canGrapple = false;
        if (!grappleToggle && detectedPoint)
        {
            grappleToggle = true;
            controller.swinging = true;
        }
        else
        {
            grappleToggle = false;
            controller.swinging = false;
        }
        controller.swingInput = false;
        yield return new WaitForSeconds(.5f);
        canGrapple = true;
    }
    //unused for now 
    /*public IEnumerator Grappling()
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
                controller.swinging = false;
                grappleToggle = false;
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
    */
}