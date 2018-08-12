﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;
    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        // disable raycasts
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        // change parent outside of Mask, to PartyInventory, so that canvas is not affected by Mask UI component
        // structure 4PartyInventory-3ItemsList(with Mask)-2Grid-1ItemSlot-Canvas(Item)
        transform.SetParent(transform.parent.parent.parent.parent);
    }
    #endregion
    #region IDragHandler implementation
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    #endregion
    #region IEndDragHandler implementation
    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == startParent)
        {
            transform.position = startPosition;
        }
    }
    #endregion
}
