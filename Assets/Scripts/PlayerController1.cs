using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Monaghan, Devin
/// 5/8/2024
/// Handles deathfloor
/// handles WASD movement
/// handles jumping
/// handles boosts
/// </summary>

public class PlayerController1 : MonoBehaviour
{
    // movement speeds
    public float walkSpeed = 25f;
    public float sprintSpeed = 40f;
    public float floatSpeed = 20f;
    // gravity speed
    public float gravitySpeed = 15f;
    // power of jump
    public float jumpForce = 20f;
    // power of boost
    public float boostForce = 40f;
    // power of sprint boost
    public float sprintForce = 5f;
    // maximum sprint velocity
    public float sprintMaxVelocity = 50f;
    // maximum walk velocity
    public float walkMaxVelocity = 50f;
    // speed at which treads rotate into position
    public float treadsSpeed = 10f;

    // is the player actively jumping
    private bool floating = false;
    // is the player on the ground
    private bool onGround = true;
    // is the player sprinting or walking
    private bool sprinting = false;
    // is the player actively moving;
    private bool moving = false;

    // reference to treads model
    [SerializeField] private Transform treads;
    // reference to head model
    [SerializeField] private Transform head;
    // reference to camera
    [SerializeField] private Transform cam;
    // reference to inputs
    private PlayerInputActions playerInputActions;
    // reference to rigidbody
    private Rigidbody rigidBodyRef;

    // direction holds movement inputs converted into Vector3
    public Vector3 direction;

    // rotation of head model
    private Quaternion headRotation;

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
        // disallow the player to fall off the map
        Deathfloor();
        // check if the player is on the ground or not
        OnGround();
        // move
        Move();
        // jump and float
        Jump();
        // player falls in air
        Gravity();
        // slow down when not moving
        SlowDown();
        // boost dodge
        Boost();
        // align treads model with move direction and head model with cam rotation
        ModelAlign();
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
        // create variables for relative direction
        Vector3 forwardRelative = vectorWASD.y * camForward;
        Vector3 rightRelative = vectorWASD.x * camRight;

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
                rigidBodyRef.velocity.y, rigidBodyRef.velocity.z);
            rigidBodyRef.velocity = new Vector3(rigidBodyRef.velocity.x,
                Mathf.Clamp(rigidBodyRef.velocity.y, -sprintMaxVelocity, sprintMaxVelocity), rigidBodyRef.velocity.z);
            rigidBodyRef.velocity = new Vector3(rigidBodyRef.velocity.x, rigidBodyRef.velocity.y,
                Mathf.Clamp(rigidBodyRef.velocity.z, -sprintMaxVelocity, sprintMaxVelocity));
        }
        else
        {
            // apply force at walk speed
            rigidBodyRef.AddForce(direction * walkSpeed * Time.fixedDeltaTime, ForceMode.Impulse);

            // clamp velocity within walk max velocity
            rigidBodyRef.velocity = new Vector3(Mathf.Clamp(rigidBodyRef.velocity.x, -walkMaxVelocity, walkMaxVelocity),
                rigidBodyRef.velocity.y, rigidBodyRef.velocity.z);
            rigidBodyRef.velocity = new Vector3(rigidBodyRef.velocity.x,
                Mathf.Clamp(rigidBodyRef.velocity.y, -walkMaxVelocity, walkMaxVelocity), rigidBodyRef.velocity.z);
            rigidBodyRef.velocity = new Vector3(rigidBodyRef.velocity.x, rigidBodyRef.velocity.y,
                Mathf.Clamp(rigidBodyRef.velocity.z, -walkMaxVelocity, walkMaxVelocity));
        }        
    }

    // rotate treads model with direction
    // align head model with camera
    private void ModelAlign()
    {
        // make treads model look in direction player is heading
        treads.forward = new Vector3(direction.x, 0f, direction.z);

        // set storage variable to a Quaternion using cam.
        headRotation = Quaternion.Euler(0f, cam.rotation.eulerAngles.y, 0f);
        // set head model rotation to headRotation variable
        head.rotation = headRotation;
    }

    // when player is not moving rapidly slow down
    private void SlowDown()
    {
        if (!Moving() && rigidBodyRef.velocity != Vector3.zero)
        {
            rigidBodyRef.velocity /= 1.03f;
        }
    }

    // get sprint input
    // when player starts sprinting enter sprinting state
    // when player stops inputting movements exit sprinting state
    // return true in springting state and false out of sprinting state
    private bool Moving()
    {
        // get movement input
        Vector2 vectorWASD = playerInputActions.PlayerActions.MoveWASD.ReadValue<Vector2>();

        // if player starts moving enter moving state
        if (vectorWASD != Vector2.zero)
        {
            moving = true;
        }
        // if player stops movement exit moving state
        else
        {
            moving = false;
        }

        return moving;
    }

    // get sprint input
    // when player presses control start sprinting
    // when player starts sprinting enter sprinting state signified with sprinting bool
    // when player stops inputting movements exit sprinting state
    // return true in springting state and false out of sprinting state
    private bool Sprinting()
    {
        // get movement input
        Vector2 vectorWASD = playerInputActions.PlayerActions.MoveWASD.ReadValue<Vector2>();

        // if player presses ctrl start sprinting
        if (playerInputActions.PlayerActions.Sprint.WasPerformedThisFrame())
        {
            sprinting = true;
            rigidBodyRef.AddForce(treads.forward * sprintForce, ForceMode.Impulse);
        }
        // if player stops movement stop sprinting
        if (vectorWASD == Vector2.zero)
        {
            sprinting = false;
        }

        return sprinting;
    }

    // check if player is on the ground via raycast
    private void OnGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.6f))
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }
    }

    // if the player is not on the ground or jumping apply force down
    private void Gravity()
    {
        if (!floating && !onGround)
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
        if (onGround && playerInputActions.PlayerActions.Jump.WasPerformedThisFrame())
        {
            rigidBodyRef.AddForce(treads.up * jumpForce, ForceMode.Impulse);
        }
        // if the player is not on the ground and holds space bar, continuosly provide small lift
        if (playerInputActions.PlayerActions.Jump.IsPressed())
        {
            rigidBodyRef.AddForce(treads.up * floatSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
            floating = true;
        }
        else
        {
            floating = false;
        }
    }

    // get boost inputs
    // if player presses space apply a force forwards
    private void Boost()
    {
        // if the player is on the ground and presses shift, boost
        if (playerInputActions.PlayerActions.Dash.WasPerformedThisFrame())
        {
            rigidBodyRef.AddForce(treads.forward * boostForce, ForceMode.Impulse);
        }
    }

    // checks if player has fallen off the floor and teleports them back to origin, resetting force
    private void Deathfloor()
    {
        if (transform.position.y <= -15)
        {
            // reset position to origin
            transform.position = Vector3.zero;
            // reset rotation
            transform.rotation = Quaternion.Euler(Vector3.zero);
            // reset momentum
            rigidBodyRef.velocity = Vector3.zero;
            print("player died");
        }
    }
}