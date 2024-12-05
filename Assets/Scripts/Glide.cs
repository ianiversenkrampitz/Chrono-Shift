using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 12/5/2024
/// handles glide input
/// </summary>

public class Glide : MonoBehaviour
{
    // holds reference to player
    public PlayerController controller;

    // is this the player's first glide input
    public bool firstGlideFrame = true;

    // handles physics controlled movement
    protected virtual void FixedUpdate()
    {
        Gliding();
    }

    public void Gliding()
    {
        // if the player is inputting the glide, is off the ground, is not grappling, and has 
        if (controller.glideInput && !controller.onGround && !controller.grappling && !controller.jumpInputDelay)
        {
            controller.gliding = true;
            print("player is gliding");
        }
        // if the player inputs glide for the first time, set the vertical velocity to 0
        // so the gravity isn't affected by any momentum going into the glide
        if (controller.glideInput && !controller.onGround && !controller.grappling
            && !controller.jumpInputDelay && firstGlideFrame)
        {
            firstGlideFrame = false;
            controller.rigidBodyRef.velocity = new Vector3(controller.rigidBodyRef.velocity.x,
                0f, controller.rigidBodyRef.velocity.z);
        }
        // if the player stops inputting the glide, touches the ground, or begins grappling, stop gliding
        if (!controller.glideInput)
        {
            controller.gliding = false;
        }
        if (controller.onGround)
        {
            controller.gliding = false;
            // the player has stopped gliding, so reset glide timer s;dlfkja ;sflsadj f;lasjd f;lkas
            firstGlideFrame = true;
        }
        if (controller.grappling)
        {
            controller.gliding = false;
        }
    }
}