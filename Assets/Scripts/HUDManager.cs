using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Monaghan, Devin
/// 11/18/2024
/// handles perpetual ui display
/// </summary>

public class HUDManager : MonoBehaviour
{
    // reference to player controller
    public PlayerController controller;

    // reference to health display
    public TMP_Text healthDisplay;
    // reference to score display
    public TMP_Text scoreDisplay;

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        healthDisplay.text = "Health: " + controller.health + "/3";
        scoreDisplay.text = "Score: " + controller.score;
    }
}