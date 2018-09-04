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

    void OnEnable()
    {
        // all items in a party
        // structure: LeftHeroParty-PartyInventory
        InventoryItem[] inventoryItems = GetComponentInParent<HeroPartyUI>().LHeroParty.GetComponentsInChildren<InventoryItem>();
        // there should be at least 3 item slots present in UI
        // get number of empty item slots
        int emptySlots = 3 - inventoryItems.Length;
        // create inventory slot and drag handler for each item
        foreach(InventoryItem inventoryItem in inventoryItems)
        {
            // create slot in items list
            Transform slotTransform = Instantiate(inventoryItemDropHandlerTemplate, inventoryItemsGrid).transform;
            // create drag handler in slotTransform
            InventoryItemDragHandler dragHandler = Instantiate(inventoryItemDragHandlerTemplate, slotTransform).GetComponent<InventoryItemDragHandler>();
            // link item to drag handler
            dragHandler.LInventoryItem = inventoryItem;
            // set item name in UI
            dragHandler.GetComponentInChildren<Text>().text = inventoryItem.ItemName;
        }
        // create an empty slot for each empty slot
        for (int i = 0; i < emptySlots; i++)
        {
            // create slot in items list
            Instantiate(inventoryItemDropHandlerTemplate, inventoryItemsGrid);
        }
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
