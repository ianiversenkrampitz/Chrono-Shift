using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 11/19/2024
/// handles glide input
/// </summary>

public class Spin : MonoBehaviour
{
    public float speed = 50f;

    // Update is called once per physics frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}