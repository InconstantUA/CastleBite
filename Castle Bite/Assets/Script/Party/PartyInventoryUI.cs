using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyInventoryUI : MonoBehaviour {
    [SerializeField]
    EquipmentSlotDropHandler equipmentSlotDropHandlerTemplate;
    [SerializeField]
    InventorySlotDropHandler inventorySlotDropHandlerTemplate;
    [SerializeField]
    InventoryItemDragHandler inventoryItemDragHandlerTemplate;
    [SerializeField]
    Transform inventoryItemsGrid;
    [SerializeField]
    int minNumberOfSlots = 3;
    [SerializeField]
    GameEvent partyInventoryUIHasBeenEnabledEvent;
    int slotID;

    string GetSlotID()
    {
        // increment slot ID
        slotID++;
        // return it as string
        return slotID.ToString();
    }

    public ItemSlotDropHandler AddSlot(InventoryItem inventoryItem = null, bool setCurrentItemEquipmentSlot = false)
    {
        // init slot variable
        ItemSlotDropHandler newSlot;
        // verify if we need to set current item equipment slot type to newly create slot
        if (setCurrentItemEquipmentSlot && inventoryItem != null)
        {
            Debug.Log("to review: Add ?equipment slot");
            //newSlot = Instantiate(equipmentSlotDropHandlerTemplate.gameObject, inventoryItemsGrid).GetComponent<ItemSlotDropHandler>();
            newSlot = Instantiate(inventorySlotDropHandlerTemplate.gameObject, inventoryItemsGrid).GetComponent<ItemSlotDropHandler>();
            // set the same slot type as item had in the past, this is needed for battle screen
            newSlot.EquipmentSlot = inventoryItem.CurrentHeroEquipmentSlot;
        }
        else
        {
            Debug.Log("Add inventory slot");
            newSlot = Instantiate(inventorySlotDropHandlerTemplate.gameObject, inventoryItemsGrid).GetComponent<ItemSlotDropHandler>();
        }
        // rename it for easier debugging
        newSlot.name = "ItemSlotDropHandler" + GetSlotID();
        // return new slot
        return newSlot;
    }

    public InventoryItemDragHandler AddItemDragHandler(ItemSlotDropHandler slot)
    {
        return Instantiate(inventoryItemDragHandlerTemplate.gameObject, slot.transform).GetComponent<InventoryItemDragHandler>();
    }

    public void RemoveAllEmptySlots()
    {
        // get all children 1 level below the parent
        Transform[] children = new Transform[inventoryItemsGrid.transform.childCount];
        int i = 0;
        foreach (Transform tempTransform in inventoryItemsGrid.transform)
        {
            children[i] = tempTransform;
            i++;
        }
        // loop through all objects in reverse order
        for (int j = children.Length - 1; j >= 0 ; j--)
        {
            // verify if slot is empty
            if (children[j].GetComponentInChildren<InventoryItemDragHandler>() == null)
            {
                // Remove slot
                RecycleBin.Recycle(children[j].gameObject);
            }
        }
        //// loop through all slots in this inventory
        //foreach (InventorySlotDropHandler slot in GetComponentsInChildren<InventorySlotDropHandler>())
        //{
        //    // verify if slot is empty
        //    if (slot.GetComponentInChildren<InventoryItemDragHandler>() == null)
        //    {
        //        // Remove slot
        //        Destroy(slot.gameObject);
        //    }
        //}
        //// loop through all slots in this inventory
        //foreach (EquipmentSlotDropHandler slot in GetComponentsInChildren<EquipmentSlotDropHandler>())
        //{
        //    // verify if slot is empty
        //    if (slot.GetComponentInChildren<InventoryItemDragHandler>() == null)
        //    {
        //        // Remove slot
        //        Destroy(slot.gameObject);
        //    }
        //}
    }

    //public void FillInEmptySlots()
    //{
    //    // there should be at least minNumberOfSlots item slots present in UI
    //    // .. Change this to list and get only first-level items, non-recursive
    //    int numberOfItems = 0;
    //    if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
    //    {
    //        foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.transform)
    //        {
    //            // verify if this is InventoryItem
    //            if (childTransform.GetComponent<InventoryItem>() != null)
    //                // increment items count
    //                numberOfItems += 1;
    //        }
    //    }
    //    // verify if we are in battle screen mode
    //    else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
    //    {
    //        // get all usable items from party leader equipment
    //        foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.GetPartyLeader().transform)
    //        {
    //            // get item
    //            InventoryItem inventoryItem = childTransform.GetComponent<InventoryItem>();
    //            // verify if there is an item
    //            if (inventoryItem != null)
    //            {
    //                // verify if item is in hero eqipment slot
    //                if (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlots.None)
    //                {
    //                    // increment items count
    //                    numberOfItems += 1;
    //                }
    //            }
    //        }
    //    }
    //    else if (transform.root.Find("MiscUI").GetComponentInChildren<PartiesInfoPanel>(false) != null)
    //    {
    //        Debug.Log("Parties info panel is active, probably we are on map. Normally inventory should not be active in this case.");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Unknown active screen");
    //    }
    //    // get number of empty item slots
    //    int emptySlots = minNumberOfSlots - numberOfItems;
    //    // create an empty slot for each empty slot
    //    for (int i = 0; i < emptySlots; i++)
    //    {
    //        // create slot in items list
    //        AddSlot();
    //    }
    //}

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
            if (inventoryItem.IsUsable)
            {
                dragHandler.GetComponentInChildren<Text>().text += inventoryItem.GetUsagesInfo();
            }
        }
    }

    public void DisplayCurrentPartyInventory()
    {
        // activate party inventory information
        // all items in a party
        // structure: LeftHeroParty-PartyInventory
        // create inventory slot and drag handler for each item
        foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.transform)
        {
            SetItemRepresentationInInventoryUI(childTransform.GetComponent<InventoryItem>());
        }
    }

    public void DisplayHeroEquipmentUsableInventory()
    {
        // get all usable items from party leader equipment
        foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.GetPartyLeader().transform)
        {
            // get item
            InventoryItem inventoryItem = childTransform.GetComponent<InventoryItem>();
            // verify if this is an item
            if (inventoryItem != null)
            {
                // verify if item is in hero eqipment slot and that it is usable
                if (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlots.None && inventoryItem.IsUsable)
                {
                    SetItemRepresentationInInventoryUI(childTransform.GetComponent<InventoryItem>(), true);
                }
            }
        }
    }

    void OnEnable()
    {
        // reset slot ID
        slotID = 0;
        // raise an event
        partyInventoryUIHasBeenEnabledEvent.Raise(this);
        //// verify if EditPartyScreen is active
        //if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
        //{
        //    // activate party inventory information
        //    // all items in a party
        //    // structure: LeftHeroParty-PartyInventory
        //    // InventoryItem[] inventoryItems = GetComponentInParent<HeroPartyUI>().LHeroParty.GetComponentsInChildren<InventoryItem>();
        //    // create inventory slot and drag handler for each item
        //    foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.transform)
        //    {
        //        SetItemRepresentationInInventoryUI(childTransform.GetComponent<InventoryItem>());
        //    }
        //    // fill in empty slots
        //    //FillInEmptySlots();
        //}
        //// verify if Battle screen is active
        //else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
        //{
        //    // get all usable items from party leader equipment
        //    foreach (Transform childTransform in GetComponentInParent<HeroPartyUI>().LHeroParty.GetPartyLeader().transform)
        //    {
        //        // get item
        //        InventoryItem inventoryItem = childTransform.GetComponent<InventoryItem>();
        //        // verify if there is an item
        //        if (inventoryItem != null)
        //        {
        //            // verify if item is in hero eqipment slot
        //            if (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlots.None)
        //            {
        //                SetItemRepresentationInInventoryUI(childTransform.GetComponent<InventoryItem>(), true);
        //            }
        //        }
        //    }
        //    // fill in empty slots
        //    //FillInEmptySlots();
        //}
        //// verify if PartiesInfoPanel is active
        //else if (transform.root.Find("MiscUI").GetComponentInChildren<PartiesInfoPanel>(false) != null)
        //{
        //    Debug.Log("Parties info panel is active, probably we are on map. No need to do anything, because party inventory will be disabled");
        //}
        //else
        //{
        //    Debug.LogWarning("unknown screen is active");
        //}
        ReorganizeInventoryUI();
    }

    void OnDisable()
    {
        //// remove all slots with items
        //foreach(ItemSlotDropHandler inventorySlot in GetComponentsInChildren<ItemSlotDropHandler>())
        //{
        //    Destroy(inventorySlot.gameObject);
        //}
        RecycleBin.RecycleChildrenOf(inventoryItemsGrid.gameObject);
    }

    public void ReorganizeInventoryUI()
    {
        Debug.LogWarning("ReorganizeInventoryUI");
        // init empty slots list
        List<Transform> emptySlots = new List<Transform>();
        // init number of occupied slots
        int occupiedSlotsCount = 0;
        // loop through all slots in a grid
        foreach (Transform child in inventoryItemsGrid)
        {
            // verify if slot is empty
            if (child.GetComponentInChildren<InventoryItemDragHandler>() == null)
            {
                //// verify if this is not the parent of the item being dragged
                //// because this slot is unofficially still occupied (if we remove it, then exchange between other slot may not happen successfully)
                //Debug.LogWarning("child: " + child.gameObject.name + "[" + child.gameObject.GetInstanceID() + "]" + ", original parent: " + InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot.gameObject.name + "[" + InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot.gameObject.GetInstanceID() + "]");
                //if (InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot.gameObject.GetInstanceID() != child.gameObject.GetInstanceID())
                //{
                //    // add new slot to the list of empty slots
                //    emptySlots.Add(child);
                //}
                //else
                //{
                //    // increment number of occupied slots
                //    occupiedSlotsCount += 1;
                //}
                // add new slot to the list of empty slots
                emptySlots.Add(child);
            }
            else
            {
                // increment number of occupied slots
                occupiedSlotsCount += 1;
            }
        }
        // get total number of slots
        int totalSlots = occupiedSlotsCount + emptySlots.Count;
        // verify if total number of slots is less than minimum
        if (totalSlots < minNumberOfSlots)
        {
            // add missing slots
            // get number of empty item slots
            int emptySlotsCountToAdd = minNumberOfSlots - totalSlots;
            // create an empty slot for each empty slot
            for (int i = 0; i < emptySlotsCountToAdd; i++)
            {
                // create slot in items list
                Debug.Log("Add new empty slot");
                AddSlot();
            }
        }
        // verify if total slots count is higher than min
        else if (totalSlots > minNumberOfSlots)
        {
            int emptySlotsCountToRemove = totalSlots - minNumberOfSlots;
            // verify if emptySlotsCountToRemove is higher than total number of empty slots
            if (emptySlotsCountToRemove > emptySlots.Count)
            {
                // reset emptySlotsCountToRemove to current number of empty slots
                emptySlotsCountToRemove = emptySlots.Count;
            }
            // Loop through empty slots in reverse order (so the strcture of List is not affected)
            // start from the emptySlotsCountToRemove
            for (int i = emptySlotsCountToRemove - 1; i >= 0; i--)
            {
                // remove extra empty slot
                Debug.Log("Remove extra empty slot");
                RecycleBin.Recycle(emptySlots[i].gameObject);
                emptySlots.RemoveAt(i);
            }
        }
        // verify if there is at least one occupied slot
        if (occupiedSlotsCount >= 1)
        {
            // move empty slots down
            foreach(Transform emptySlotTransform in emptySlots)
            {
                emptySlotTransform.SetAsLastSibling();
            }
        }
    }

    public void OnItemHasBeenDroppedIntoEquipmentSlot(System.Object inventorySlotDropHandler)
    {
        ReorganizeInventoryUI();
    }

    public void OnItemHasBeenDroppedIntoInventorySlot(System.Object inventorySlotDropHandler)
    {
        ReorganizeInventoryUI();
    }

}
