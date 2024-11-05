using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 10/29/2024
/// holds ninja variables
/// </summary>

public class NinjaStats : MonoBehaviour
{
    // movement speeds
    public float ninjaWalkSpeed = 100f;
    public float ninjaSprintSpeed = 100f;
    public float ninjaGlideSpeed = 85f;
    public float ninjaSlowSpeed = 1.25f;
    // gravity speed
    public float ninjaGravitySpeed = 35f;
    public float ninjaGlideGravity = 5f;
    // power of jump
    public float ninjaJumpForce = 15f;
    // power of dash
    public float ninjaDashForce = 50f;
    // power of sprint boost
    public float ninjaSprintForce = 3f;
    // maximum velocities
    public float ninjaWalkMaxVelocity = 17f;
    public float ninjaSprintMaxVelocity = 27f;
    public float ninjaGravityMaxVelocity = 30f;

    // holds reference to player
    public PlayerController controller;

    // Awake is called before the first frame update
    void Awake()
    {
        // movement speeds
        controller.walkSpeed = ninjaWalkSpeed;
        controller.sprintSpeed = ninjaSprintSpeed;
        controller.glideSpeed = ninjaGlideSpeed;
        controller.slowSpeed = ninjaSlowSpeed;
        // gravity speed
        controller.gravitySpeed = ninjaGravitySpeed;
        controller.glideGravity = ninjaGlideGravity;
        // power of jump
        controller.jumpForce = ninjaJumpForce;
        // power of dash
        controller.dashForce = ninjaJumpForce;
        // power of sprint boost
        controller.sprintForce = ninjaDashForce;
        // maximum velocities
        controller.walkMaxVelocity = ninjaWalkMaxVelocity;
        controller.sprintMaxVelocity = ninjaSprintMaxVelocity;
        controller.gravityMaxVelocity = ninjaGravityMaxVelocity;

    }
}