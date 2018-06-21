using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBlocker : MonoBehaviour {

    public void SetActive(bool doActivate)
    {
        // get mouse cursor controller
        CursorController cursorController = transform.root.Find("CursorController").GetComponent<CursorController>();
        if (doActivate)
        {
            // change mouse cursor
            cursorController.SetBlockInputCursor();
        }
        else
        {
            // change mouse cursor
            cursorController.SetNormalCursor();
        }
        // Activate or deactivate input blocker
        gameObject.SetActive(doActivate);
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
