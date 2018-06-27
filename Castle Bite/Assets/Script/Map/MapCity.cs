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
    enum State { NotSelected, Selected };

    public Transform linkedCityTr;
    public Transform linkedPartyTr;
    public float heroLableSubmenuDimTimeout;
    City linkedCity;
    Text cityDescrTxt;
    Button btn;
    Color tmpColor;
    bool isMouseOver;
    // for map logic
    State state = State.NotSelected;
    // for animation
    bool isOn;
    [SerializeField]
    float animationDuration = 1f;

    void Start()
    {
        // for anmimation
        isOn = true;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerUp");
        // keep state On
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerEnter");
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
        isMouseOver = true;
        // give control on actions to map manager
        MapManager mapManager = transform.parent.GetComponent<MapManager>();
        mapManager.OnPointerEnterChildObject(gameObject, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerExit");
        isMouseOver = false;
        // return to previous toggle state
        SetNormalStatus();
        // give control on actions to map manager
        MapManager mapManager = transform.parent.GetComponent<MapManager>();
        mapManager.OnPointerExitChildObject(gameObject, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerClick");
        // change city pressed status to city highlighted color
        // so it is not in pressed status any more
        SetHighlightedStatus();
        // give control on actions to map manager
        MapManager mapManager = transform.parent.GetComponent<MapManager>();
        mapManager.ActOnClick(gameObject, eventData);
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
                // stop courutine, if it was running to prevent it to dimm out hero lable
                // this does not work
                // StopCoroutine(DimmHeroLabelWithDelay());
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
            StartCoroutine(DimmHeroLabelWithDelay());
        }
        // Debug.Log("SetNormalStatus " + btn.name + " button");
    }

    IEnumerator DimmHeroLabelWithDelay()
    {
        // Debug.LogWarning("Before dimm");
        yield return new WaitForSeconds(heroLableSubmenuDimTimeout);
        // verify if mouse is not entered again after we started to wait
        if (!isMouseOver)
        {
            MapHeroLabel mapHeroLabel = linkedPartyTr.Find("HeroLabel").GetComponent<MapHeroLabel>();
            mapHeroLabel.SetHiddenStatus();
            // Debug.LogWarning("After dimm");
        }
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

    public void EnterCityEditMode()
    {
        // go to city edit mode
        // get variables
        GameObject mapScreen = btn.transform.root.Find("MapScreen").gameObject;
        GameObject cityMenu = linkedCity.gameObject;
        // Deactivate map and activate city
        mapScreen.SetActive(false);
        cityMenu.SetActive(true);
    }

    //void ActOnClick()
    //{
    //    // act based on the MapManager state
    //    MapManager.Mode mMode = transform.parent.GetComponent<MapManager>().GetMode();
    //    switch (mMode)
    //    {
    //        case MapManager.Mode.Browse:
    //            EnterCityEditMode();
    //            break;
    //        case MapManager.Mode.HighlightMovePath:
    //            // Move hero to the city
    //            transform.parent.GetComponent<MapManager>().EnterMoveMode();
    //            break;
    //        default:
    //            Debug.LogError("unknown MapManager mode " + mMode);
    //            break;
    //    }
    //}

    public void SetSelectedState(bool doActivate)
    {
        // select this hero
        if (doActivate)
        {
            // higlight it with red blinking
            state = State.Selected;
            //// start blinking (selection) animation
            //InvokeRepeating("Blink", 0, animationDuration);
        }
        else
        {
            // exit selected mode
            state = State.NotSelected;
            //// stop blinking
            //CancelInvoke();
            //// show marker, it is needed because sometimes it may cancel invoke, when we are blinked off and invisible
            //Color tmpClr = new Color(markerTxt.color.r, markerTxt.color.g, markerTxt.color.b, 1);
            //markerTxt.color = tmpClr;
        }
    }

    #endregion OnClick

    #region Animation
    void Blink()
    {
        if (isOn)
        {
            // fade away until is off, reduce alpha to 0
            //Color tmpClr = new Color(markerTxt.color.r, markerTxt.color.g, markerTxt.color.b, 0);
            //markerTxt.color = tmpClr;
            //Debug.Log("disable");
            isOn = false;
        }
        else
        {
            // appear until is on, increase alpha to 1 
            //Color tmpClr = new Color(markerTxt.color.r, markerTxt.color.g, markerTxt.color.b, 1);
            //markerTxt.color = tmpClr;
            //Debug.Log("enable");
            isOn = true;
        }
        // yield return new WaitForSeconds(0.2f);
    }
    #endregion Animation

}