using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

/// <summary>
/// Monaghan, Devin
/// 11/2/2024
/// handles dash
/// </summary>

public class Dash : MonoBehaviour
{    
    [SerializeField] private PlayerController controller;

    // handles physics controlled movement
    private void FixedUpdate()
    {
        DashMove();
    }

        // get dash inputs
        // if player presses shift apply a force forwards
    private void DashMove()
    {
        // if the player is on the ground and presses shift, dash
        if (controller.playerInputActions.PlayerActions.Dash.WasPerformedThisFrame() && !controller.dashCooldown)
        {
            // apply force
            controller.rigidBodyRef.AddForce(controller.direction * controller.dashForce, ForceMode.Impulse);

            //begin dash timers
            StartCoroutine(DashTimer());
            StartCoroutine(DashCooldownTimer());
        }
    }

    // timer for dash
    IEnumerator DashTimer()
    {
        // set dashing to true
        controller.dashing = true;
        Debug.Log("dash has started");
        // wait .25 second
        yield return new WaitForSeconds(.25f);
        // stop dashing
        controller.dashing = false;
        Debug.Log("dash has ended");
    }

    // timer for dash cooldown
    IEnumerator DashCooldownTimer()
    {
        // set dash cooldown to true
        controller.dashCooldown = true;
        Debug.Log("dash cooldown has started");
        // wait .25 seconds for dash, wait 5 seconds for dash cooldown
        yield return new WaitForSeconds(controller.dashCoolTime);
        // turn off dashCooldown
        controller.dashCooldown = false;
        Debug.Log("dash cooldown has ended");
    }
}