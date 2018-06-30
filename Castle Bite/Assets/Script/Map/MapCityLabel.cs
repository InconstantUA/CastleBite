using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapCityLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
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
    MapCity mapCity;
    bool isMouseOver = false;

    public MapCity MapCity
    {
        get
        {
            return mapCity;
        }

        set
        {
            mapCity = value;
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
        MapCity = transform.parent.GetComponent<MapCity>();
        labelTxt.color = inactiveColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("MapCityLabel OnPointerEnter");
        // dimm all other menus
        // DimmAllOtherMenus();
        // highlight this menu
        isMouseOver = true;
        SetHighlightedStatus();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("MapCityLabel OnPointerExit");
        isMouseOver = false;
        SetHiddenStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("MapCityLabel OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("MapCityLabel OnPointerUp");
        // keep state On
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("MapCityLabel OnPointerClick");
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
        // do this only if mouse is not over this lable and not over parent city marker
        if (!isMouseOver && !mapCity.IsMouseOver)
        {
            labelTxt.color = inactiveColor;
            labelTxt.raycastTarget = false;
        }
    }

}
