using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Monaghan, Devin
/// 11/5/2024
/// handles menu buttons
/// handles scene transitions
/// </summary>

public class MenuManager : MonoBehaviour
{
    public int thisScene = 0;

    // quits game
    public void Quit()
    {
        Application.Quit();
        print("Player has quit game");
    }

    // moves to next scene
    public void NextScene()
    {
        SceneManager.LoadScene(thisScene + 1);
    }
}