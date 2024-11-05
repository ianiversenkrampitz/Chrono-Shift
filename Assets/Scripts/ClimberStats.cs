using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 11/5/2024
/// holds ice climber variables
/// </summary>

public class ClimberStats : MonoBehaviour
{
    // movement speeds
    public float climberWalkSpeed = 100f;
    public float climberSprintSpeed = 100f;
    public float climberGlideSpeed = 85f;
    public float climberSlowSpeed = 1.25f;
    // gravity speed
    public float climberGravitySpeed = 35f;
    public float climberGlideGravity = 5f;
    // power of jump
    public float climberJumpForce = 15f;
    // power of dash
    public float climberDashForce = 50f;
    // power of sprint boost
    public float climberSprintForce = 3f;
    // maximum velocities
    public float climberWalkMaxVelocity = 17f;
    public float climberSprintMaxVelocity = 27f;
    public float climberGravityMaxVelocity = 30f;

    // holds reference to player
    public PlayerController controller;

    // Awake is called before the first frame update
    void Awake()
    {
        // movement speeds
        controller.walkSpeed = climberWalkSpeed;
        controller.sprintSpeed = climberSprintSpeed;
        controller.glideSpeed = climberGlideSpeed;
        controller.slowSpeed = climberSlowSpeed;
        // gravity speed
        controller.gravitySpeed = climberGravitySpeed;
        controller.glideGravity = climberGlideGravity;
        // power of jump
        controller.jumpForce = climberJumpForce;
        // power of dash
        controller.dashForce = climberJumpForce;
        // power of sprint boost
        controller.sprintForce = climberDashForce;
        // maximum velocities
        controller.walkMaxVelocity = climberWalkMaxVelocity;
        controller.sprintMaxVelocity = climberSprintMaxVelocity;
        controller.gravityMaxVelocity = climberGravityMaxVelocity;
    }
}