using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 10/29/2024
/// handles glide input
/// </summary>

public class Glide : MonoBehaviour
{
    // holds reference to player
    public PlayerController controller;

    // handles physics controlled movement
    protected virtual void FixedUpdate()
    {
        Gliding();
    }

    public void Gliding()
    {
        if (controller.glideInput && !controller.onGround)
        {
            controller.gliding = true;
            print("player is gliding");

            // halt upward momentum
            Vector3 clampedVelocity = controller.rigidBodyRef.velocity.normalized;
            controller.rigidBodyRef.velocity = new Vector3(controller.rigidBodyRef.velocity.x,
                0f, controller.rigidBodyRef.velocity.z);
        }
        else if (!controller.Moving())
        {
            controller.gliding = false;
        }
        else if (controller.onGround)
        {
            controller.gliding = false;
        }
        controller.glideInput = false;
    }
}