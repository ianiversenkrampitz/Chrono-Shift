using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 10/29/2024
/// holds vampire variables
/// </summary>

public class VampireStats : MonoBehaviour
{
    // movement speeds
    public float vampireWalkSpeed = 750f;
    public float vampireSprintSpeed = 100f;
    public float vampireGlideSpeed = 85f;
    public float vampireSlowSpeed = 1.25f;
    // gravity speed
    public float vampireGravitySpeed = 32f;
    public float vampireGlideGravity = 3f;
    // power of jump
    public float vampireJumpForce = 22f;
    // power of dash
    public float vampireDashForce = 40f;
    // power of sprint boost
    public float vampireSprintForce = 3f;
    // maximum velocities
    public float vampireWalkMaxVelocity = 17f;
    public float vampireSprintMaxVelocity = 27f;
    public float vampireGravityMaxVelocity = 30f;

    // holds reference to player
    public PlayerController controller;

    // Awake is called before the first frame update
    void Awake()
    {
        // movement speeds
        controller.walkSpeed = vampireWalkSpeed;
        controller.sprintSpeed = vampireSprintSpeed;
        controller.glideSpeed = vampireGlideSpeed;
        controller.slowSpeed = vampireSlowSpeed;
        // gravity speed
        controller.gravitySpeed = vampireGravitySpeed;
        controller.glideGravity = vampireGlideGravity;
        // power of jump
        controller.jumpForce = vampireJumpForce;
        // power of dash
        controller.dashForce = vampireJumpForce;
        // power of sprint boost
        controller.sprintForce = vampireDashForce;
        // maximum velocities
        controller.walkMaxVelocity = vampireWalkMaxVelocity;
        controller.sprintMaxVelocity = vampireSprintMaxVelocity;
        controller.gravityMaxVelocity = vampireGravityMaxVelocity;
    }
}