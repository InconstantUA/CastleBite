using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Prolog : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

	// Use this for initialization
	void Start () {
		
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
