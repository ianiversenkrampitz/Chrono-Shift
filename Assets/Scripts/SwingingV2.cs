using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/* 
 * Iversen-Krampitz, Ian
 * Monaghan, Devin
 * 11/35/2024
 * Controls ice climber mechanics
 */
  
public class SwingingV2 : MonoBehaviour
{
    public float sphereRadius = 5f;

    // hold references
    public LineRenderer lineRenderer;
    public HingeJoint joint;
    public SwingPlayer controller;
    // material of rope
    public Material ropeMat;

    // Start is called before the first frame update
    void Start()
    {
        // add line renderer component to player & set reference
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        // set line material to look like rope
        lineRenderer.material = ropeMat;
        // make rope thicker
        lineRenderer.widthMultiplier = .1f;
        // turn line renderer off so we're only drawing the rope when needed
        lineRenderer.enabled = false;
    }

    // called every fixed frame
    public void FixedUpdate()
    {
        // only detect input when in the air
        if (!controller.onGround)
        {
            if (controller.swingInput && !controller.swinging)
            {
                Swing();
                controller.swingInput = false;
            }
        }

        // if player stops swinging, destroy joint so the player is no longer attached to the grapple point
        if (!controller.swinging)
        {
            Destroy(joint);
            // controller.rigidBodyRef.freezeRotation = true;
            controller.rigidBodyRef.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // draw line to grapple point when attached & swinging
        DrawLine();

        // ensure swing input is off
        controller.swingInput = false;
    }

    // called every time the joint is destroyed
    private void OnJointBreak()
    {
        // reset & refreeze player's rotation
        transform.rotation = new Quaternion(0f, transform.rotation.y, 0f, transform.rotation.w);

        // if player cancelled swing with a jump, then
        if (controller.jumpedOutOfSwing)
        {
            controller.rigidBodyRef.AddForce(controller.model.up * controller.jumpForce, ForceMode.Impulse);
            controller.jumpedOutOfSwing = false;
        }
    }

    // find a grapple point to attach to & swing from
    private void Swing()
    {
        // Create a sphere around the player’s position with a given radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);

        // sort through each collided object to find a grapple point
        foreach (Collider hitCollider in hitColliders)
        {
            // the collided object is a grapple, so attach to it and start swinging
            if (hitCollider.CompareTag("Grapple"))
            {
                // begin swinging
                controller.swinging = true;

                // unfreeze player rotations
                controller.rigidBodyRef.constraints =  RigidbodyConstraints.None;

                // add a joint to the player so that they swing around the grapple point
                joint = gameObject.AddComponent<HingeJoint>();
                // set joint's anchor to this grapple point's position so that the player swings around it
                joint.anchor = hitCollider.transform.position - transform.position;
                // set joint's axis according to the object
                joint.axis = hitCollider.GetComponent<GrapplePoint>().axis;

                // create a variable to hold the limits of the joint
                JointLimits limits = joint.limits;
                // set limits to distance from grapple so that the player stays the same distance from the grapple point
                limits.min = Vector3.Distance(transform.position, hitCollider.transform.position);
                limits.max = Vector3.Distance(transform.position, hitCollider.transform.position);
                // set joint reference's limits & activate them
                joint.limits = limits;

                //break so it doesnt choose multiple points if more than one is detected
                print("Grapple point detected");
                print("anchor point = " + joint.connectedAnchor);
                break;
            }
        }
    }

    // debug for showing sphere
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }

    // draws rope
    public void DrawLine()
    {
        // if swinging draw a rope to the grapple position 
        if (controller.swinging)
        {
            // turn on line renderer since we're using it
            lineRenderer.enabled = true;

            //draws line from player to grapple point
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, joint.connectedAnchor);
        }
        else
        {
            // turn off line renderer when not swinging
            lineRenderer.enabled = false;
        }
    }
}