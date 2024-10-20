using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;
 
/// <summary>
/// Monaghan, Devin
/// Iversen-Krampitz, Ian 
/// 10/19/2024
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

    // reference to model
    [SerializeField] private Transform model;
    // reference to camera
    [SerializeField] private Transform cam;
    // reference to inputs
    private PlayerInputActions playerInputActions;

    // direction holds movement inputs converted into Vector3
    public Vector3 direction;

    // Awake is called before the first frame update
    void Awake()
    {
        // get rigidbody reference
        rigidBodyRef = this.GetComponent<Rigidbody>();

        // enable inputs
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    // handles physics controlled movement
    private void FixedUpdate()
    {
        // check if the player is on the ground or not
        OnGround();
        // move when not dashing
        if (!dashing)
        {
            Move();
        }
        // dash
        Dash();
        // jump
        Jump();
                      /// is this necessary?? who knows?
        // player falls in air
        Gravity();
        // slow down when not moving or dashing
        SlowDown();
        // align model rotation
       // ModelAlign();
    }

    // get movement inputs
    // move via applying force to rigidbody
    private void Move()
    {
        // get movement input values
        Vector2 vectorWASD = playerInputActions.PlayerActions.MoveWASD.ReadValue<Vector2>();

        // create variables holding camera transform values
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        // remove y values
        camForward.y = 0f;
        camRight.y = 0f;
        // create variables for direction of model relative to camera
        Vector3 forwardRelative = vectorWASD.y * camForward;
        Vector3 rightRelative = vectorWASD.x * camRight;
        // set direction
        direction = forwardRelative + rightRelative;
        direction.y = 0f;

        // apply force
        // clamp velocity
        if (Sprinting())
        {
            // apply force at sprint speed
            rigidBodyRef.AddForce(direction * sprintSpeed * Time.fixedDeltaTime, ForceMode.Impulse);

            // clamp velocity within sprint max velocity
            rigidBodyRef.velocity = new Vector3(Mathf.Clamp(rigidBodyRef.velocity.x, -sprintMaxVelocity, sprintMaxVelocity),
                Mathf.Clamp(rigidBodyRef.velocity.y, -sprintMaxVelocity, sprintMaxVelocity),
                Mathf.Clamp(rigidBodyRef.velocity.z, -sprintMaxVelocity, sprintMaxVelocity));
        }
        else 
        {
            // apply force at walk speed
            rigidBodyRef.AddForce(direction * walkSpeed * Time.fixedDeltaTime, ForceMode.Impulse);

            // clamp velocity within walk max velocity
            rigidBodyRef.velocity = new Vector3(Mathf.Clamp(rigidBodyRef.velocity.x, -walkMaxVelocity, walkMaxVelocity),
                Mathf.Clamp(rigidBodyRef.velocity.y, -walkMaxVelocity, walkMaxVelocity), 
                Mathf.Clamp(rigidBodyRef.velocity.z, -walkMaxVelocity, walkMaxVelocity));
        }        
    }

    // rotate model with direction it is moving regardless of camera direction
    private void ModelAlign()
    {
        // make model look in direction player is heading
        model.forward = new Vector3(direction.x, 0f, direction.z);
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

            // Assign the new velocity to the rigidbody
            rigidBodyRef.velocity = newVelocity;
        }
    }

    // get sprint input
    // when player starts movign enter moving state
    // when player stops inputting movements exit moving state
    private bool Moving()
    {
        // get movement input
        Vector2 vectorWASD = playerInputActions.PlayerActions.MoveWASD.ReadValue<Vector2>();

        // if player is entering movement inputs, enter moving state
        if (vectorWASD != Vector2.zero)
        {
            moving = true;
        }
        // if player stops movement exit moving state
        else
        {
            moving = false;
        }

        // return true in moving state and false out of moving state
        return moving;
    }

    // get sprint input
    // when player presses control start sprinting
    // when player stops inputting movements exit sprinting state
    private bool Sprinting()
    {
        // get movement input
        Vector2 vectorWASD = playerInputActions.PlayerActions.MoveWASD.ReadValue<Vector2>();

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
            rigidBodyRef.AddForce(Vector3.down * jumpForce * Time.fixedDeltaTime, ForceMode.Impulse);
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

    // get dash inputs
    // if player presses shift apply a force forwards
    private void Dash()
    {
        // if the player is on the ground and presses shift, dash
        if (playerInputActions.PlayerActions.Dash.WasPerformedThisFrame() && !dashCooldown)
        {
            // set dashing to true
            dashing = true;
            // apply force
            rigidBodyRef.AddForce(model.forward * dashForce, ForceMode.Impulse);
            //begin dash cooldown
            StartCoroutine(DashTimer());
            StartCoroutine(DashCooldownTimer());
        }
    }
     
    // timer for dash
    IEnumerator DashTimer()
    {
        Debug.Log("dash has started");
        // begin dash cooldown
        dashCooldown = true;
        // wait 1 second
        yield return new WaitForSeconds(.5f);
        // stop dashing
        dashing = false;
        Debug.Log("dash has ended");
    }

    // timer for dash cooldown
    IEnumerator DashCooldownTimer()
    {
        Debug.Log("dash cooldown has started");
        // wait 5 seconds for dash cooldown
        yield return new WaitForSeconds(2.5f);
        // turn off dashCooldown
        dashCooldown = false;
        Debug.Log("dash cooldown has ended");
    }
}