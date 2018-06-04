using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class MapCity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Transform linkedCityTr;
    public Transform linkedPartyTr;
    City linkedCity;
    Text cityDescrTxt;
    Button btn;
    Color tmpColor;

    void Start()
    {
        // init linkedCity object
        linkedCity = linkedCityTr.GetComponent<City>();
        // init text object
        cityDescrTxt = GetComponentInChildren<Text>();
        cityDescrTxt.text = "[" + linkedCity.GetCityName() + "]\n\r <size=12>" + linkedCity.GetCityDescription() + "</size>";
        // hide it
        cityDescrTxt.color = new Color(0, 0, 0, 0);
        btn = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        // enable mouse on its move, if it was disabled before by keyboard activity
        if (((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0)) & (!Cursor.visible))
        {
            Cursor.visible = true;
            // Highlight button, if needed by triggering on point enter
            OnPointerEnter(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("MapCity OnPointerEnter");
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("MapCity OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("MapCity OnPointerUp");
        // keep state On
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("MapCity OnPointerExit");
        // return to previous toggle state
        SetNormalStatus();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("MapCity OnPointerClick");
        ActOnClick();
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
        if (!CompareColors(btn.colors.highlightedColor, cityDescrTxt.color))
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
            cityDescrTxt.color = tmpColor;
            // also highlight party in the city if it is present
            // but do it a little dimmed to show that it will not be activated on press
            // but to indicate that you can move mouse left to activate it
            if (linkedPartyTr)
            {
                linkedPartyTr.GetComponent<MapHero>().SetNormalStatus();
                // also show hero label
                linkedPartyTr.Find("HeroLabel").GetComponent<MapHeroLabel>().SetVisibleAndClickableStatus();
            }
            // Debug.Log("SetHighlightedStatus " + btn.name + " button");
        }
    }

    void SetPressedStatus()
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
        cityDescrTxt.color = tmpColor;
        // Debug.Log("SetPressedStatus " + btn.name + " button");
    }

    void SetNormalStatus()
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
        cityDescrTxt.color = tmpColor;
        // also hide map hero lable
        if (linkedPartyTr)
        {
            // linkedPartyTr.GetComponent<MapHero>().SetNormalStatus();
            // also show hero label
            MapHeroLabel mapHeroLabel = linkedPartyTr.Find("HeroLabel").GetComponent<MapHeroLabel>();
            mapHeroLabel.SetNormalStatus();
        }
        // Debug.Log("SetNormalStatus " + btn.name + " button");
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

    #region OnClick

    void ActOnClick()
    {
        // go back to main menu
        // change this to map in future
        GameObject mapScreen = btn.transform.root.Find("MapScreen").gameObject;
        GameObject cityMenu = linkedCity.gameObject;
        mapScreen.SetActive(false);
        cityMenu.SetActive(true);
    }

    #endregion OnClick
}