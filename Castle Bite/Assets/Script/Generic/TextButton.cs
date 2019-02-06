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
    bool scaleHighlightedColorFromNormal = false;
    [SerializeField]
    float highlightedColorScale = 1.3f;
    [SerializeField]
    Color pressedColor;
    [SerializeField]
    bool scalePressedColorFromNormal = false;
    [SerializeField]
    float pressedColorScale = 1.6f;
    [SerializeField]
    Color disabledColor;
    // create event, which later can be configured in Unity Editor
    public UnityEvent OnClick;
    public UnityEvent OnLeftMouseButtonDown;
    public UnityEvent OnLeftMouseButtonUp;
    public UnityEvent OnRightMouseButtonDown;
    public UnityEvent OnRightMouseButtonUp;
    public UnityEvent OnMouseEnter;
    public UnityEvent OnMouseExit;

    public Color NormalColor
    {
        get
        {
            return normalColor;
        }

        set
        {
            normalColor = value;
        }
    }

    public Color HighlightedColor
    {
        get
        {
            return highlightedColor;
        }

        set
        {
            highlightedColor = value;
        }
    }

    public Color PressedColor
    {
        get
        {
            return pressedColor;
        }

        set
        {
            pressedColor = value;
        }
    }

    public Color DisabledColor
    {
        get
        {
            return disabledColor;
        }

        set
        {
            disabledColor = value;
        }
    }

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
                OnLeftMouseButtonDown.Invoke();
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
                OnLeftMouseButtonUp.Invoke();
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
        if (scaleHighlightedColorFromNormal)
        {
            // GetComponent<Text>().color = new Color32((byte)((normalColor.r * 255) + highlightedColorScale), (byte)((normalColor.g * 255) + highlightedColorScale), (byte)((normalColor.b * 255) + highlightedColorScale), (byte)(normalColor.a * 255));
            GetComponent<Text>().color = new Color(normalColor.r * highlightedColorScale, normalColor.g * highlightedColorScale, normalColor.b * highlightedColorScale, normalColor.a);
        }
        else
        {
            GetComponent<Text>().color = highlightedColor;
        }
    }

    public void SetPressedStatus()
    {
        if (scalePressedColorFromNormal)
        {
            // GetComponent<Text>().color = new Color32((byte)((normalColor.r * 255) + pressedColorScale), (byte)((normalColor.g * 255) + pressedColorScale), (byte)((normalColor.b * 255) + pressedColorScale), (byte)(normalColor.a * 255));
            GetComponent<Text>().color = new Color(normalColor.r * pressedColorScale, normalColor.g * pressedColorScale, normalColor.b * pressedColorScale, normalColor.a);
        }
        else
        {
            GetComponent<Text>().color = pressedColor;
        }
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

    public void TriggerEvent(GameEvent gameEvent)
    {
        gameEvent.Raise();
    }
}
