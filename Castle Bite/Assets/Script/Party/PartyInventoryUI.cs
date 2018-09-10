using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyInventoryUI : MonoBehaviour {
    [SerializeField]
    GameObject inventoryItemDropHandlerTemplate;
    [SerializeField]
    GameObject inventoryItemDragHandlerTemplate;
    [SerializeField]
    Transform inventoryItemsGrid;

    public InventorySlotDropHandler AddSlot()
    {
        Debug.Log("Add slot");
        return Instantiate(inventoryItemDropHandlerTemplate, inventoryItemsGrid).GetComponent<InventorySlotDropHandler>();
    }

    public InventoryItemDragHandler AddItemDragHandler(InventorySlotDropHandler slot)
    {
        return Instantiate(inventoryItemDragHandlerTemplate, slot.transform).GetComponent<InventoryItemDragHandler>();
    }

    public void RemoveAllEmptySlots()
    {
        // loop through all slots in this inventory
        foreach (InventorySlotDropHandler slot in GetComponentsInChildren<InventorySlotDropHandler>())
        {
            // verify if slot is empty
            if (slot.GetComponentInChildren<InventoryItemDragHandler>() == null)
            {
                // Remove slot
                Destroy(slot.gameObject);
            }
        }
    }

    public void FillInEmptySlots()
    {
        // there should be at least 3 item slots present in UI
        // .. Change this to list and get only first-level items, non-recursive
        int numberOfItems = 0;
        foreach(Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.transform)
        {
            // verify if this is InventoryItem
            if (childTransform.GetComponent<InventoryItem>() != null)
                // increment items count
                numberOfItems += 1;
        }
        // get number of empty item slots
        int emptySlots = 3 - numberOfItems;
        // create an empty slot for each empty slot
        for (int i = 0; i < emptySlots; i++)
        {
            // create slot in items list
            AddSlot();
        }
    }

    void OnEnable()
    {
        // all items in a party
        // structure: LeftHeroParty-PartyInventory
        // InventoryItem[] inventoryItems = GetComponentInParent<HeroPartyUI>().LHeroParty.GetComponentsInChildren<InventoryItem>();
        // create inventory slot and drag handler for each item
        foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.transform)
        {
            InventoryItem inventoryItem = childTransform.GetComponent<InventoryItem>();
            // verify if this is InventoryItem
            if (inventoryItem != null)
            {
                // create drag handler and slotTransform
                InventoryItemDragHandler dragHandler = AddItemDragHandler(AddSlot());
                // link item to drag handler
                dragHandler.LInventoryItem = inventoryItem;
                // set item name in UI
                dragHandler.GetComponentInChildren<Text>().text = inventoryItem.ItemName;
                // verify if item has active modifiers
                if (inventoryItem.HasActiveModifiers())
                {
                    dragHandler.GetComponentInChildren<Text>().text += inventoryItem.GetUsagesInfo();
                }
            }
        }
        // fill in empty slots
        FillInEmptySlots();
    }

    void OnDisable()
    {
        // remove all slots with items
        foreach(InventorySlotDropHandler inventorySlot in GetComponentsInChildren<InventorySlotDropHandler>())
        {
            Destroy(inventorySlot.gameObject);
        }
    }
}
