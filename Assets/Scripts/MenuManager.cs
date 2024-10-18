using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Monaghan, Devin
/// 5/8/2024
/// Handles menu buttons
/// </summary>

public class MenuManager : MonoBehaviour
{
    // quits game
    public void Quit()
    {
        Application.Quit();
        print("Player has quit game");
    }

    // starts game
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
}