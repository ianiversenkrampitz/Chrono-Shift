using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/* 
 * Iversen-Krampitz, Ian
 * 11/6/2024
 * Controls ice climber mechanics. 
 */

public class IceClimber : MonoBehaviour
{
    public float sphereRadius;
    public float sphereMaxDistance;
    public float ropeLength;
    public float distanceFromPoint;
    public float swingForce;
    public bool grappleToggle = true;
    public Vector3 grapplePoint;
    public Vector3 sphereCenter;
    public Material ropeMat;
    public LineRenderer lineRenderer;
    public SpringJoint joint;
    public PlayerController controller;
    public Camera mainCam;

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
        lineRenderer.material = ropeMat;
        lineRenderer.widthMultiplier = .2f;
    }

    // Update is called once every frame 
    public void Update()
    {
        sphereCenter = transform.position + mainCam.transform.forward * sphereRadius * .5f;
        //can only be used in midair 
        if (!controller.onGround)
        {
            //if input works and can grapple, toggle 
            if (Input.GetKeyDown(KeyCode.F) && grappleToggle)
            {
                Grapple();
                Debug.Log("Pressed mouse down, grappling");
            }
            else if (Input.GetKeyDown(KeyCode.F) && !grappleToggle)
            {
                Debug.Log("Pressed mouse down, stop grappling");
                StopGrappling();
            }
        }
        //if pressed again disconnect regardless 
        else
        {
            StopGrappling();
        }
        //if grappling, use movement
        if (!grappleToggle)
        {
            GrappleMove();
        }
        
        
        UpdateLineRenderer();
       
    }

    /// <summary>
    /// does grappling code 
    /// </summary>
    private void Grapple()
    {
        // Create a sphere around the player’s position with a given radius
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, sphereRadius);

        Collider closestPoint = null;
        float closestDistance = float.MaxValue;

        // Check each collider within the sphere
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Grapple"))
            {
                //check distance from point
                distanceFromPoint = Vector3.Distance(transform.position, hitCollider.transform.position);

                //if point is closer than another in range, use this one 
                if (distanceFromPoint < closestDistance)
                {
                    closestDistance = distanceFromPoint;
                    closestPoint = hitCollider;
                }
            }
        }
            //check if loop is done and point is found 
            if (closestPoint != null)
            {
                //set pointposition to this point's position 
                grapplePoint = closestPoint.transform.position;
                joint = gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;
                distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

                joint.maxDistance = distanceFromPoint * .8f;
                joint.minDistance = distanceFromPoint * .25f;

                joint.spring = 4.5f;
                joint.damper = 7f;
                joint.massScale = 4.5f;

                grappleToggle = false;
                controller.grappling = true;
                Debug.Log("Grapple point detected");
            }
            else
            {
                grappleToggle = true;
                controller.grappling = false;
                Debug.Log("No grapple point detected");
            }
    }

    /// <summary>
    /// movement while grappling
    /// </summary>
    private void GrappleMove()
    {
        Vector3 forceDirection = Vector3.zero;

        // Get the camera's forward and right vectors, ignoring the y-axis
        Vector3 cameraForward = mainCam.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = mainCam.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        if (Input.GetKey(KeyCode.W))
            forceDirection += cameraForward;
        if (Input.GetKey(KeyCode.S))
            forceDirection += -cameraForward;
        if (Input.GetKey(KeyCode.A))
            forceDirection += -cameraRight;
        if (Input.GetKey(KeyCode.D))
            forceDirection += cameraRight;

        if (forceDirection != Vector3.zero)
        {
            controller.grappleDirection = forceDirection.normalized;
            controller.rigidBodyRef.AddForce(forceDirection.normalized * swingForce * Time.deltaTime, ForceMode.Force);
        }
    }

    /// <summary>
    /// stops swinging 
    /// </summary>
    private void StopGrappling()
    {
        Destroy(joint);
        grappleToggle = true;
        controller.grappling = false;
    }

    /// <summary>
    /// debug for showing sphere
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sphereCenter, sphereRadius);
    }

    /// <summary>
    /// creates rope 
    /// </summary>
    private void UpdateLineRenderer()
    {
        if (joint)
        {
            //draws line 
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
        else
        {
            //disables line 
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }
}