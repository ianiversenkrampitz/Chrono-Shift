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
    public Vector3 pointDirection;
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
        //can only be used in midair 
        if (!controller.onGround)
        {
            //if input works and can grapple, toggle 
            if (Input.GetMouseButtonDown(0) && grappleToggle)
            {
                Grapple();
                Debug.Log("Pressed mouse down, grappling");
            }
            else if (Input.GetMouseButtonDown(0) && !grappleToggle)
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);

        // Check each collider within the sphere
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Grapple"))
            {
                //set pointposition to this point's position 
                grapplePoint = hitCollider.transform.position;
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
                //break so it doesnt choose multiple points if more than one is detected
                Debug.Log("Grapple point detected");
                break;
            }
            else
            {
                grappleToggle = true;
                controller.grappling = false;
                Debug.Log("No grapple point detected");
            }
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
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
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