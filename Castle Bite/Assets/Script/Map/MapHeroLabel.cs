using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapHeroLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    enum State { NotSelected, Selected };
    State state = State.NotSelected;
    // for highlight
    Button btn;
    Color tmpColor;
    Text heroLabel;
    MapHero mapHero;
    bool isMouseOver = false;

    void Awake()
    {
        btn = gameObject.GetComponent<Button>();
        heroLabel = gameObject.GetComponent<Text>();
        mapHero = transform.parent.GetComponent<MapHero>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("MapHeroLabel OnPointerEnter");
        // dimm all other menus
        // DimmAllOtherMenus();
        // highlight this menu
        isMouseOver = true;
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("MapHeroLabel OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("MapHeroLabel OnPointerUp");
        // keep state On
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("MapHeroLabel OnPointerExit");
        isMouseOver = false;
        // return to previous toggle state
        SetNormalStatus();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("MapHeroLabel OnPointerClick");
        ActOnClick();
    }

    void SetSelectedState(bool doActivate)
    {
        if (doActivate)
        {
            // higlight it with red blinking
            state = State.Selected;
        }
        else
        {
            // exit highlight mode
            state = State.NotSelected;
        }
    }

    void ActOnClick()
    {
        // do actions for map hero object
        mapHero.ActOnClick();
        // do actions for yourself
        switch (state)
        {
            case State.NotSelected:
                // select it
                SetSelectedState(true);
                break;
            case State.Selected:
                // enter edit hero mode
                SetSelectedState(false);
                break;
            default:
                Debug.LogError("Unknown state");
                break;
        }
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
        if (!CompareColors(btn.colors.highlightedColor, heroLabel.color))
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
        heroLabel.color = tmpColor;
        // Debug.Log("SetPressedStatus " + btn.name + " button");
    }

    public void SetNormalStatus()
    {
        // if (State.NotSelected == state)
        if (!isMouseOver)
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
            heroLabel.color = tmpColor;
        }
        // disable labels clickability, so it does not pop up when you mouse over map on top of it
        // do this only if mouse is not over this lable
        //if (!((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0)))
        //{
        if (!isMouseOver)
        {
            heroLabel.raycastTarget = false;
        }
        //}
        // Debug.Log("SetNormalStatus " + btn.name + " button");
    }


    public void SetVisibleAndClickableStatus()
    {
        // if (State.NotSelected == state)
        if (true)
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
            heroLabel.color = tmpColor;
        }
        heroLabel.raycastTarget = true;
        // Debug.Log("SetNormalStatus " + btn.name + " button");
    }

}
