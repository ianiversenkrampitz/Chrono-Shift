using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    public static int score;
    public static int nextScene;
    public TMP_Text endScore;
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
       
        nextScene = moveToScene;
        score = levelScore;
        //loads intermission 
        SceneManager.LoadScene(6);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextScene);
    }
}