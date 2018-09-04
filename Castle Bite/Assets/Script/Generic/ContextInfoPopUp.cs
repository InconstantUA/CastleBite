using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContextInfoPopUp : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    public void SetActive(bool doActivate, string message = "")
    {
        gameObject.SetActive(doActivate);
        // set text information
        transform.Find("MessageBox/Text").GetComponent<Text>().text = message;
        // GetComponentInChildren<Transform>(true).GetComponentInChildren<Text>(true).text = message;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate this menu
            SetActive(false);
        }
    }
}
