using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Monaghan, Devin
/// 12/10/2024
/// handles menu buttons
/// handles scene transitions
/// </summary>

public class MenuManager : MonoBehaviour
{
    // # of the current scene in the build index
    public int moveToScene = 0;

    public static int score;
    public static int nextScene;

    public TMP_Text endScore;

    public bool finaleLevel = false;

    private void Start()
    {
       if (endScore != null)
       {
            endScore.text = ("Score: " + score.ToString());
       }
    }

    // quits game
    public void Quit()
    {
        Application.Quit();
        print("Player has quit game");
    }

   /// <summary>
   /// move to intermission screen and save score and scene number
   /// </summary>
   /// <param name="levelScore"></param>
    public void SelectScene(int levelScore)
    {
        //sets static int to correct scene so intermission knows whats next 
        // store score
        score = levelScore;
        nextScene = moveToScene;
        //loads intermission 
        SceneManager.LoadScene(6);
    }

    public void FinishScene(int levelScore)
    {
        //sets static int to correct scene so intermission knows whats next 
        // store score
        score = levelScore;
        nextScene = 1;
        //loads intermission 
        SceneManager.LoadScene(7);
    }

    public void MoveToScene()
    {
        //check if theres no value for nextscene so title screen works 
        if (nextScene == 0)
        {
            nextScene = moveToScene;
        }
        SceneManager.LoadScene(nextScene);
    }
}