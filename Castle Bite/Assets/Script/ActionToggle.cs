using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// rename to TargetedToggle
public class ActionToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum ActToggleType { Heal, Resurect, Dismiss, HeroEquipment };
    [SerializeField]
    ActToggleType toggleType;


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
    //  change cursor back to normal

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}


    Text tglName;
    Toggle tgl;
    Color tmpColor;

    void Start()
    {
        // init lable object
        tglName = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        tgl = gameObject.GetComponent<Toggle>();
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
        // CityControlPanel ctrlPnl = transform.parent.GetComponent<CityControlPanel>();
        CityControlPanel ctrlPnl = tgl.group.GetComponent<CityControlPanel>();
        ctrlPnl.DimmAllOtherMenusExceptToggled(tgl);
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        // Simulate on/off togle
        if (tgl.isOn)
        {
            SetOffStatus();
            ChangeCityActiveState(false);
        }
        else
        {
            SetOnStatus();
            ChangeCityActiveState(true);
            CityControlPanel ctrlPnl = tgl.group.GetComponent<CityControlPanel>();
            ctrlPnl.DeselectAllOtherTogglesInGroup(tgl);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
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
        if (!CompareColors(tgl.colors.highlightedColor, tglName.color))
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
            tglName.color = tmpColor;
            // Debug.Log("SetHighlightedStatus " + tgl.name + " button");
        }
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
        tglName.color = tmpColor;
        // Debug.Log("SetPreHighlightStatus");
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
        tglName.color = tmpColor;
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
        tglName.color = tmpColor;
        // Debug.Log("SetOnStatus " + tgl.name + " button");
    }

    void ChangeCityActiveState(bool doActivate)
    {
        // change cursor to required state based on toggle type
        CityViewActiveState cityViewActiveState = CityViewActiveState.Normal;
        if (toggleType == ActToggleType.Dismiss)
        {
            cityViewActiveState = CityViewActiveState.ActiveDismiss;
        }
        else if (toggleType == ActToggleType.Heal)
        {
            cityViewActiveState = CityViewActiveState.ActiveHeal;

        }
        else if (toggleType == ActToggleType.Resurect)
        {
            cityViewActiveState = CityViewActiveState.ActiveResurect;
        }
        else if (toggleType == ActToggleType.HeroEquipment)
        {
            cityViewActiveState = CityViewActiveState.ActiveHeroEquipment;
        }
        //  get city, structure: [City]-CtrlPnlCity-(this)Toggle, and activate state
        transform.parent.parent.GetComponent<City>().SetActiveState(cityViewActiveState, doActivate);
    }
}
