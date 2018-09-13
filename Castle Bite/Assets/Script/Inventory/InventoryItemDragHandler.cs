using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        if (Input.GetMouseButton(0))
        {
            // Debug.Log("OnBeginDrag: left mouse");
            // on left mouse drag
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
            // Instruct Edit Party Screen that item is being dragged
            transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(true).SetActiveState(EditPartyScreenActiveState.ActiveItemDrag, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // verify if user clicked left or right mouse button
        if (Input.GetMouseButtonUp(0))
        {
            // Instruct Edit Party Screen to disable ActiveItemDrag state (true) because we might be in battle screen mode using item
            transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(true).SetActiveState(EditPartyScreenActiveState.ActiveItemDrag, false);
            // get Battle screen
            BattleScreen battleScreen = transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false);
            // Verify if battle screen is active
            if (battleScreen != null)
            {
                // Instruct Battle screen to update units highlight
                battleScreen.SetHighlight();
            }
            // reset item being dragged
            itemBeingDragged = null;
            // enable block raycasts, so item can be dragged again
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // verify if on drop was not triggered (parent has not changed)
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
            else
            {
                // parent has changed
                // verify if previous parent was hero eqiupment slot
                if (InventorySlotDropHandler.Mode.HeroEquipment == itemBeingDraggedSlot.SlotMode)
                {
                    // update unit info UI
                    transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>().ActivateAdvance(itemBeingDraggedSlot.GetComponentInParent<HeroEquipment>().LPartyUnit, UnitInfoPanel.Align.Right, false, UnitInfoPanel.Mode.Short);
                }
            }
        }
        // verify if there is still item linked
        if (LInventoryItem != null)
        {
            // verify if item has usages
            if (LInventoryItem.HasActiveModifiers())
            {
                // update usages info, because it may be reduced, if item is usable
                GetComponentInChildren<Text>().text = LInventoryItem.ItemName + LInventoryItem.GetUsagesInfo();
            }
        }
    }

    public void ShowItemInformation()
    {
        // set content popup box with information
        transform.root.Find("MiscUI").GetComponentInChildren<InventoryItemInfoPanel>(true).SetActive(LInventoryItem);
    }

    public void HideItemInformation()
    {
        // set content popup box with information
        transform.root.Find("MiscUI").GetComponentInChildren<InventoryItemInfoPanel>(true).gameObject.SetActive(false);
    }
}
