using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapObjectLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    Text labelTxt;
    MapObject mapObject;
    [SerializeField]
    bool isMouseOver = false;

    void Awake()
    {
        labelTxt = GetComponent<Text>();
        Debug.LogWarning("Label text: " + labelTxt.text);
        mapObject = transform.parent.GetComponent<MapObject>();
        labelTxt.color = mapObject.HiddenLabelColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("MapObjectLabel OnPointerEnter");
        // dimm all other menus
        // DimmAllOtherMenus();
        // highlight this menu
        isMouseOver = true;
        labelTxt.color = mapObject.HighlightedLabelColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("MapObjectLabel OnPointerExit");
        isMouseOver = false;
        // SetHiddenStatus();
        // Verify if mouse is not over MapObject, because in this case we do not need to hide label
        if (!mapObject.IsMouseOver)
        {
            HideLabel();
        }
    }

    public void HideLabel()
    {
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
        // Debug.Log("MapObjectLabel OnPointerDown");
        labelTxt.color = mapObject.PressedLabelColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("MapObjectLabel OnPointerUp");
        // keep state On
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("MapObjectLabel OnPointerClick");
        labelTxt.color = mapObject.HighlightedLabelColor;
        // give control on actions to map manager
        MapManager mapManager = transform.parent.parent.GetComponent<MapManager>();
        mapManager.ActOnClick(gameObject, eventData);
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
}
