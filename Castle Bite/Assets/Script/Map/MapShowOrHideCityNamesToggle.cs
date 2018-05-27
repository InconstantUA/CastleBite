using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class MapShowOrHideCityNamesToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Text lbl;
    Text tip;
    Button btn;
    Color tmpColor;
    bool isOn = false;

    void Start()
    {
        // init lable object
        lbl = GetComponentsInChildren<Text>()[0];
        // init tip text object
        tip = GetComponentsInChildren<Text>()[1];
        // hide tip
        tip.color = new Color(0, 0, 0, 0);
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
        // Debug.Log("OnPointerEnter");
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        // Simulate on/off togle
        // if (CompareColors(btn.colors.pressedColor, preHighlightColor))
        if (isOn)
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
        // Debug.Log("OnPointerUp");
        // keep state On
        ActOnClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // return to previous toggle state
        SetPreHighlightStatus();
        SetTipNormalStatus();
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
        if (!CompareColors(btn.colors.highlightedColor, lbl.color))
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
            // Highlight label ([N] button)
            lbl.color = tmpColor;
            // Show tip
            tip.color = tmpColor;
            // Debug.Log("SetHighlightedStatus " + btn.name + " button");
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
        lbl.color = tmpColor;
        isOn = true;
        // Debug.Log("SetOnStatus " + btn.name + " button");
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
        lbl.color = tmpColor;
        isOn = false;
        // Debug.Log("SetOnStatus " + btn.name + " button");
    }

    void SetPreHighlightStatus()
    {
        // return to previous color if was not On
        // if (CompareColors(btn.colors.pressedColor, preHighlightColor))
        if (isOn)
        {
            tmpColor = btn.colors.pressedColor;
        }
        else
        {
            tmpColor = btn.colors.normalColor;
        }
        tmpColor.a = 1;
        lbl.color = tmpColor;
        // Debug.Log("SetPreHighlightStatus");
    }

    void DimmAllOtherMenus()
    {
        // to make it easier dimm just all menus
        GameObject[] highlightableText = GameObject.FindGameObjectsWithTag("HighlightableMapView");
        foreach (GameObject text in highlightableText)
        {
            Text tmpTxt = text.GetComponentInChildren<Text>();
            Button tmpBtn = text.GetComponentInChildren<Button>();
            tmpTxt.color = tmpBtn.colors.normalColor;
            // Debug.Log("dimm " + otherButton.name + " button");
        }
        // Debug.Log("DimmAllOtherMenus");
    }

    void SetTipNormalStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.normalColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 0;
        tip.color = tmpColor;
        // Debug.Log("SetTipNormalStatus " + btn.name + " button");
    }

    #region OnClick

    void ActOnClick()
    {
        // Find all city objects and show their names
        GameObject[] allCities = GameObject.FindGameObjectsWithTag("MapCity");
        foreach (GameObject city in allCities)
        {
            Text tmpTxt = city.GetComponentInChildren<Text>();
            Button tmpBtn = city.GetComponentInChildren<Button>();
            if (isOn)
            {
                tmpColor = tmpBtn.colors.highlightedColor;
                tmpColor.a = 1; // show it
                // Debug.Log("Show all cities names");
            }
            else
            {
                tmpColor = tmpBtn.colors.normalColor;
                tmpColor.a = 0; // hide it
                // Debug.Log("Hide all cities names");
            }
            tmpTxt.color = tmpColor;
            // Debug.Log("dimm " + otherButton.name + " button");
        }
    }

    #endregion OnClick
}