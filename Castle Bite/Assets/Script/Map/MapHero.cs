using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapHero : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Transform linkedPartyTr;
    public Transform linkedCityOnMapTr;
    enum State { NotSelected, Selected };
    State state = State.NotSelected;
    // for animation
    bool isOn;
    [SerializeField]
    float animationDuration = 1f;
    Text markerTxt;
    // for highlight
    Button btn;
    Color tmpColor;
    Text heroLabel;
    // for path finding
    Vector2 heroTilePosition;

    void Awake()
    {
        isOn = true;
        markerTxt = gameObject.GetComponent<Text>();
        btn = gameObject.GetComponent<Button>();
        heroLabel = transform.Find("HeroLabel").GetComponent<Text>();
        // set party leader lable if hero is already linked
        if (linkedPartyTr)
        {
            PartyPanel partyPanel = linkedPartyTr.GetComponentInChildren<PartyPanel>();
            UpdateHeroLable(partyPanel.GetPartyLeader());
        }
    }

    public void UpdateHeroLable(PartyUnit leaderUnit)
    {
        // GameObject newPartyHeroLable = newPartyOnMapUI.transform.Find("HeroLabel").gameObject;
        heroLabel = transform.Find("HeroLabel").GetComponent<Text>();
        heroLabel.GetComponent<Text>().text = "[" + leaderUnit.GetGivenName() + "]\r\n <size=12>" + leaderUnit.GetUnitName() + "</size> ";
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("MapHero OnPointerEnter");
        // dimm all other menus
        // DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("MapHero OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("MapHero OnPointerUp");
        // keep state On
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("MapHero OnPointerExit");
        // return to previous toggle state
        SetNormalStatus();
        HideLable();
    }

    void HideLable()
    {
        heroLabel.GetComponent<MapHeroLabel>().SetHiddenStatus();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("MapHero OnPointerClick");
        // verify if we are inside the city
        MapCity city = transform.parent.GetComponent<MapCity>();
        if (city)
        {
            // we clicked on a hero's lable, but because hero is inside the city
            // we transfer control to the city's script
            city.OnPointerClick(eventData);
        }
        else
        {
            ActOnClick();
        }
    }

    public void SetSelectedState(bool doActivate)
    {
        // select this hero
        if (doActivate)
        {
            // deselect previously selected hero if it was present
            MapHero previouslySelectedHero = transform.parent.GetComponent<MapManager>().GetSelectedHero();
            if (previouslySelectedHero)
            {
                previouslySelectedHero.SetSelectedState(false);
            }
            // higlight it with red blinking
            state = State.Selected;
            // start blinking (selection) animation
            InvokeRepeating("Blink", 0, animationDuration);
            // inform MapManager about selected hero
            transform.parent.GetComponent<MapManager>().SetSelectedHero(GetComponent<MapHero>());
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

    void ToggleSelectedState()
    {
        switch (state)
        {
            case State.NotSelected:
                // select it
                SetSelectedState(true);
                break;
            case State.Selected:
                // enter edit hero mode
                SetSelectedState(false);
                EnterEditMode();
                break;
            default:
                Debug.LogError("Unknown state");
                break;
        }
    }

    public void ActOnClick()
    {
        // act based on map manager state
        switch (transform.parent.GetComponent<MapManager>().GetMode())
        {
            case MapManager.Mode.Browse:
                ToggleSelectedState();
                break;
            case MapManager.Mode.HighlightMovePath:
                // act based on the fact of it is the same hero or not
                // check if we click on ourselves in highlight move path mode
                if (State.Selected == state)
                {
                    // clicked on myself
                    // do nothing
                } else
                {
                    // this hero is clicked, while other hero is selected
                    // find out if both heros are from the same faction or not
                    HeroParty selectedParty = transform.parent.GetComponent<MapManager>().GetSelectedHero().linkedPartyTr.GetComponent<HeroParty>();
                    HeroParty thisParty = linkedPartyTr.GetComponent<HeroParty>();
                    if (selectedParty.GetFaction() == thisParty.GetFaction())
                    {
                        // highlight this new hero instead
                        SetSelectedState(true);
                    } else
                    {
                        // other faction hero is clicked
                        // verify faction relationships
                        Relationships.State relationships = Relationships.Instance.GetRelationships(selectedParty.GetFaction(), thisParty.GetFaction());
                        switch (relationships)
                        {
                            case Relationships.State.Allies:
                                // nothing to do here
                                // we cannot select allies
                                // we can only right click to see their party
                                break;
                            case Relationships.State.AtWar:
                                // move, attack will be triggered automatically at the end of the move
                                transform.parent.GetComponent<MapManager>().EnterMoveMode();
                                break;
                            case Relationships.State.Neutral:
                                // move, attack will be triggered automatically at the end of the move
                                transform.parent.GetComponent<MapManager>().EnterMoveMode();
                                break;
                            case Relationships.State.SameFaction:
                                // this should not happen here, because we check this previously
                                Debug.LogError("This should not happen here");
                                break;
                            default:
                                Debug.LogError("Unknown relationships");
                                break;
                        }
                    }
                }
                break;
            default:
                Debug.LogError("unknown MapManager mode");
                break;
        }
    }

    #region Animation
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
    #endregion Animation


    bool CompareColors(Color a, Color b)
    {
        bool result = false;
        if (((int)(a.r * 1000) == (int)(b.r * 1000)) || ((int)(a.g * 1000) == (int)(b.g * 1000)) || ((int)(a.b * 1000) == (int)(b.b * 1000)))
        {
            result = true;
        }
        return result;
    }

    public void SetHighlightedStatus()
    {
        // avoid double job
        if (!CompareColors(btn.colors.highlightedColor, markerTxt.color))
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
            markerTxt.color = tmpColor;
            // make also highlight hero label
            heroLabel.color = tmpColor;
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
        markerTxt.color = tmpColor;
        // Debug.Log("SetPressedStatus " + btn.name + " button");
    }

    public void SetNormalStatus()
    {
        // btn = gameObject.GetComponent<Button>();
        if (btn.interactable)
        {
            tmpColor = btn.colors.normalColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        markerTxt.color = tmpColor;
        // Debug.Log("SetNormalStatus " + btn.name + " button");
    }

}
