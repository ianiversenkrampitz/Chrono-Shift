using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

/// <summary>
/// Monaghan, Devin
/// Iversen-Krampitz, Ian 
/// 11/4/2024
/// handles deathfloor
/// handles WASD movement
/// handles jumping
/// handles dash
/// </summary>

public class PlayerController : MonoBehaviour
{
    // movement speeds
    public float walkSpeed = 50f;
    public float sprintSpeed = 100f;
    public float glideSpeed = 90f;
    public float slowSpeed = .01f;
    // gravity speed
    public float gravitySpeed = 15f;
    public float glideGravity = 5f;
    // power of jump
    public float jumpForce = 20f;
    // power of dash
    public float dashForce = 30f;
    // power of sprint boost
    public float sprintForce = 2f;
    // maximum velocities
    public float walkMaxVelocity = 2f;
    public float sprintMaxVelocity = 4f;
    public float gravityMaxVelocity = 30f;
    // # of seconds where player cannot input jump after already inputting
    public float jumpInputDelay = .25f;
    // # of seconds of dash cooldown
    public float dashCoolTime = 2f;

    // is the player on the ground
    public bool onGround;
    // is the player sprinting or walking
    public bool sprinting = false;
    // is the player actively moving;
    public bool moving = false;
    // is the player actively dashing
    public bool dashing = false;
    // is the dash on cooldown
    public bool dashCooldown = false;
    // is the player currently gliding
    public bool gliding;
    // did the player just press jump
    public bool hasJumped;
    // did the player input these actions?
    public bool dashInput = false;
    public bool glideInput = false;
    public bool sprintInput = false;
    public bool jumpInput = false;
    public bool swingInput = false;

    // reference to rigidbody
    public Rigidbody rigidBodyRef;
    // reference to inputs
    public PlayerInputActions playerInputActions;

    // reference to model
    public Transform model;

    // reference to capsule
    public GameObject capsule;

    // references to different color materials
    public Material red;
    public Material green;
    public Material blue;
    public Material silver;

    // direction holds movement inputs converted into Vector3
    public Vector3 direction;

    // reference to camera
    [SerializeField] private Transform mainCam;

    // holds movement inputs
    private Vector2 vectorWASD;

    // Awake is called before the first frame update
    void Awake()
    {
        // hide cursor
        Cursor.visible = false;

        // get rigidbody reference
        rigidBodyRef = this.GetComponent<Rigidbody>();

        // enable inputs
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        print(mainCam.forward);
    }

    // called every frame
    private void Update()
    {
        Inputs();
    }

    // called every fixed frame
    protected virtual void FixedUpdate()
    {
        // check if the player is on the ground or not
        OnGround();
        // check for if player is trying to sprint
        Sprinting();
        // move when not dashing
        if (!dashing)
        {
            Move();
        }
        // jump
        Jump();
        // player falls in air
        Gravity();
        // slow down when not moving or dashing
        SlowDown();
        // align model rotation
        ModelAlign();
        // change color to provide feedback
        ColorChange();
    }

    // get movement inputs
    // move via applying force to rigidbody
    private void Move()
    {
        // get movement input values
        Vector2 vectorWASD = playerInputActions.PlayerActions.MoveWASD.ReadValue<Vector2>();

        // create variables holding camera transform values
        Vector3 forward = mainCam.forward;
        Vector3 right = mainCam.right;
        // remove y values
        forward.y = 0f;
        right.y = 0f;
        // normalize values
        forward.Normalize();
        right.Normalize();
        // set direction
        direction = (forward * vectorWASD.y + right * vectorWASD.x).normalized;

        if (Moving())
        {
            // holds clamped Velocity of player
            Vector3 clampedVelocity;

            // apply force
            // clamp velocity
            if (sprinting)
            {
                // apply force at sprint speed
                rigidBodyRef.AddForce(direction * sprintSpeed, ForceMode.Acceleration);

                // normalize velocity to preserve direction
                // set velocity to max sprint velocity
                if (rigidBodyRef.velocity.magnitude > sprintMaxVelocity)
                {
                    clampedVelocity = rigidBodyRef.velocity.normalized * sprintMaxVelocity;
                    rigidBodyRef.velocity = new Vector3(clampedVelocity.x, rigidBodyRef.velocity.y, clampedVelocity.z);
                }
            }
            else
            {
                // apply force at walk speed
                rigidBodyRef.AddForce(direction * walkSpeed, ForceMode.Acceleration);

                // normalize velocity to preserve direction
                // set velocity to max walk velocity
                if (rigidBodyRef.velocity.magnitude > walkMaxVelocity)
                {
                    clampedVelocity = rigidBodyRef.velocity.normalized * walkMaxVelocity;
                    rigidBodyRef.velocity = new Vector3(clampedVelocity.x, rigidBodyRef.velocity.y, clampedVelocity.z);
                }
            }
        }
    }

