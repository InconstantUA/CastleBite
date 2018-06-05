using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapHero : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Transform linkedHeroTr;
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

    void SetSelectedState(bool doActivate)
    {
        if (doActivate)
        {
            // higlight it with red blinking
            state = State.Selected;
            // start blinking (selection) animation
            InvokeRepeating("Blink", 0, animationDuration);
            // inform MapManager about selected hero
            transform.parent.GetComponent<MapManager>().SetSelectedHero(GetComponent<MapHero>());
        }
        else
        {
            // exit highlight mode
            state = State.NotSelected;
            CancelInvoke();
        }
    }

    void EnterEditMode()
    {

    }

    public void ActOnClick()
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
            // appear auntil is on, increase alpha to 1 
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
