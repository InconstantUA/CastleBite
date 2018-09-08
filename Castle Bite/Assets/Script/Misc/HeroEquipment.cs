using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquipment : MonoBehaviour {
    UnitEquipmentButton callingUnitEquipmentButton;
    PartyUnit partyUnit;

    public UnitEquipmentButton CallingUnitEquipmentButton
    {
        get
        {
            return callingUnitEquipmentButton;
        }
    }

    public PartyUnit PartyUnit
    {
        get
        {
            return partyUnit;
        }
    }

    public void ActivateAdvance(UnitEquipmentButton unitEquipmentButton)
    {
        callingUnitEquipmentButton = unitEquipmentButton;
        partyUnit = callingUnitEquipmentButton.GetComponentInParent<PartyUnitUI>().LPartyUnit;
        // Activate intermediate background
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(true);
        // activate this menu
        gameObject.SetActive(true);
        // deactivate other uneeded menus
        callingUnitEquipmentButton.SetRequiredMenusActive(false);
        // bring left and right hero parties with inventories to te front
        transform.root.Find("MiscUI/LeftHeroParty").SetAsLastSibling();
        transform.root.Find("MiscUI/RightHeroParty").SetAsLastSibling();
    }

    public void DeactivateAdvance()
    {
        // Deactivate intermediate background
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(false);
        // deactivate this menu
        gameObject.SetActive(false);
        // activate other menus back
        callingUnitEquipmentButton.SetRequiredMenusActive(true);
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
                            if (Array.Find(partyUnit.UnitSkills, element => element.mName == UnitSkill.SkillName.ShardAura).mLevel.mCurrent >= 1)
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