    // rotate model with direction it is moving regardless of camera direction
    private void ModelAlign()
    {
        // make model look in direction player is heading only when moving
        if (Moving())
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            model.rotation = Quaternion.RotateTowards(model.rotation, toRotation, 720 * Time.deltaTime);
        }
    }

    // when player is not moving rapidly slow down (only for x and z movement)
    private void SlowDown()
    {
        if (!Moving() && !dashing && rigidBodyRef.velocity != Vector3.zero)
        {
            // Store the y component to maintain it
            float currentYVelocity = rigidBodyRef.velocity.y;

            // Create a new Vector3 with scaled x and z components, and preserve y
            Vector3 newVelocity = new Vector3(rigidBodyRef.velocity.x / slowSpeed,
                currentYVelocity, rigidBodyRef.velocity.z / slowSpeed);

            // if player is moving to slowly stop entirely
            if (newVelocity.x <= 0.1f && newVelocity.x >= -0.1f)
            {
                newVelocity.x = 0f;
            }
            if (newVelocity.z <= 0.1f && newVelocity.z >= -0.1f)
            {
                newVelocity.z = 0f;
            }

            // Assign the new velocity to the rigidbody
            rigidBodyRef.velocity = newVelocity;
        }
    }

    // get sprint input
    // when player starts moving enter moving state
    // when player stops inputting movements exit moving state
    public bool Moving()
    {
        // get movement input
        vectorWASD = playerInputActions.PlayerActions.MoveWASD.ReadValue<Vector2>();

        // if player is entering movement inputs, return true
        if (vectorWASD != Vector2.zero)
        {
            return true;
        }
        // if player stops movement return false
        else
        {
            return false;
        }
    }

    // get sprint input
    // when player presses control start sprinting
    // when player stops inputting movements exit sprinting state
    private bool Sprinting()
    {
        // if player presses ctrl start sprinting
        if (sprintInput)
        {
            sprinting = true;
            rigidBodyRef.AddForce(model.forward * sprintForce, ForceMode.Impulse);
        }
        // if player stops movement stop sprinting
        if (!Moving())
        {
            sprinting = false;
        }

        // ensure sprint input is off
        sprintInput = false;

        // return true in sprinting state and false out of sprinting state
        return sprinting;
    }
       
    // check if player is on the ground via raycast
    private void OnGround()
    {
        // create new raycast
        RaycastHit hit;

        // send raycast down beneath model
        // if raycast collides with anything mark player as on the ground
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.505f))
        {
            onGround = true;
            // the player is back on the ground so they may jump again
            hasJumped = false;
        }
        else
        {
            onGround = false;
        }
    }

    // if the player is not on the ground apply force down
    private void Gravity()
    {
        // if gliding apply reduced gravity
        if (gliding)
        {
            // turn off physics gravity when not on ground
            rigidBodyRef.useGravity = false;

            Vector3 gravityDirection = transform.position;
            gravityDirection.y -= glideGravity * Time.deltaTime;
            transform.position = gravityDirection;
        }
        // apply gravity when not on ground
        else if (!onGround)
        {
            // turn off physics gravity when not on ground
            rigidBodyRef.useGravity = false;
            // turn off physics gravity when not on ground
            rigidBodyRef.AddForce(Vector3.down * gravitySpeed, ForceMode.Acceleration);
        }
        else
        {
            // use unity gravity on ground to make sure the player is proplerly on the floor
            rigidBodyRef.useGravity = true;
        }
        // clamp gravity speed
        Vector3 clampedVelocity;
        if (rigidBodyRef.velocity.magnitude > gravityMaxVelocity)
        {
            clampedVelocity = rigidBodyRef.velocity.normalized * gravityMaxVelocity;
            rigidBodyRef.velocity = new Vector3(rigidBodyRef.velocity.x, clampedVelocity.y, rigidBodyRef.velocity.z);
        }
    }

    // get jump inputs
    // if player presses space apply a force up
    // if player keeps pressing space float via continuous 
    private void Jump()
    {
        // if the player is on the ground and presses the spacebar, jump
        if (onGround && jumpInput && !hasJumped)
        {
            rigidBodyRef.AddForce(model.up * jumpForce, ForceMode.Impulse);

            // set hasJumped to true so that you can only jump once
            hasJumped = true;
        }

        // ensure jump input is off
        jumpInput = false;
    }

    // sets player color differently depending on what action they are performing
    private void ColorChange()
    {
        if (dashCooldown)
        {
            capsule.GetComponent<Renderer>().material = red;
            return;
        }
        else if (gliding)
        {
            capsule.GetComponent<Renderer>().material = blue;
            return;
        }
        else if (sprinting)
        {
            capsule.GetComponent<Renderer>().material = green;
        }
        else
        {
            capsule.GetComponent<Renderer>().material = silver;
        }
    }

    // gets inputs in Update to be used in Fixed Update
    private void Inputs()
    {
        if (playerInputActions.PlayerActions.Dash.IsPressed())
        {
            dashInput = true;
        }
        if (playerInputActions.PlayerActions.Glide.IsPressed())
        {
            glideInput = true;
        }
        if (playerInputActions.PlayerActions.Sprint.IsPressed())
        {
            sprintInput = true;
        }
        if (playerInputActions.PlayerActions.Jump.IsPressed())
        {
            jumpInput = true;
        }
        if (playerInputActions.PlayerActions.Swing.IsPressed())
        {
            swingInput = true;
        }
    }
}