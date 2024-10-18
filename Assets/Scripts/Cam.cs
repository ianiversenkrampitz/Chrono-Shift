using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Monaghan, Devin
/// 5/8/2024
/// camera is controlled by mouse & looks at player
/// </summary>

public class Cam : MonoBehaviour
{
    // distance of camera from player on z axis
    public float distanceZ = 13f;
    // distance of camera from player on y axis
    public float distanceY = -5f;
    // sensitivity of x axis of mouse
    public float sensitivityX = 5f;
    // sensitivity of y axis of mouse
    public float sensitivityY = 4f;

    // reference to player
    [SerializeField] private Transform player;

    // reference to inputs
    private PlayerInputActions playerInputActions;

    // current x value of cam
    private float currentX = 0f;
    // current y value of cam
    private float currentY = 0f;
    // minimum allowed y value
    private const float minY = -40f;
    // maximum allowed y value
    private const float maxY = 30f;

    // Awake is called before the first frame update
    private void Awake()
    {
        // set cursor invisible
        Cursor.visible = false;

        // get inputs
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // put inputs in mouse variable
        Vector2 mouse = playerInputActions.PlayerActions.Mouse.ReadValue<Vector2>();
        // set current x and y with inputs and sensitivity
        currentX += mouse.x * sensitivityX * Time.deltaTime;
        currentY += mouse.y * sensitivityY * Time.deltaTime;

        // clamp current y value betwen min and max y value
        currentY = Mathf.Clamp(currentY, minY, maxY);

        Vector3 direction = new Vector3(0f, -distanceY, -distanceZ);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
        transform.position = player.position + rotation * direction;

        // make camera look at player
        transform.LookAt(player.position);
    }
}