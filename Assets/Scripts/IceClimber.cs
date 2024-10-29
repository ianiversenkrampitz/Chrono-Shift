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
    public Vector3 pointDistance;
    public Vector3 pointPosition;
    public Material lineMat;
    public int lengthOfLineRenderer = 20;
    public bool isGrappling;

    // Start is called before the first frame update
    void Start()
    {
        //for debug sphere drawing
        Gizmos.color = Color.red;
        //for drawing line from point to player 
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;
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
                Grapple();
            }
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
                pointDistance = (hitCollider.transform.position - transform.position);
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
        if (playerInputActions.PlayerActions.Swing.IsPressed())
        {
            isGrappling = true;
            //swing towards center of object with fixed momentum 
            //check if the player's distance is farther than the original distance 
            //written in pointDistance and keep from moving or break 
            if (pointDistance.x > transform.position.x - pointPosition.x)
            {
                
            }
            Debug.Log("pointdistance is " + pointDistance);
        }
        //break if not still held 
        else
        {
            isGrappling = false;
            yield return null; 
        }
    }
}
