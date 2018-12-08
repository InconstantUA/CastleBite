using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBlocker : MonoBehaviour {

    public static InputBlocker Instance { get; private set; }

    void Awake()
    {
        // initialize instance
        Instance = this;
        // disable it
        gameObject.SetActive(false);
    }

    public void SetActive(bool doActivate)
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
        gameObject.SetActive(doActivate);
    }

}
