using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyPanelCell : MonoBehaviour
{
    [SerializeField]
    PartyPanel.Cell cell;
    [SerializeField]
    Text canvasText;

    private Color beforeItemDragColor;

    public PartyPanel.Cell Cell
    {
        get
        {
            return cell;
        }
    }

    public void OnBeginItemDrag()
    {
        Debug.LogWarning("OnBeginItemDrag");
        // save original color
        beforeItemDragColor = canvasText.color;
        // verify if item has active modifiers or usages
        if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.HasActiveModifiers())
        {
            // get party unit UI
            PartyUnitUI partyUnitUI = GetComponentInChildren<PartyUnitUI>();
            // verify if its not null
            if (partyUnitUI != null)
            {
                // activate highlight
                // get source context 
                // try to get party unit (assume that during battle unit can only use items which are located in (childs of) this unit game object)
                // if outside of the battle or if item is dragged from inventiry, then this will result in null
                System.Object srcContext = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.GetComponentInParent<PartyUnit>();
                // verify if srcPartyUnit is null
                if (srcContext == null)
                {
                    // context is hero party (item is dragged from inventory)
                    // get party
                    HeroParty heroParty = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.GetComponentInParent<HeroParty>();
                    // verify if party is garnizon type
                    if (heroParty.PartyMode == PartyMode.Garnizon)
                    {
                        // set context to the city
                        srcContext = heroParty.GetComponentInParent<City>();
                    }
                    else
                    {
                        // party mode = normal party
                        // set context to the party leader
                        srcContext = heroParty.GetPartyLeader();
                    }
                }
                // verify if UPM can be applied to destination unit
                if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].AreRequirementsMetInContextOf(srcContext, partyUnitUI.LPartyUnit) )
                {
                    // verify if it is advised to use this item in this context
                    if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].IsItAdvisedToActInContextOf(srcContext, partyUnitUI.LPartyUnit))
                    {
                        // advised
                        // item can be applied to this hero, highlight with applicable color
                        canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
                    }
                    else
                    {
                        // not advised
                        // item can be applied to this hero, highlight with applicable color
                        canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableButNotAdvisedForUnitSlotColor;
                    }
                }
                else
                {
                    // item cannot be applied to this hero, highlight with not applicable color
                    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
                }
                //// try to consume item in preview mode without actually doing anything
                //if (partyUnitUI.LPartyUnit.UseItem(InventoryItemDragHandler.itemBeingDragged.LInventoryItem, true))
                //{
                //    // item can be applied to this hero, highlight with applicable color
                //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
                //}
                //else
                //{
                //    // item cannot be applied to this hero, highlight with not applicable color
                //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
                //}
            }
            else
            {
                // there is no hero in this slot, highlight with not applicable color
                canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
            }
        }
        else
        {
            // item is not consumable, highlight with not applicable color
            canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        }
    }

    public void OnEndItemDrag()
    {
        // restore original color
        canvasText.color = beforeItemDragColor;
    }
}
