using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    Text txt;
    // Color baseColor;
    Button btn;
    bool interactableDelay;

    void Start()
    {
        txt = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        btn = gameObject.GetComponent<Button>();
        interactableDelay = btn.interactable;
        // use the same color as set for button normal color
        // otherwise we will have to remember to change both values
        // because at the game start text will not have the same color as button
        Color tmpColor = btn.colors.normalColor;
        tmpColor.a = 1;
        txt.color = tmpColor;
    }

    void Update()
    {
        if (btn.interactable != interactableDelay)
        {
            Color tmpColor;
            if (btn.interactable)
            {
                tmpColor = btn.colors.normalColor;
            }
            else
            {
                tmpColor = btn.colors.disabledColor;
            }
            tmpColor.a = 1;
            txt.color = tmpColor;
        }
        interactableDelay = btn.interactable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color tmpColor;
        if (btn.interactable)
        {
            tmpColor = btn.colors.highlightedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Color tmpColor;
        if (btn.interactable)
        {
            tmpColor = btn.colors.pressedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Color tmpColor;
        if (btn.interactable)
        {
            tmpColor = btn.colors.highlightedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color tmpColor;
        if (btn.interactable)
        {
            tmpColor = btn.colors.normalColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
    }

}