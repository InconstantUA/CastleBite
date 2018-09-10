using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    public bool interactable;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color highlightedColor;
    [SerializeField]
    Color pressedColor;
    [SerializeField]
    Color disabledColor;
    // create event, which later can be configured in Unity Editor
    public UnityEvent OnClick;
    public UnityEvent OnRightMouseButtonDown;
    public UnityEvent OnRightMouseButtonUp;
    public UnityEvent OnMouseEnter;
    public UnityEvent OnMouseExit;

    void Start()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
        // highlight this menu
        if (interactable)
        {
            SetHighlightedStatus();
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
                OnRightMouseButtonUp.Invoke();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable)
        {
            // return to previous toggle state
            SetNormalStatus();
            // Trigger events defined in Unity Editor
            OnMouseExit.Invoke();
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
                SetHighlightedStatus();
                OnClick.Invoke();
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
