using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Iversen-Krampitz, Ian 
 * 11/10/2024
 * changes text box on canvas to trigger's message 
 */

public class Text : MonoBehaviour
{
    public TextMeshProUGUI boxText;
    //text to be added to specific trigger
    public string thisText;
    //variables to easily customize the text in the box
    public float fontSize;
    public Color textColor;
    //variables to customize amount of time text stays on screen
    //change this bool and float in the inspector for each object 
    public bool timedText;
    public float textTime;
    public TextManager textManager;

    private void Start()
    {
        textManager = FindObjectOfType<TextManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //checks if hitting the player, if so show box and set text
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit a text trigger");
            textManager.textBox.SetActive(true);
            textManager.managerText.text = thisText;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //if exiting trigger, set text to false or start timer if timed box
        if (other.gameObject.CompareTag("Player"))
        {
            if (timedText)
            {
                StartCoroutine(TextTimer());
            }
            else
            {
                textManager.textBox.SetActive(false);
                textManager.managerText.text = ("");
            }
        }
    }
    /// <summary>
    /// optional timer for text to remain on screen 
    /// </summary>
    /// <returns></returns>
    public IEnumerator TextTimer()
    {
        yield return new WaitForSeconds(textTime);
        textManager.textBox.SetActive(false);
        textManager.managerText.text = ("");
    }
}
