using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MapCity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    enum State { NotSelected, Selected };

    public Transform linkedCityTr;
    public Transform linkedPartyTr;
    [SerializeField]
    public float labelDimTimeout;
    City linkedCity;
    MapCityLabel label;
    Text labelTxt;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color highlightedColor;
    [SerializeField]
    Color pressedColor;
    [SerializeField]
    bool isMouseOver;
    // for map logic
    //State state;
    // for animation
    bool isOn;
    // [SerializeField]
    // float animationDuration = 1f;
    Image image;

    public bool IsMouseOver
    {
        get
        {
            return isMouseOver;
        }

        set
        {
            isMouseOver = value;
        }
    }

    void Start()
    {
        // for anmimation
        isOn = true;
        // init linkedCity object
        linkedCity = linkedCityTr.GetComponent<City>();
        // init text object
        labelTxt = GetComponentInChildren<Text>();
        label = transform.Find("CityLabel").GetComponent<MapCityLabel>();
        labelTxt.text = "[" + linkedCity.GetCityName() + "]\n\r <size=12>" + linkedCity.GetCityDescription() + "</size>";
        // hide it
        labelTxt.color = new Color(0, 0, 0, 0);
        // get image
        image = GetComponent<Image>();
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
        // SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerUp");
        // keep state On
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerEnter");
        // highlight this menu
        SetHighlightedStatus();
        isMouseOver = true;
        labelTxt.raycastTarget = true;
        // give control on actions to map manager
        MapManager mapManager = transform.parent.GetComponent<MapManager>();
        mapManager.OnPointerEnterChildObject(gameObject, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("MapCity OnPointerExit");
        isMouseOver = false;
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

    void SetHighlightedStatus()
    {
        // Debug.Log("SetHighlightedStatus " + btn.name + " button");
        // change to highlighted color
        labelTxt.color = highlightedColor;
        // also highlight party in the city if it is present
        // but do it a little dimmed to show that it will not be activated on press
        // but to indicate that you can move mouse left to activate it
        if (linkedPartyTr)
        {
            linkedPartyTr.GetComponent<MapHero>().SetNormalStatus();
            // also show hero label
            linkedPartyTr.Find("HeroLabel").GetComponent<MapHeroLabel>().LabelTxt.raycastTarget = true;
            // stop courutine, if it was running to prevent it to dimm out hero lable
            // this does not work
            // StopCoroutine(DimmHeroLabelWithDelay());
        }
    }

    void SetPressedStatus()
    {
        // Debug.Log("SetPressedStatus " + btn.name + " button");
        labelTxt.color = pressedColor;
    }

    void SetNormalStatus()
    {
        // Debug.Log("SetNormalStatus " + btn.name + " button");
        // Dimm label
        StartCoroutine(DimmLabelWithDelay());
        // verify if there is hero in this city
        if (linkedPartyTr)
        {
            // do the same for hero's object
            linkedPartyTr.GetComponent<MapHero>().SetNormalStatus();
        }
    }

    IEnumerator DimmLabelWithDelay()
    {
        // Debug.LogWarning("Before dimm");
        yield return new WaitForSeconds(labelDimTimeout);
        // verify if mouse is not entered again after we started to wait
        if (!isMouseOver)
        {
            label.SetHiddenStatus();
            // Debug.LogWarning("After dimm");
        }
    }

    #region OnClick

    public void EnterCityEditMode()
    {
        // Return to normal status
        SetNormalStatus();
        // go to city edit mode
        // get variables
        GameObject mapScreen = transform.root.Find("MapScreen").gameObject;
        GameObject cityMenu = linkedCity.gameObject;
        // Deactivate map and activate city
        mapScreen.SetActive(false);
        cityMenu.SetActive(true);
    }

    public void SetSelectedState(bool doActivate)
    {
        // select this city
        if (doActivate)
        {
            // higlight it with red blinking
            //state = State.Selected;
            image.color = highlightedColor;
            //// start blinking (selection) animation
            //InvokeRepeating("Blink", 0, animationDuration);
        }
        else
        {
            // exit selected mode
            //state = State.NotSelected;
            image.color = normalColor;
            //// stop blinking
            //CancelInvoke();
            //// show marker, it is needed because sometimes it may cancel invoke, when we are blinked off and invisible
            //Color tmpClr = new Color(labelTxt.color.r, labelTxt.color.g, labelTxt.color.b, 1);
            //labelTxt.color = tmpClr;
        }
    }

    #endregion OnClick

    #region Animation
    void Blink()
    {
        if (isOn)
        {
            // fade away until is off, reduce alpha to 0
            //Color tmpClr = new Color(labelTxt.color.r, labelTxt.color.g, labelTxt.color.b, 0);
            //labelTxt.color = tmpClr;
            //Debug.Log("disable");
            isOn = false;
        }
        else
        {
            // appear until is on, increase alpha to 1 
            //Color tmpClr = new Color(labelTxt.color.r, labelTxt.color.g, labelTxt.color.b, 1);
            //labelTxt.color = tmpClr;
            //Debug.Log("enable");
            isOn = true;
        }
        // yield return new WaitForSeconds(0.2f);
    }
    #endregion Animation

}