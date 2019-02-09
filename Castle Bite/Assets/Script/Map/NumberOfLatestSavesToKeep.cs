using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NumberOfLatestSavesToKeep : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorController.Instance.SetEditTextCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorController.Instance.SetNormalCursor();
    }
}

