using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSlotDropHandler : MonoBehaviour, IDropHandler
{
    public GameObject unit
    {
        get
        {
            // Debug.Log("Child count: " + transform.childCount.ToString());
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // drop unit if there is no other unit already present
        if (!unit)
        {
            // enable hire unit button in the original parent cell and bring it to the front
            UnitDragHandler.unitBeingDragged.transform.parent.parent.Find("HireUnitPnlBtn").gameObject.SetActive(true);
            UnitDragHandler.unitBeingDragged.transform.parent.parent.Find("HireUnitPnlBtn").SetAsLastSibling();
            // change parent of the dragged unit
            Debug.Log("Drop unit from " + UnitDragHandler.unitBeingDraggedParentTr.parent.gameObject.name);
            UnitDragHandler.unitBeingDragged.transform.SetParent(transform);
            // reset position to 0/0/0/0
            // [ left - bottom ]
            UnitDragHandler.unitBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            // [ right - top ]
            UnitDragHandler.unitBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            // disable hire unit button in the destination (this) cell
            transform.parent.Find("HireUnitPnlBtn").gameObject.SetActive(false);
            // trigger event
            // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
        }
    }
}
