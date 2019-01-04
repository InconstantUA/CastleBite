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

    public InventorySlotDropHandler AddSlot(InventoryItem inventoryItem = null, bool setCurrentItemEquipmentSlot = false)
    {
        Debug.Log("Add slot");
        InventorySlotDropHandler newSlot = Instantiate(inventoryItemDropHandlerTemplate, inventoryItemsGrid).GetComponent<InventorySlotDropHandler>();
        // verify if we need to set current item equipment slot type to newly create slot
        if (setCurrentItemEquipmentSlot && inventoryItem != null)
        {
            // set the same slot type as item had in the past, this is needed for battle screen
            newSlot.EquipmentSlot = inventoryItem.CurrentHeroEquipmentSlot;
        }
        return newSlot;
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
        if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
        {
            foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.transform)
            {
                // verify if this is InventoryItem
                if (childTransform.GetComponent<InventoryItem>() != null)
                    // increment items count
                    numberOfItems += 1;
            }
        }
        // verify if we are in battle screen mode
        else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
        {
            // get all usable items from party leader equipment
            foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.GetPartyLeader().transform)
            {
                // get item
                InventoryItem inventoryItem = childTransform.GetComponent<InventoryItem>();
                // verify if there is an item
                if (inventoryItem != null)
                {
                    // verify if item is in hero eqipment slot
                    if (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlots.None)
                    {
                        // increment items count
                        numberOfItems += 1;
                    }
                }
            }
        }
        else if (transform.root.Find("MiscUI").GetComponentInChildren<PartiesInfoPanel>(false) != null)
        {
            Debug.Log("Parties info panel is active, probably we are on map. Normally inventory should not be active in this case.");
        }
        else
        {
            Debug.LogWarning("Unknown active screen");
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

    // on change - check similar function in HeroEquipment class
    public void SetItemRepresentationInInventoryUI(InventoryItem inventoryItem, bool setCurrentItemEquipmentSlot = false)
    {
        // verify if this is InventoryItem
        if (inventoryItem != null)
        {
            // create drag handler and slotTransform
            InventoryItemDragHandler dragHandler = AddItemDragHandler(AddSlot(inventoryItem, setCurrentItemEquipmentSlot));
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

    void OnEnable()
    {
        // verify if EditPartyScreen is active
        if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
        {
            // activate party inventory information
            // all items in a party
            // structure: LeftHeroParty-PartyInventory
            // InventoryItem[] inventoryItems = GetComponentInParent<HeroPartyUI>().LHeroParty.GetComponentsInChildren<InventoryItem>();
            // create inventory slot and drag handler for each item
            foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.transform)
            {
                SetItemRepresentationInInventoryUI(childTransform.GetComponent<InventoryItem>());
            }
            // fill in empty slots
            FillInEmptySlots();
        }
        // verify if Battle screen is active
        else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
        {
            // get all usable items from party leader equipment
            foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.GetPartyLeader().transform)
            {
                // get item
                InventoryItem inventoryItem = childTransform.GetComponent<InventoryItem>();
                // verify if there is an item
                if (inventoryItem != null)
                {
                    // verify if item is in hero eqipment slot
                    if (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlots.None)
                    {
                        SetItemRepresentationInInventoryUI(childTransform.GetComponent<InventoryItem>(), true);
                    }
                }
            }
            // fill in empty slots
            FillInEmptySlots();
        }
        // verify if PartiesInfoPanel is active
        else if (transform.root.Find("MiscUI").GetComponentInChildren<PartiesInfoPanel>(false) != null)
        {
            Debug.Log("Parties info panel is active, probably we are on map. No need to do anything, because party inventory will be disabled");
        }
        else
        {
            Debug.LogWarning("unknown screen is active");
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
