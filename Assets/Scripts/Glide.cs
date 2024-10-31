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
        if (controller.playerInputActions.PlayerActions.Jump.WasPerformedThisFrame()
            && !controller.jumpInputCooldown && !controller.onGround)
        {
            controller.gliding = true;
            print("player is gliding");

            // start jump input cooldown
            StartCoroutine(controller.JumpInputCooldown());
        }
        else if (!controller.Moving())
        {
            controller.gliding = false;
        }
        else if (controller.onGround)
        {
            controller.gliding = false;
        }
    }
}