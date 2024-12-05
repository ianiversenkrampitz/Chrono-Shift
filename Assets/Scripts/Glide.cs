using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 12/3/2024
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
        // if the player inputs glide for the first time, normalize the vertical velocity

        // if the controller is inputting the glide, is off the ground, and not gliding
        if (controller.glideInput && !controller.onGround && !controller.grappling && !controller.jumpInputDelay)
        {
            controller.gliding = true;
            print("player is gliding");
        }
        if (!controller.glideInput)
        {
            controller.gliding = false;
        }
        if (controller.onGround)
        {
            controller.gliding = false;
        }
        if (controller.grappling)
        {
            controller.gliding = false;
        }
    }

   IEnumerator GravityDelay()
    {
        print("player began glide");
        yield return new WaitForSeconds(.25f);
        controller.gliding = true;
        print("glide gravity begun");
    }
}