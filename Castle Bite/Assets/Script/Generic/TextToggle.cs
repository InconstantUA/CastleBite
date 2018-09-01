using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    public bool interactable;
    public bool selected;
    public TextToggleGroup toggleGroup;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color highlightedColor;
    [SerializeField]
    Color pressedColor;
    [SerializeField]
    Color disabledColor;
    // create event, which later can be configured in Unity Editor
    public UnityEvent OnTurnOn;
    public UnityEvent OnTurnOff;
    public UnityEvent OnRightMouseButtonDown;
    public UnityEvent OnMouseEnter;
    public UnityEvent OnMouseExit;
    bool mouseIsOver = false;

    void Start()
    {
    }

    void OnDisable()
    {
        // turn off toggle if it was active
        TurnOff();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
        // highlight this menu
        if (interactable)
        {
            SetHighlightedStatus();
            mouseIsOver = true;
            // Trigger events defined in Unity Editor
            OnMouseEnter.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        if (interactable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // on left mouse
                SetPressedStatus();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // on right mouse
                OnRightMouseButtonDown.Invoke();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (interactable)
        {
            // Debug.Log("OnPointerUp");
            if (Input.GetMouseButtonUp(0))
            {
                // on left mouse
            }
            else if (Input.GetMouseButtonUp(1))
            {
                // on right mouse
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable)
        {
            // verify if it is selected
            if (selected)
            {
                // return to pressed state
                SetPressedStatus();
            }
            else
            {
                // return to normal state
                SetNormalStatus();
            }
            mouseIsOver = false;
            // Trigger events defined in Unity Editor
            OnMouseExit.Invoke();
        }
    }

    public void TurnOff()
    {
        // deselect
        selected = false;
        // verify if mouse is over
        if (mouseIsOver)
        {
            SetHighlightedStatus();
        }
        else
        {
            SetNormalStatus();
        }
    }

    public void TurnOn()
    {
        // select
        selected = true;
        SetPressedStatus();
    }

    public void ActOnLeftMouseClick()
    {
        // verify if it is selected
        if (selected)
        {
            TurnOff();
            OnTurnOff.Invoke();
            // verify if toggle is part of the group
            if (toggleGroup != null)
            {
                // instruct toggle group to act on this
                toggleGroup.DeselectToggle();
            }
        }
        else
        {
            TurnOn();
            OnTurnOn.Invoke();
            // verify if toggle is part of the group
            if (toggleGroup != null)
            {
                // instruct toggle group to act on this
                toggleGroup.SetSelectedToggle(this);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (interactable)
        {
            // trigger registered via Unity editor functions
            if (Input.GetMouseButtonUp(0))
            {
                // on left mouse click
                ActOnLeftMouseClick();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                // on right mouse click
            }
        }
    }

    void SetHighlightedStatus()
    {
        GetComponent<Text>().color = highlightedColor;
    }

    public void SetPressedStatus()
    {
        GetComponent<Text>().color = pressedColor;
    }

    public void SetNormalStatus()
    {
        GetComponent<Text>().color = normalColor;
    }

    public void SetDisabledStatus()
    {
        GetComponent<Text>().color = disabledColor;
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
        if (interactable)
        {
            SetNormalStatus();
        }
        else
        {
            SetDisabledStatus();
        }
    }
}
