using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Prolog : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public void SetActive(bool doActivate)
    {
        // activate/deactivate button
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/PrologBeginGameButton").gameObject.SetActive(doActivate);
        // activate/deactivate background
        transform.root.Find("MiscUI").GetComponentInChildren<BackgroundUI>(true).SetActive(doActivate);
        // activate/deactivate this menu
        gameObject.SetActive(doActivate);
    }

    public void BeginGame()
    {
        // deactivate prolog and game screen
        SetActive(false);
        // activate map
        MapManager.Instance.gameObject.SetActive(true);
        UIRoot.Instance.GetComponentInChildren<MapMenuManager>(true).gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // on left mouse click
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // on right mouse click
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
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
            // Skip prolog animation
            GetComponentInChildren<PrologAnimation>().Skip();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click

        }
    }

}
