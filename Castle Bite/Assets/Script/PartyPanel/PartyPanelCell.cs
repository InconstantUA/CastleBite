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
                // try to consume item in preview mode without actually doing anything
                if (partyUnitUI.LPartyUnit.UseItem(InventoryItemDragHandler.itemBeingDragged.LInventoryItem, true))
                {
                    // item can be applied to this hero, highlight with applicable color
                    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
                }
                else
                {
                    // item cannot be applied to this hero, highlight with not applicable color
                    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
                }
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
