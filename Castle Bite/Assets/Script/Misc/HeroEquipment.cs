using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquipment : MonoBehaviour {
    [SerializeField]
    GameObject inventoryItemDragHandlerTemplate;
    UnitEquipmentButton lUnitEquipmentButton;
    PartyUnit lPartyUnit;

    public UnitEquipmentButton LUnitEquipmentButton
    {
        get
        {
            return lUnitEquipmentButton;
        }
    }

    public PartyUnit LPartyUnit
    {
        get
        {
            return lPartyUnit;
        }
    }

    InventorySlotDropHandler GetEquipmentSlotByType(HeroEquipmentSlot heroEquipmentSlot)
    {
        // loop through all equipment slots
        foreach(InventorySlotDropHandler inventorySlotDropHandler in GetComponentsInChildren<InventorySlotDropHandler>(true))
        {
            // verify if inventorySlotDropHandler match required slot
            if (inventorySlotDropHandler.EquipmentSlot == heroEquipmentSlot)
            {
                return inventorySlotDropHandler;
            }
        }
        // this should not happen, because all slot types, except None should be present in hero Equipment
        Debug.LogError("Cound find slot of " + heroEquipmentSlot.ToString() + " type");
        return null;
    }

    // on change - check similar function in PartyInventoryUI class
    void SetItemRepresentationInEquipmentUI(InventoryItem inventoryItem)
    {
        // verify if this is InventoryItem
        if (inventoryItem != null)
        {
            // create drag handler and slotTransform
            InventoryItemDragHandler dragHandler = Instantiate(inventoryItemDragHandlerTemplate, GetEquipmentSlotByType(inventoryItem.HeroEquipmentSlot).transform).GetComponent<InventoryItemDragHandler>();
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

    void SetEquipmentSlots(PartyUnit partyUnit)
    {
        // loop through all items owned by unit
        foreach(InventoryItem inventoryItem in partyUnit.GetComponentsInChildren<InventoryItem>())
        {
            // verify if item has equipment slot assigned to it
            if (inventoryItem.HeroEquipmentSlot != HeroEquipmentSlot.None)
            {
                // set item representation in equipment UI
                SetItemRepresentationInEquipmentUI(inventoryItem);
            }
        }
    }

    public void ActivateAdvance(UnitEquipmentButton unitEquipmentButton)
    {
        lUnitEquipmentButton = unitEquipmentButton;
        lPartyUnit = lUnitEquipmentButton.GetComponentInParent<PartyUnitUI>().LPartyUnit;
        // Activate intermediate background
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(true);
        // activate this menu
        gameObject.SetActive(true);
        // deactivate other uneeded menus
        lUnitEquipmentButton.SetRequiredMenusActive(false);
        // bring left and right hero parties inventories with disabled party panels to te front
        transform.root.Find("MiscUI/LeftHeroParty").SetAsLastSibling();
        transform.root.Find("MiscUI/RightHeroParty").SetAsLastSibling();
        // set equipment slots with items
        SetEquipmentSlots(lPartyUnit);
    }

    public void DeactivateAdvance()
    {
        // Deactivate intermediate background
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(false);
        // remove all InventoryItemDragHandlers
        foreach(InventoryItemDragHandler itemDragHandler in GetComponentsInChildren<InventoryItemDragHandler>())
        {
            Destroy(itemDragHandler.gameObject);
        }
        // deactivate this menu
        gameObject.SetActive(false);
        // activate other menus back
        lUnitEquipmentButton.SetRequiredMenusActive(true);
    }

    public void SetActiveItemDrag(bool activate)
    {
        Color greenHighlight = Color.green;
        Color redHighlight = Color.red;
        Color normalColor = new Color(0.5f, 0.5f, 0.5f);
        Color hightlightColor;
        bool isCompatible = false;
        bool isDroppable = false;
        // loop through all slots in hero equipment
        foreach (InventorySlotDropHandler slot in GetComponentsInChildren<InventorySlotDropHandler>())
        {
            // reset highlight color to normal
            hightlightColor = normalColor;
            // verify if we need to activate or deactivate state
            if (activate)
            {
                // reset is compatible flag
                isCompatible = false;
                // reset is droppable flag
                isDroppable = false;
                // loop through all compatible slots types
                foreach (HeroEquipmentSlot slotType in InventoryItemDragHandler.itemBeingDragged.LInventoryItem.CompatibleEquipmentSlots)
                {
                    // verify if equipment slot is compatible
                    if (slot.EquipmentSlot == slotType)
                    {
                        // verify if this is shard slot
                        if (slotType == HeroEquipmentSlot.Shard)
                        {
                            // for shard slot we need to verify if party leader has skill at least 1st level
                            if (Array.Find(lPartyUnit.UnitSkills, element => element.mName == UnitSkill.SkillName.ShardAura).mLevel.mCurrent >= 1)
                            {
                                // set compatible flag
                                isCompatible = true;
                            } else
                            {
                                // set not compatible flag
                                isCompatible = false;
                            }
                        }
                        else
                        {
                            // set compatible flag
                            isCompatible = true;
                        }
                        // exit loop
                        break;
                    }
                }
                // verify if slot is compatible
                if (isCompatible)
                {
                    // set highlight color to green
                    hightlightColor = greenHighlight;
                    // set droppable flag
                    isDroppable = true;
                }
                else
                {
                    // set highlight color to green
                    hightlightColor = redHighlight;
                }
            }
            // set color in UI
            slot.GetComponentInChildren<Text>().color = hightlightColor;
            // set slot is droppable or not status
            slot.IsDroppable = isDroppable;
        }
        // and disable/enable hire buttons
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().SetHireUnitPnlButtonActive(!activate);
    }
}
