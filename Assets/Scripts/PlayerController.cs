using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
/// <summary>
/// Monaghan, Devin
/// Iversen-Krampitz, Ian 
/// 10/24/2024
/// Handles deathfloor
/// handles WASD movement
/// handles jumping
/// handles dash
/// </summary>

public class PlayerController : MonoBehaviour
{
    // movement speeds
    public float walkSpeed = 50f;
    public float sprintSpeed = 100f;
    public float slowSpeed = .01f;

    // gravity speed
    public float gravitySpeed = 15f;
    // power of jump
    public float jumpForce = 20f;
    // power of dash
    public float dashForce = 30f;
    // power of sprint boost
    public float sprintForce = 2f;
    // maximum walk velocity
    public float walkMaxVelocity = 2f;
    // maximum sprint velocity
    public float sprintMaxVelocity = 4f;

    // is the player on the ground
    public bool onGround;
    // is the player sprinting or walking
    private bool sprinting = false;
    // is the player actively moving;
    public bool moving = false;
    // is the player actively dashing
    public bool dashing = false;
    // is the dash on cooldown
    public bool dashCooldown = false;

    // reference to rigidbody
    public Rigidbody rigidBodyRef;
    // reference to inputs
    public PlayerInputActions playerInputActions;

    // reference to model
    public Transform model;

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

    // handles physics controlled movement
    protected virtual void FixedUpdate()
    {
        // check if the player is on the ground or not
        OnGround();
        // move when not dashing
        if (!dashing)
        {
            Move();
        }
        // jump
        Jump();
                      /// is this necessary?? who knows?
                      /// i think so, lets keep it in mind tho
        // player falls in air
        Gravity();
        // slow down when not moving or dashing
        SlowDown();
        // align model rotation
        ModelAlign();
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
            if (Sprinting())
            {
                // apply force at sprint speed
                rigidBodyRef.AddForce(direction * sprintSpeed, ForceMode.Acceleration);

                // clamp velocity within sprint max speed
                if (rigidBodyRef.velocity.magnitude > sprintMaxVelocity && onGround)
                {
                    clampedVelocity = rigidBodyRef.velocity.normalized * sprintMaxVelocity;
                    rigidBodyRef.velocity = new Vector3(clampedVelocity.x, rigidBodyRef.velocity.y, clampedVelocity.z);
                }
            }
            else
            {
                // apply force at walk speed
                rigidBodyRef.AddForce(direction * walkSpeed, ForceMode.Acceleration);

                // clamp velocity within walk max speed
                if (rigidBodyRef.velocity.magnitude > walkMaxVelocity && onGround)
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
    private bool Moving()
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
        if (playerInputActions.PlayerActions.Sprint.WasPerformedThisFrame())
        {
            sprinting = true;
            rigidBodyRef.AddForce(model.forward * sprintForce, ForceMode.Impulse);
        }
        // if player stops movement stop sprinting
        if (!Moving())
        {
            sprinting = false;
        }

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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.51f))
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }
    }

    // if the player is not on the ground apply force down
    private void Gravity()
    {
        if (!onGround)
        {
            rigidBodyRef.AddForce(Vector3.down * gravitySpeed, ForceMode.Acceleration);
        }
    }

    // get jump inputs
    // if player presses space apply a force up
    // if player keeps pressing space float via continuous 
    private void Jump()
    {
        // if the player is on the ground and presses the spacebar, jump
        if (onGround && playerInputActions.PlayerActions.Jump.IsPressed())
        {
            rigidBodyRef.AddForce(model.up * jumpForce, ForceMode.Impulse);
        }
    }
}