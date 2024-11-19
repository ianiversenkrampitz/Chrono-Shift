using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Monaghan, Devin
/// 11/18/2024
/// handles menu buttons
/// handles scene transitions
/// </summary>

public class MenuManager : MonoBehaviour
{
    // # of the current scene in the build index
    public int moveToScene = 0;

    // quits game
    public void Quit()
    {
        Application.Quit();
        print("Player has quit game");
    }

    // moves to scene number sceneNumber in the build index
    public void SelectScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}