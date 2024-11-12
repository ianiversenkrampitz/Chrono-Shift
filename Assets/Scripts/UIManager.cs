using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 11/11/2024
/// handles perpetual ui display
/// </summary>

public class UIManager : MonoBehaviour
{
    // reference to player controller
    public PlayerController controller;

    // reference to health display
    public TMP_Text healthDisplay;

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        healthDisplay.text = "Health: " + controller.health + "/3";
    }
}