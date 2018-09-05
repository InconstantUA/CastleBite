using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanelDropHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryItemDragHandler.itemBeingDragged != null)
        {
            Debug.Log("Find or create the right slot and put an item into it");
            //// Change parent to the right slot transform
            //InventoryItemDragHandler.itemBeingDragged.transform.SetParent(transform);
            //// Reset position to 0/0/0/0
            //InventoryItemDragHandler.itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // [ left - bottom ]
            //InventoryItemDragHandler.itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // [ right - top ]
        }
    }
}
