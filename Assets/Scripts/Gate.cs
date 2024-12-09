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
    public static int keyNum = 0;
    public bool isGate;

    private void Update()
    {
        // if this is a locked gate, set false when keys are 3 or more
        if (keyNum >= 3 && isGate)
        {
            this.gameObject.SetActive(false);
        }
    }
    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        // if this is a key, set this false and increase keycount
        if (collision.gameObject.CompareTag("PlayerTag") && !isGate)
        {
            keyNum++;
            Debug.Log("keys:" + keyNum);
            this.gameObject.SetActive(false);
            // teleport player back to start
            collision.transform.position = collision.gameObject.GetComponent<PlayerController>().startPosition;
            // set checkpoint back to start position
            collision.gameObject.GetComponent<Collision>().spawnPoint = 
                collision.gameObject.GetComponent<PlayerController>().startPosition;
        }
    }
}