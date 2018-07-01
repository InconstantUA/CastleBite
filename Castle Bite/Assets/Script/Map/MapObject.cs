﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MapObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // For UI user interraction
    [SerializeField]
    public float labelDimTimeout;
    MapObjectLabel label;
    Text labelTxt;
    // Colors for Label
    [SerializeField]
    Color hiddenLabelColor;
    [SerializeField]
    Color notHighlightedLabelColor;
    [SerializeField]
    Color highlightedLabelColor;
    [SerializeField]
    Color pressedLabelColor;
    // For dimm
    [SerializeField]
    bool isMouseOver;

    void Start()
    {
        // set label
        label = GetComponentInChildren<MapObjectLabel>();
        // set label text object
        labelTxt = label.GetComponent<Text>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.LogWarning("OnPointerDown");
            // on left mouse click
            labelTxt.color = pressedLabelColor;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // on right mouse click
            // show unit info
            transform.root.Find("MiscUI/PartiesInfoPanel").GetComponent<PartiesInfoPanel>().ActivateAdvance(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate unit info
            transform.root.Find("MiscUI/PartiesInfoPanel").gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
        isMouseOver = false;
        labelTxt.color = notHighlightedLabelColor;
        // Dimm label
        StartCoroutine(DimmLabelWithDelay());
        // give control on actions to map manager
        MapManager mapManager = transform.parent.GetComponent<MapManager>();
        mapManager.OnPointerExitChildObject(gameObject, eventData);
    }

    IEnumerator DimmLabelWithDelay()
    {
        yield return new WaitForSeconds(labelDimTimeout);
        // verify if mouse is not entered again after we started to wait
        if (!isMouseOver && !label.IsMouseOver)
        {
            label.HideLabel();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // change city pressed status to city highlighted color
        // so it is not in pressed status any more
        SetHighlightedStatus();
        // give control on actions to map manager
        MapManager mapManager = transform.parent.GetComponent<MapManager>();
        mapManager.ActOnClick(gameObject, eventData);
    }

    void SetHighlightedStatus()
    {
        // change to highlighted color
        labelTxt.color = highlightedLabelColor;
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

    public Color HiddenLabelColor
    {
        get
        {
            return hiddenLabelColor;
        }

        set
        {
            hiddenLabelColor = value;
        }
    }

    public Color NotHighlightedLabelColor
    {
        get
        {
            return notHighlightedLabelColor;
        }

        set
        {
            notHighlightedLabelColor = value;
        }
    }

    public Color HighlightedLabelColor
    {
        get
        {
            return highlightedLabelColor;
        }

        set
        {
            highlightedLabelColor = value;
        }
    }

    public Color PressedLabelColor
    {
        get
        {
            return pressedLabelColor;
        }

        set
        {
            pressedLabelColor = value;
        }
    }

}