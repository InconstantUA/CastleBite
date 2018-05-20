using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Toggle in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning toggle colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Toggle))]
public class HeroHireMenuLeaderToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum HeroType { Knight, Ranger, Archmage, Archangel, Thief };
    public HeroType heroType;
    Text heroTypeTxt;
    Toggle tgl;
    Color tmpColor;
    public bool isPLeaderSelected = false;

    void Start()
    {
        // init lable object
        heroTypeTxt = GetComponentsInChildren<Text>()[0];
        // baseColor = txt.color;
        tgl = gameObject.GetComponent<Toggle>();
        // pre-select knight leader
        if (heroType == HeroType.Knight)
        {
            isPLeaderSelected = true;
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
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        // Simulate on/off togle
        // Do not off, if it was on, because it means that no object is selected
        // We should have at least one object selected
        if (isPLeaderSelected)
        {
            // was on -> transition to off
            // SetOffStatus();
        }
        else
        {
            // was off -> transition to on
            SetOnStatus();
            DeselectAllOtherTogglesInGroup();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        // keep state On
        // ActOnClick();
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
        if (!CompareColors(tgl.colors.highlightedColor, heroTypeTxt.color))
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
            heroTypeTxt.color = tmpColor;
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
        heroTypeTxt.color = tmpColor;
        isPLeaderSelected = true;
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
        heroTypeTxt.color = tmpColor;
        isPLeaderSelected = false;
        Debug.Log("SetOnStatus " + tgl.name + " button");
    }

    void SetPreHighlightStatus()
    {
        // return to previous color if was not On
        if (isPLeaderSelected)
        {
            tmpColor = tgl.colors.pressedColor;
        }
        else
        {
            tmpColor = tgl.colors.normalColor;
        }
        tmpColor.a = 1;
        heroTypeTxt.color = tmpColor;
        Debug.Log("SetPreHighlightStatus");
    }

    void DimmAllOtherMenus()
    {
        GameObject[] partyLeaders = GameObject.FindGameObjectsWithTag("HirePartyLeader");
        foreach (GameObject leader in partyLeaders)
        {
            // do not dimm currently selected objects
            Text tmpTxt = leader.GetComponentInChildren<Text>();
            Toggle tmpTgl = leader.GetComponentInChildren<Toggle>();
            HeroHireMenuLeaderToggle tmpHero = tmpTgl.GetComponent<HeroHireMenuLeaderToggle>();
            if ( (!tmpTgl.isOn) && (tmpHero.heroType != heroType) )
            {
                tmpTxt.color = tmpTgl.colors.normalColor;
            }
        }
        Debug.Log("DimmAllOtherMenus");
    }
    
    void DeselectAllOtherTogglesInGroup()
    {
        GameObject[] partyLeaders = GameObject.FindGameObjectsWithTag("HirePartyLeader");
        foreach (GameObject leader in partyLeaders)
        {
            // do not dimm currently selected objects
            Text tmpTxt = leader.GetComponentInChildren<Text>();
            Toggle tmpTgl = leader.GetComponentInChildren<Toggle>();
            HeroHireMenuLeaderToggle tmpHero = tmpTgl.GetComponent<HeroHireMenuLeaderToggle>();
            if ((tmpTgl.isOn) && (tmpHero.heroType != heroType))
            {
                tmpTxt.color = tmpTgl.colors.normalColor;
                tmpHero.isPLeaderSelected = false;
            }
        }
        Debug.Log("DeselectAllOtherTogglesInGroup");
    }
}