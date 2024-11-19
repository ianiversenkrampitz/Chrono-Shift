using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 
 * Iversen-Krampitz, Ian 
 * 11/18/2024
 * controls on screen text box object 
 */

public class TextManager : MonoBehaviour
{
    public TextMeshProUGUI managerText;
    public GameObject textBox;

    // is the textbox actively displaying a text
    public bool displaying;

    // Start is called before the first frame update
    void Start()
    {
        managerText = GetComponentInChildren<TextMeshProUGUI>();
        managerText.text = ("");
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "TextBox")
            {
                textBox = child.gameObject;
            }
        }
        textBox.SetActive(false);
    }
}