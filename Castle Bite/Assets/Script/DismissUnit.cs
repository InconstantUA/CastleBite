using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DismissUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    // button works as toggle
    // - on click
    //  change cursor to dismiss state
    //  highligt cells, which can be dismissed
    // - on Escape keyboard presss or on second click on a dismis button
    //  change cursor back to normal state
    // while cursor is in dismiss state
    // if clicked on a common unit
    //  show confirmation popup (yes,no to remove the unit)
    //  remove unit
    // if clicked on a party leader with other units
    //  verify if user really wants to dismiss hero with other party members
    //  remvove whole party
    // if clicked on something else
    // change cursor back to normal

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}


    Text unitName;
    Toggle tgl;
    Color tmpColor;

    void Start()
    {
        // init lable object
        unitName = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        tgl = gameObject.GetComponent<Toggle>();
        // pre-select leader if it is selected in Unity UI
        if (tgl.isOn)
        {
            SetOnStatus();
        }
    }

    void Update()
    {
        // enable mouse on its move, if it was disabled before by keyboard activity
        if (((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0)) & (!Cursor.visible))
        {
            Cursor.visible = true;
            OnPointerEnter(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        // dimm all other menus
        // DimmAllOtherMenusExceptToggled();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        // Simulate on/off togle
        if (tgl.isOn)
        {
            SetOffStatus();
        }
        else
        {
            SetOnStatus();
            // DeselectAllOtherTogglesInGroup();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        // turn it on if it is not ON already
        //if (!tgl.isOn)
        //{
        //    // was off -> transition to on
        //    SetOnStatus();
        //    DeselectAllOtherTogglesInGroup();
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // return to previous toggle state
        SetPreHighlightStatus();
    }

    bool CompareColors(Color a, Color b)
    {
        bool result = false;
        if (((int)(a.r * 1000) == (int)(b.r * 1000)) || ((int)(a.g * 1000) == (int)(b.g * 1000)) || ((int)(a.b * 1000) == (int)(b.b * 1000)))
        {
            result = true;
        }
        return result;
    }

    void SetHighlightedStatus()
    {
        // avoid double job
        if (!CompareColors(tgl.colors.highlightedColor, unitName.color))
        {
            // change to highlighted color
            if (tgl.interactable)
            {
                tmpColor = tgl.colors.highlightedColor;
            }
            else
            {
                tmpColor = tgl.colors.disabledColor;
            }
            tmpColor.a = 1;
            // Highlight label ([N] button)
            unitName.color = tmpColor;
            Debug.Log("SetHighlightedStatus " + tgl.name + " button");
        }
    }

    void SetOnStatus()
    {
        if (tgl.interactable)
        {
            tmpColor = tgl.colors.pressedColor;
        }
        else
        {
            tmpColor = tgl.colors.disabledColor;
        }
        tmpColor.a = 1;
        unitName.color = tmpColor;
        // change cursor to Dismiss
        CursorController.Instance.SetDismissUnitCursor();
        Debug.Log("SetOnStatus " + tgl.name + " button");
    }

    void SetOffStatus()
    {
        if (tgl.interactable)
        {
            tmpColor = tgl.colors.normalColor;
        }
        else
        {
            tmpColor = tgl.colors.disabledColor;
        }
        tmpColor.a = 1;
        unitName.color = tmpColor;
        // change cursor to Normal
        CursorController.Instance.SetNormalCursor();
        Debug.Log("SetOnStatus " + tgl.name + " button");
    }

    void SetPreHighlightStatus()
    {
        // return to previous color if was not On
        if (tgl.isOn)
        {
            tmpColor = tgl.colors.pressedColor;
        }
        else
        {
            tmpColor = tgl.colors.normalColor;
        }
        tmpColor.a = 1;
        unitName.color = tmpColor;
        Debug.Log("SetPreHighlightStatus");
    }

    //void DimmAllOtherMenusExceptToggled()
    //{
    //    foreach (Toggle tmpTgl in allTogglesInGroup)
    //    {
    //        // do not dimm currently selected objects
    //        // first child in toggle is Name Text UI obj
    //        Text tmpUnitName = tmpTgl.GetComponentInChildren<Text>();
    //        // make sure that we do not deem ourselves and toggled (selected) unit
    //        if ((!tmpTgl.isOn) && (unitName != tmpUnitName))
    //        {
    //            tmpUnitName.color = tmpTgl.colors.normalColor;
    //        }
    //    }
    //    Debug.Log("DimmAllOtherMenusExceptToggled");
    //}

    //void DeselectAllOtherTogglesInGroup()
    //{
    //    foreach (Toggle tmpTgl in allTogglesInGroup)
    //    {
    //        // do not dimm currently selected objects
    //        // first child in toggle is Name Text UI obj
    //        Text tmpUnitName = tmpTgl.GetComponentInChildren<Text>();
    //        // make sure that we do not deem ourselves
    //        if ((tmpTgl.isOn) && (unitName != tmpUnitName))
    //        {
    //            tmpUnitName.color = tmpTgl.colors.normalColor;
    //        }
    //    }
    //    Debug.Log("DeselectAllOtherTogglesInGroup");
    //}
}
