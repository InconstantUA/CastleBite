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

    public Text CanvasText
    {
        get
        {
            return canvasText;
        }
    }

    public bool IsOccupied()
    {
        // verify if cell has party unit UI
        if (GetComponentInChildren<PartyUnitUI>() != null)
        {
            return true;
        }
        return false;
    }

    public void OnBeginItemDrag()
    {
        // Debug.LogWarning("OnBeginItemDrag");
        // save original color
        // Debug.LogWarning("Save original color");
        beforeItemDragColor = CanvasText.color;
        // verify if item is usable
        if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].AreRequirementsMetInContextOf(InventoryItemDragHandler.itemBeingDragged.LInventoryItem, this))
        {
            Debug.Log("Cell Requirements are met");
            // verify if party unit is not present, because if it is present, then we need to give it a possibility to override "applicability"
            // it maybe already overritten, because PartyUnitUI could possibly react earlier on event
            // get party unit UI
            PartyUnitUI partyUnitUI = GetComponentInChildren<PartyUnitUI>();
            // verify if its not null
            if (partyUnitUI != null)
            {
                // let party unit override highlights and react on begin item drag event
                partyUnitUI.ActOnBeginItemDrag();
            }
            else
            {
                // highlight with applicable color
                CanvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
            }
        }
        else
        {
            Debug.Log("Cell Requirements are not met");
            // highlight with not applicable color
            CanvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        }
        //// verify if item has active modifiers or usages
        //if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.HasActiveModifiers())
        //{
        //    Debug.Log("Item has active modifiers");
        //    // get party unit UI
        //    PartyUnitUI partyUnitUI = GetComponentInChildren<PartyUnitUI>();
        //    // verify if its not null
        //    if (partyUnitUI != null)
        //    {
        //        Debug.Log("Found partyUnitUI");
        //        // activate highlight
        //        // get source context 
        //        // try to get party unit (assume that during battle unit can only use items which are located in (childs of) this unit game object)
        //        // if outside of the battle or if item is dragged from inventiry, then this will result in null
        //        System.Object srcContext = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.GetComponentInParent<PartyUnit>();
        //        // verify if srcPartyUnit is null
        //        if (srcContext == null)
        //        {
        //            // context is hero party (item is dragged from inventory)
        //            // get party
        //            HeroParty heroParty = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.GetComponentInParent<HeroParty>();
        //            // verify if party is garnizon type
        //            if (heroParty.PartyMode == PartyMode.Garnizon)
        //            {
        //                // set context to the city
        //                srcContext = heroParty.GetComponentInParent<City>();
        //            }
        //            else
        //            {
        //                // party mode = normal party
        //                // set context to the party leader
        //                srcContext = heroParty.GetPartyLeader();
        //            }
        //        }
        //        // verify if UPM can be applied to destination unit
        //        if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].AreRequirementsMetInContextOf(srcContext, partyUnitUI.LPartyUnit) )
        //        {
        //            Debug.Log("Requirements are met");
        //            // verify if it is advised to use this item in this context
        //            if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].IsItAdvisedToActInContextOf(srcContext, partyUnitUI.LPartyUnit))
        //            {
        //                Debug.Log("Advised");
        //                // advised
        //                // item can be applied to this hero, highlight with applicable color
        //                canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
        //            }
        //            else
        //            {
        //                Debug.Log("Not Advised");
        //                // not advised
        //                // item can be applied to this hero, highlight with applicable color
        //                canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableButNotAdvisedForUnitSlotColor;
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("Requirements are not met");
        //            // item cannot be applied to this hero, highlight with not applicable color
        //            canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //        }
        //        //// try to consume item in preview mode without actually doing anything
        //        //if (partyUnitUI.LPartyUnit.UseItem(InventoryItemDragHandler.itemBeingDragged.LInventoryItem, true))
        //        //{
        //        //    // item can be applied to this hero, highlight with applicable color
        //        //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
        //        //}
        //        //else
        //        //{
        //        //    // item cannot be applied to this hero, highlight with not applicable color
        //        //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //        //}
        //    }
        //    else
        //    {
        //        Debug.Log("no party unit UI");
        //        // there is no hero in this slot, highlight with not applicable color
        //        canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //    }
        //}
        //else
        //{
        //    Debug.Log("Item has no active modifiers");
        //    // item is not consumable, highlight with not applicable color
        //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //}
    }

    public void OnEndItemDrag()
    {
        // Debug.LogWarning("Restore original color");
        // restore original color
        CanvasText.color = beforeItemDragColor;
    }
}
