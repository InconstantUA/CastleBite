using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// public class MapHero : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
public class MapHero : MonoBehaviour
{
    enum State { NotSelected, Selected };

    [SerializeField]
    private Transform linkedPartyTr;
    public Transform linkedCityOnMapTr;
    [SerializeField]
    public float labelDimTimeout;
    State state;
    // for animation
    bool isOn;
    [SerializeField]
    float animationDuration = 1f;
    Text markerTxt;
    //// for highlight
    //[SerializeField]
    //Color normalColor;
    //[SerializeField]
    //Color highlightedColor;
    //[SerializeField]
    //Color pressedColor;
    [SerializeField]
    MapObjectLabel label;
    // for path finding
    //Vector2 heroTilePosition;

    public Transform LinkedPartyTr
    {
        get
        {
            return linkedPartyTr;
        }

        set
        {
            linkedPartyTr = value;
            UpdateLabelText();
        }
    }

    void Awake()
    {
        isOn = true;
        markerTxt = gameObject.GetComponent<Text>();
        // update linked party transform if it is already predefined
        // - this is predefined of object alreayd created on the map
        // - this is set autmoatically when new party leader is highered
        if (linkedPartyTr)
        {
            UpdateLabelText();
        }
    }

    void UpdateLabelText()
    {
        // set label
        label = GetComponentInChildren<MapObjectLabel>();
        // set label text
        PartyUnit leaderUnit = linkedPartyTr.GetComponentInChildren<PartyPanel>().GetPartyLeader();
        string givenName = leaderUnit.GetGivenName();
        string unitName = leaderUnit.GetUnitName();
        label.LabelTxt.text = "[" + givenName + "]\r\n <size=12>" + unitName + "</size> ";
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    // Debug.Log("MapHero OnPointerDown");
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // Debug.LogWarning("OnPointerDown");
    //        // on left mouse click
    //        SetPressedStatus();
    //    }
    //    else if (Input.GetMouseButtonDown(1))
    //    {
    //        // on right mouse click
    //        // show unit info
    //        transform.root.Find("MiscUI/PartiesInfoPanel").GetComponent<PartiesInfoPanel>().ActivateAdvance(GetComponent<MapHero>());
    //    }
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    // Debug.Log("MapHero OnPointerUp");
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        // on left mouse click
    //        // keep state On
    //    }
    //    else if (Input.GetMouseButtonUp(1))
    //    {
    //        // on right mouse click
    //        // deactivate unit info
    //        transform.root.Find("MiscUI/PartiesInfoPanel").gameObject.SetActive(false);
    //    }
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    // Debug.Log("MapHero OnPointerEnter");
    //    // dimm all other menus
    //    // DimmAllOtherMenus();
    //    // highlight this menu
    //    SetHighlightedStatus();
    //    // give control on actions to map manager
    //    MapManager mapManager = transform.parent.GetComponent<MapManager>();
    //    mapManager.OnPointerEnterChildObject(gameObject, eventData);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    // Debug.Log("MapHero OnPointerExit");
    //    // return to previous toggle state
    //    SetNormalStatus();
    //    HideLable();
    //    // give control on actions to map manager
    //    MapManager mapManager = transform.parent.GetComponent<MapManager>();
    //    mapManager.OnPointerExitChildObject(gameObject, eventData);
    //}

    //void HideLable()
    //{
    //    heroLabel.GetComponent<MapHeroLabel>().SetHiddenStatus();
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    // Debug.Log("MapHero OnPointerClick");
    //    // verify if we are inside the city
    //    MapCity city = transform.parent.GetComponent<MapCity>();
    //    if (city)
    //    {
    //        // we clicked on a hero's lable, but because hero is inside the city
    //        // we transfer control to the city's script
    //        city.OnPointerClick(eventData);
    //    }
    //    else
    //    {
    //        // give control on actions to map manager
    //        MapManager mapManager = transform.parent.GetComponent<MapManager>();
    //        mapManager.ActOnClick(gameObject, eventData);
    //    }
    //}

    public void SetSelectedState(bool doActivate)
    {
        // select this hero
        if (doActivate)
        {
            // higlight it with red blinking
            state = State.Selected;
            // start blinking (selection) animation
            InvokeRepeating("Blink", 0, animationDuration);
        }
        else
        {
            // exit selected mode
            state = State.NotSelected;
            // stop blinking
            CancelInvoke();
            // show marker, it is needed because sometimes it may cancel invoke, when we are blinked off and invisible
            Color tmpClr = new Color(markerTxt.color.r, markerTxt.color.g, markerTxt.color.b, 1);
            markerTxt.color = tmpClr;
        }
    }

    void EnterEditMode()
    {
        Debug.Log("Enter party edit mode");
    }

    //void ToggleSelectedState()
    //{
    //    switch (state)
    //    {
    //        case State.NotSelected:
    //            // select it
    //            SetSelectedState(true);
    //            break;
    //        case State.Selected:
    //            // enter edit hero mode
    //            SetSelectedState(false);
    //            EnterEditMode();
    //            break;
    //        default:
    //            Debug.LogError("Unknown state");
    //            break;
    //    }
    //}

    //public void ActOnClick()
    //{
    //    // act based on map manager state
    //    switch (transform.parent.GetComponent<MapManager>().GetMode())
    //    {
    //        case MapManager.Mode.Browse:
    //            //ToggleSelectedState();
    //            break;
    //        case MapManager.Mode.Animation:
    //            // act based on the fact of it is the same hero or not
    //            // check if we click on ourselves in highlight move path mode
    //            if (State.Selected == state)
    //            {
    //                // clicked on myself
    //                // do nothing
    //            } else
    //            {
    //                // this hero is clicked, while other hero is selected
    //                // find out if both heros are from the same faction or not
    //                HeroParty selectedParty = transform.parent.GetComponent<MapManager>().GetSelectedHero().linkedPartyTr.GetComponent<HeroParty>();
    //                HeroParty thisParty = linkedPartyTr.GetComponent<HeroParty>();
    //                if (selectedParty.GetFaction() == thisParty.GetFaction())
    //                {
    //                    // highlight this new hero instead
    //                    SetSelectedState(true);
    //                } else
    //                {
    //                    // other faction hero is clicked
    //                    // verify faction relationships
    //                    Relationships.State relationships = Relationships.Instance.GetRelationships(selectedParty.GetFaction(), thisParty.GetFaction());
    //                    switch (relationships)
    //                    {
    //                        case Relationships.State.Allies:
    //                            // nothing to do here
    //                            // we cannot select allies
    //                            // we can only right click to see their party
    //                            break;
    //                        case Relationships.State.AtWar:
    //                            // move, attack will be triggered automatically at the end of the move
    //                            //transform.parent.GetComponent<MapManager>().EnterMoveMode();
    //                            break;
    //                        case Relationships.State.Neutral:
    //                            // move, attack will be triggered automatically at the end of the move
    //                            //transform.parent.GetComponent<MapManager>().EnterMoveMode();
    //                            break;
    //                        case Relationships.State.SameFaction:
    //                            // this should not happen here, because we check this previously
    //                            Debug.LogError("This should not happen here");
    //                            break;
    //                        default:
    //                            Debug.LogError("Unknown relationships");
    //                            break;
    //                    }
    //                }
    //            }
    //            break;
    //        default:
    //            Debug.LogError("unknown MapManager mode");
    //            break;
    //    }
    //}


    public void EnterHeroEditMode()
    {
        Debug.Log("Enter hero edit mode.");
        // Hide label
        label.HideLabel();
        // go to hero edit mode
        // get variables
        GameObject mapScreen = transform.root.Find("MapScreen").gameObject;
        // .. set hero edit menu game object
        // Deactivate map and activate hero edit menu
        //mapScreen.SetActive(false);
        // .. activate hero edit menu
    }

    void Blink()
    {
        if (isOn)
        {
            // fade away until is off, reduce alpha to 0
            Color tmpClr = new Color(markerTxt.color.r, markerTxt.color.g, markerTxt.color.b, 0);
            markerTxt.color = tmpClr;
            //Debug.Log("disable");
            isOn = false;
        }
        else
        {
            // appear until is on, increase alpha to 1 
            Color tmpClr = new Color(markerTxt.color.r, markerTxt.color.g, markerTxt.color.b, 1);
            markerTxt.color = tmpClr;
            //Debug.Log("enable");
            isOn = true;
        }
        // yield return new WaitForSeconds(0.2f);
    }

    //public void SetHighlightedStatus()
    //{
    //    // Debug.Log("SetHighlightedStatus " + btn.name + " button");
    //    markerTxt.color = highlightedColor;
    //    // make also highlight hero label
    //    heroLabel.color = highlightedColor;
    //}

    //void SetPressedStatus()
    //{
    //    // Debug.Log("SetPressedStatus " + btn.name + " button");
    //    markerTxt.color = pressedColor;
    //}

    //public void SetNormalStatus()
    //{
    //    // Debug.Log("SetNormalStatus " + btn.name + " button");
    //    markerTxt.color = normalColor;
    //}

}
