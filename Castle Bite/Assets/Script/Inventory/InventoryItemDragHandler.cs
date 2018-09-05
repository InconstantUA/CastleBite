using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField]
    InventoryItem lInventoryItem;
    public static InventoryItemDragHandler itemBeingDragged;
    //Vector3 startPosition;
    InventorySlotDropHandler itemBeingDraggedSlot;
    Transform outOfMaskParentTransform;

    public InventoryItem LInventoryItem
    {
        get
        {
            return lInventoryItem;
        }

        set
        {
            lInventoryItem = value;
        }
    }

    public InventorySlotDropHandler ItemBeindDraggedSlot
    {
        get
        {
            return itemBeingDraggedSlot;
        }
    }

    void BringItemToFront()
    {
        // set parent panel to the top layer
        // structure: 5[Right-LeftHeroParty]-4PartyInventory-3ItemsList-2Grid-1ItemSlot(Drop)-Item(Drag)
        //transform.parent.SetAsLastSibling(); // Item slot
        transform.parent.parent.parent.parent.SetAsLastSibling(); // PartyInventory
        transform.parent.parent.parent.parent.parent.SetAsLastSibling(); // HeroParty
        // Bring hire Common Unit Buttons to the front, otherwise they are not visible
        transform.root.Find("MiscUI/HireCommonUnitButtons").SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BringItemToFront();
        itemBeingDragged = this;
        //startPosition = transform.position;
        itemBeingDraggedSlot = GetComponentInParent<InventorySlotDropHandler>();
        // change parent outside of Mask, to PartyInventory, so that canvas is not affected by Mask UI component
        // structure 4PartyInventory-3ItemsList-2Grid-1ItemSlot(Drop)-Item(Drag)
        transform.SetParent(transform.parent.parent.parent.parent);
        outOfMaskParentTransform = transform.parent.transform;
        // disable raycasts
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        // verify if on drop was not triggered
        if (transform.parent == outOfMaskParentTransform)
        {
            Debug.Log("Return to original position");
            // change parent back to original
            transform.SetParent(itemBeingDraggedSlot.transform);
            // reset position to 0/0/0/0
            // [ left - bottom ]
            GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            // [ right - top ]
            GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        }
    }
}
