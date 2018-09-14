using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPickUpPopUp : MonoBehaviour {

    void OnDisable()
    {
        // clean up all item slots in inventory
        foreach(InventorySlotDropHandler slot in GetComponentsInChildren<InventorySlotDropHandler>())
        {
            Destroy(slot.gameObject);
        }
    }

    public void SetActive(MapItemsContainer mapItem)
    {
        // Activate this object
        gameObject.SetActive(true);
        // Get party inventory UI
        PartyInventoryUI partyInventoryUI = transform.Find("Panel").GetComponentInChildren<PartyInventoryUI>();
        // Loop through each item in the chest
        foreach (InventoryItem inventoryItem in mapItem.LInventoryItems)
        {
            // set item representation in inventory UI
            partyInventoryUI.SetItemRepresentationInInventoryUI(inventoryItem);
        }
        // do clean up, because by default PartyInventoryUI on enable fills in empty slots
        partyInventoryUI.RemoveAllEmptySlots();
    }
}
