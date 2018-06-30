﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapHeroLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // for highlight
    [SerializeField]
    Color inactiveColor;
    [SerializeField]
    Color activeColor;
    [SerializeField]
    Color highlightedColor;
    [SerializeField]
    Color pressedColor;
    Text labelTxt;
    MapHero mapHero;
    bool isMouseOver = false;

    public MapHero MapHero
    {
        get
        {
            return mapHero;
        }

        set
        {
            mapHero = value;
        }
    }

    public Text LabelTxt
    {
        get
        {
            return labelTxt;
        }

        set
        {
            labelTxt = value;
        }
    }

    void Awake()
    {
        labelTxt = gameObject.GetComponent<Text>();
        MapHero = transform.parent.GetComponent<MapHero>();
        labelTxt.color = inactiveColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("MapHeroLabel OnPointerEnter");
        // highlight this menu
        isMouseOver = true;
        labelTxt.raycastTarget = true;
        SetHighlightedStatus();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("MapHeroLabel OnPointerExit");
        isMouseOver = false;
        labelTxt.raycastTarget = false;
        SetHiddenStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("MapHeroLabel OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("MapHeroLabel OnPointerUp");
        // keep state On
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("MapHeroLabel OnPointerClick");
        // give control on actions to map manager
        MapManager mapManager = transform.parent.parent.GetComponent<MapManager>();
        mapManager.ActOnClick(gameObject, eventData);
    }

    void SetHighlightedStatus()
    {
        // Debug.Log("SetHighlightedStatus " + btn.name + " button");
        labelTxt.color = highlightedColor;
    }

    void SetPressedStatus()
    {
        // Debug.Log("SetPressedStatus " + btn.name + " button");
        labelTxt.color = pressedColor;
    }

    public void SetHiddenStatus()
    {
        // Debug.Log("SetHiddenStatus " + btn.name + " button");
        // disable labels clickability, so it does not pop up when you mouse over map on top of it
        // do this only if mouse is not over this lable
        if (!isMouseOver)
        {
            labelTxt.color = inactiveColor;
            labelTxt.raycastTarget = false;
        }
    }

}
