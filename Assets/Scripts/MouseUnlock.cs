using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Iversen-Krampitz, Ian 
 * 12/10/2024
 * Unlocks cursor for menus.
 */
public class MouseUnlock : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
