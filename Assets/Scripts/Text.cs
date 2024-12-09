using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 
 * Iversen-Krampitz, Ian 
 * 11/18/2024
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
    //check this bool in inspector to make box accessible once only
    public bool oneTime;
    //variables to customize amount of time text stays on screen
    //change this bool and float in the inspector for each object 
    public bool timedText;
    public float textTime;
    public TextManager textManager;
    public GameObject parentToDestroy;

    private void Start()
    {
        textManager = FindObjectOfType<TextManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if hitting the player, if so show box and set text
        if (other.gameObject.CompareTag("PlayerTag"))
        {
            Debug.Log("hit a text trigger");
            textManager.displaying = true;
            textManager.textBox.SetActive(true);
            textManager.managerText.text = thisText;
            textManager.managerText.color = textColor;
            textManager.managerText.fontSize = fontSize;    
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if exiting trigger, set text to false or start timer if timed box
        if (other.gameObject.CompareTag("PlayerTag"))
        {
            textManager.displaying = false;

            if (timedText)
            {
                StartCoroutine(TextTimer());
            }
            else if (oneTime)
            {
                textManager.textBox.SetActive(false);
                textManager.managerText.text = ("");
                if (parentToDestroy != null)
                {
                    Debug.Log("destroyed parent");
                    Destroy(parentToDestroy);
                }
                Debug.Log("Destroyed trigger");
                Destroy(this.gameObject);
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

        if (!textManager.displaying)
        {
            textManager.textBox.SetActive(false);
            textManager.managerText.text = ("");
        }
        if (oneTime)
        {
            if (parentToDestroy != null)
            {
                Debug.Log("destroyed parent");
                Destroy(parentToDestroy);
            }
            Debug.Log("Destroyed trigger");
            Destroy(this.gameObject);
        }
    }
}