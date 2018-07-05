using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Toggles should be organized under same toggle group as parent object
// Toggles should have child Name Text UI object
[RequireComponent(typeof(Toggle))]
public class ToggleForText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Text unitName;
    Toggle tgl;
    Toggle[] allTogglesInGroup;
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
        // get all toggles in Toggle group
        allTogglesInGroup = transform.parent.GetComponentsInChildren<Toggle>();
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
        // Debug.Log("OnPointerEnter");
        // dimm all other menus
        DimmAllOtherMenusExceptToggled();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        if (Input.GetMouseButtonDown(0))
        {
            // Simulate on/off togle
            // Do not off, if it was on, because it means that no object is selected
            // We should have at least one object selected
            if (!tgl.isOn)
            {
                SetOnStatus();
                DeselectAllOtherTogglesInGroup();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // on right mouse click
            // verify if this is unit hire menu and display unit info
            if (gameObject.GetComponent<UnitHirePanel>())
            {
                // show unit info
                PartyUnit partyUnit = gameObject.GetComponent<UnitHirePanel>().GetUnitToHire();
                // verify if this is partyUnit
                if (partyUnit)
                {
                    transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>().ActivateAdvance(partyUnit);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
            // turn it on if it is not ON already
            //if (!tgl.isOn)
            //{
            //    // was off -> transition to on
            //    SetOnStatus();
            //    DeselectAllOtherTogglesInGroup();
            //}
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate unit info
            transform.root.Find("MiscUI/UnitInfoPanel").gameObject.SetActive(false);
        }
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
            // Debug.Log("SetHighlightedStatus " + tgl.name + " button");
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
        // Debug.Log("SetOnStatus " + tgl.name + " button");
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
        // Debug.Log("SetOnStatus " + tgl.name + " button");
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
        // Debug.Log("SetPreHighlightStatus");
    }

    void DimmAllOtherMenusExceptToggled()
    {
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            // do not dimm currently selected objects
            // first child in toggle is Name Text UI obj
            Text tmpUnitName = tmpTgl.GetComponentInChildren<Text>();
            // make sure that we do not deem ourselves and toggled (selected) unit
            if ( (!tmpTgl.isOn) && (unitName != tmpUnitName) )
            {
                tmpUnitName.color = tmpTgl.colors.normalColor;
            }
        }
        // Debug.Log("DimmAllOtherMenusExceptToggled");
    }
    
    void DeselectAllOtherTogglesInGroup()
    {
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            // do not dimm currently selected objects
            // first child in toggle is Name Text UI obj
            Text tmpUnitName = tmpTgl.GetComponentInChildren<Text>();
            // make sure that we do not deem ourselves
            if ((tmpTgl.isOn) && (unitName != tmpUnitName))
            {
                tmpUnitName.color = tmpTgl.colors.normalColor;
            }
        }
        // Debug.Log("DeselectAllOtherTogglesInGroup");
    }
}