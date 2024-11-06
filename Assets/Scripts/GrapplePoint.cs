using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    // is the grapple point front/back vs left/right
    public bool swingingOnXAxis = true;

    // axis value that will be put into the joint
    public Vector3 axis;

    public void Start()
    {
        if (swingingOnXAxis)
        {
            axis = new Vector3(1f, 0f, 0f);
        }
        else
        {
            axis = new Vector3(0f, 0f, 1f);
        }
    }
}