﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapObjectLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    Text labelTxt;
    [SerializeField]
    MapObject mapObject;
    [SerializeField]
    bool isMouseOver = false;
    [SerializeField]
    bool interactable = true;
    [SerializeField]
    float offsetX;
    [SerializeField]
    float offsetY;

    void Awake()
    {
        labelTxt = GetComponent<Text>();
        //Debug.Log("Label text: " + labelTxt.text);
        // mapObject = transform.parent.GetComponent<MapObject>();
        labelTxt.color = mapObject.HiddenLabelColor;
    }

    //void Update()
    //{
    //    if (labelTxt.color != mapObject.HiddenLabelColor){
    //        SetLabelByMapObjectPosition();
    //    }
    //}

    public void SetLabelByMapObjectPosition()
    {
        // place it at the map object position
        // Offset position above object bbox (in world space)
        float offsetPosX = mapObject.transform.position.x + offsetX;
        float offsetPosY = mapObject.transform.position.y + offsetY;
        // Final position of marker above GO in world space
        Vector3 offsetPos = new Vector3(offsetPosX, offsetPosY, mapObject.transform.position.z);
        // Debug.Log("Set label position");
        // transform.position = Camera.main.WorldToScreenPoint(offsetPos);
        transform.position = offsetPos;
    }

    void OnEnable()
    {
        // place it at the map object position
        SetLabelByMapObjectPosition();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable)
        {
            // Debug.Log("MapObjectLabel OnPointerEnter");
            // highlight this menu
            isMouseOver = true;
            labelTxt.color = mapObject.HighlightedLabelColor;
            // give control on actions to map manager
            // MapManager mapManager = transform.parent.parent.GetComponent<MapManager>();
            MapManager.Instance.OnPointerEnterChildObject(gameObject, eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable)
        {
            // Debug.Log("MapObjectLabel OnPointerExit");
            isMouseOver = false;
            // SetHiddenStatus();
            // Verify if mouse is not over MapObject, because in this case we do not need to hide label
            // otherwise it will first trigger enter on map object and then exit on map object label, which will cause troubles
            if (!mapObject.IsMouseOver)
            {
                if (mapObject.LabelAlwaysOn)
                {
                    // HideLabel();
                    SetAlwaysOnLabelColor();
                }
                else
                {
                    // SetAlwaysOnLabelColor();
                    HideLabel();
                }
                // give control on actions to map manager
                // MapManager mapManager = transform.parent.parent.GetComponent<MapManager>();
                MapManager.Instance.OnPointerExitChildObject(gameObject, eventData);
            }
            else
            {
                // trigger eneter on the parent object, like we entered it again
                // because it will not trigger, because it does not know that mouse left it, because mosue was over child object
                // new note it will trigger on pointer enter, because we splited up label from map object
                // mapObject.OnPointerEnter(eventData);
            }
        }
    }

    public void SetAlwaysOnLabelColor()
    {
        if (labelTxt == null)
            labelTxt = GetComponent<Text>();
        if (mapObject == null)
            mapObject = transform.parent.GetComponent<MapObject>();
        labelTxt.color = mapObject.AlwaysOnLabelColor;
    }

    public void HideLabel()
    {
        if (labelTxt == null)
            labelTxt = GetComponent<Text>();
        if (mapObject == null)
            mapObject = transform.parent.GetComponent<MapObject>();
        labelTxt.color = mapObject.HiddenLabelColor;
        labelTxt.raycastTarget = false;
    }

    //public void SetHiddenStatus()
    //{
    //    // Debug.Log("SetHiddenStatus " + btn.name + " button");
    //    // disable labels clickability, so it does not pop up when you mouse over map on top of it
    //    // do this only if mouse is not over this lable and not over parent city marker
    //    if (!isMouseOver && !mapObject.IsMouseOver)
    //    {
    //        labelTxt.color = mapObject.HiddenLabelColor;
    //        labelTxt.raycastTarget = false;
    //    }
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        if (interactable)
        {
            // Debug.Log("MapObjectLabel OnPointerDown");
            labelTxt.color = mapObject.PressedLabelColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (interactable)
        {
            // Debug.Log("MapObjectLabel OnPointerUp");
            // keep state On
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (interactable)
        {
            // Debug.Log("MapObjectLabel OnPointerClick");
            labelTxt.color = mapObject.HighlightedLabelColor;
            // give control on actions to map manager
            // MapManager mapManager = transform.parent.parent.GetComponent<MapManager>();
            MapManager.Instance.ActOnClick(gameObject, eventData);
        }
    }

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

    public Text LabelTxt
    {
        get
        {
            // return labelTxt;
            return GetComponent<Text>();
        }

        set
        {
            labelTxt = value;
        }
    }

    public bool Interactable
    {
        get
        {
            return interactable;
        }

        set
        {
            interactable = value;
        }
    }

    public MapObject MapObject
    {
        get
        {
            return mapObject;
        }

        set
        {
            mapObject = value;
        }
    }
}
