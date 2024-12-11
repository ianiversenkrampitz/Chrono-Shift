using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Iversen-Krampitz, Ian 
 * 12/10/2024
 * Locks cursor to game window.
 */
public class MouseLock : MonoBehaviour
{
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
