using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquipment : MonoBehaviour {
    UnitEquipmentButton callingUnitEquipmentButton;

    public UnitEquipmentButton CallingUnitEquipmentButton
    {
        get
        {
            return callingUnitEquipmentButton;
        }
    }

    public void ActivateAdvance(UnitEquipmentButton unitEquipmentButton)
    {
        callingUnitEquipmentButton = unitEquipmentButton;
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
                        // set compatible flag
                        isCompatible = true;
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
