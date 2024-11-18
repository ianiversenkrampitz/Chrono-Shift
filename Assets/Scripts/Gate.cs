using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Iversen-Krampitz, Ian 
 * 11/17/2024
 * controls keys and locked door. 
 */

public class Gate : MonoBehaviour
{
    public static int keyNum;
    public bool isGate;
    public GameObject textBox;
    private void Update()
    {
        //if this is a locked gate, set false when keys are 3 or more 
        //for some reason it counts the first key as 2, so 4 is the number
        if (keyNum >= 4 && isGate)
        {
            this.gameObject.SetActive(false);
            textBox.SetActive(false);
        }
    }
    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        //if this is a key, set this false and increase keycount
        if (collision.gameObject.CompareTag("Player") && !isGate)
        {
            keyNum++;
            Debug.Log("keys:" + keyNum);
            this.gameObject.SetActive(false);
        }
    }
}
