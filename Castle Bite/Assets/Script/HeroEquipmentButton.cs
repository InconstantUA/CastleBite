using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class HeroEquipmentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Text txt;
    Button btn;
    Color tmpColor;
    Color preHighlightColor;

    void Start()
    {
        // init text object
        txt = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        btn = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        // enable mouse on its move, if it was disabled before by keyboard activity
        if (((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0)) & (!Cursor.visible))
        {
            Cursor.visible = true;
            // highlight button if it was highlighted before
            //if (CompareColors(btn.colors.highlightedColor, txt.color))
            //{
            //    DimmAllOtherMenus();
            //    SetHighlightedStatus();
            //}
            // Highlight button, if needed by triggering on point enter
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
        if (CompareColors(btn.colors.pressedColor, preHighlightColor))
        {
            // was on -> transition to off
            SetOffStatus();
        }
        else
        {
            // was off -> transition to on
            SetOnStatus();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        // keep state On
        ActOnClick();
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
        if (!CompareColors(btn.colors.highlightedColor, txt.color))
        {
            // change to highlighted color
            if (btn.interactable)
            {
                tmpColor = btn.colors.highlightedColor;
            }
            else
            {
                tmpColor = btn.colors.disabledColor;
            }
            tmpColor.a = 1;
            txt.color = tmpColor;
            Debug.Log("SetHighlightedStatus " + btn.name + " button");
        }
    }

    void SetOnStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.pressedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        preHighlightColor = tmpColor;
    Debug.Log("SetOnStatus " + btn.name + " button");
    }

    void SetOffStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.normalColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        preHighlightColor = tmpColor;
        Debug.Log("SetOnStatus " + btn.name + " button");
    }

    void SetPreHighlightStatus()
    {
        // return to previous color if was not On
        if (CompareColors(btn.colors.pressedColor, preHighlightColor))
        {
            txt.color = btn.colors.pressedColor;
        } else
        {
            txt.color = btn.colors.normalColor;
        }
    }

    void DimmAllOtherMenus()
    {
        // to make it easier dimm just all menus
        GameObject[] highlightableText = GameObject.FindGameObjectsWithTag("HighlightableCityView");
        foreach (GameObject text in highlightableText)
        {
            Text tmpTxt = text.GetComponentInChildren<Text>();
            // Button tmpBtn = text.GetComponentInChildren<Button>();
            tmpTxt.color = btn.colors.normalColor;
            // Debug.Log("dimm " + otherButton.name + " button");
        }
        Debug.Log("DimmAllOtherMenus");
    }

    GameObject GetChildGameObjByTag(string tag)
    {
        GameObject result = null;
        foreach (Transform child in transform.parent)
        {
            if (child.gameObject.tag == tag)
            {
                result = child.gameObject;
            }
        }
        return result;
    }

    #region OnClick

    void ActOnClick()
    {
        // Disable Hero equipment menu if it was enabled and enable it otherwise
        // Also disable / enable hero party and city garnizon
        // Structure:   [city]->(this)Button
        GameObject heroParty = GetChildGameObjByTag("HeroParty");
        GameObject heroEquipmentMenu = heroParty.transform.Find("HeroEquipment").gameObject;
        GameObject heroUnitsPanel = heroParty.transform.Find("PartyPanel").gameObject;
        GameObject cityGarnizon = transform.parent.Find("CityGarnizon").gameObject;
        if (heroEquipmentMenu.activeSelf)
        {
            heroEquipmentMenu.SetActive(false);
            heroUnitsPanel.SetActive(true);
            cityGarnizon.SetActive(true);
        }
        else
        {
            heroEquipmentMenu.SetActive(true);
            heroUnitsPanel.SetActive(false);
            cityGarnizon.SetActive(false);
        }
    }

    #endregion OnClick
}