using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBlocker : MonoBehaviour {

    private static InputBlocker Instance { get; set; }

    void Awake()
    {
        // initialize instance
        Instance = this;
        // disable it
        gameObject.SetActive(false);
    }

    public static void SetActive(bool doActivate)
    {
        // get mouse cursor controller
        // CursorController cursorController = CursorController.Instance;
        if (doActivate)
        {
            // change mouse cursor
            CursorController.Instance.SetBlockInputCursor();
        }
        else
        {
            // change mouse cursor
            CursorController.Instance.SetNormalCursor();
        }
        // Activate or deactivate input blocker
        Instance.gameObject.SetActive(doActivate);
    }

}
