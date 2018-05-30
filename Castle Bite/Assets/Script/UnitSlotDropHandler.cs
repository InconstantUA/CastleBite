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
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    City GetParentCity()
    {
        // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Left/Right/Wide]Panel-(this)UnitSlot
        return transform.parent.parent.parent.parent.parent.GetComponent<City>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // observations
        // 
        // 
        // disable drag state
        GetParentCity().SetActiveState(City.CityViewActiveState.ActiveUnitDrag, false);
        // drop unit if there is no other unit already present
        if (!unit)
        {
            PartyPanel.PanelMode panelMode = UnitDragHandler.unitBeingDragged.transform.parent.parent.parent.parent.GetComponent<PartyPanel>().GetPanelMode();
            if (PartyPanel.PanelMode.Garnizon == panelMode)
            {
                // enable hire unit button in the original parent cell and bring it to the front
                // and it is not wide
                // todo: add not wide condition check
                UnitDragHandler.unitBeingDragged.transform.parent.parent.Find("HireUnitPnlBtn").gameObject.SetActive(true);
                UnitDragHandler.unitBeingDragged.transform.parent.parent.Find("HireUnitPnlBtn").SetAsLastSibling();
            }
            // change parent of the dragged unit
            Debug.Log("Drop unit from " + UnitDragHandler.unitBeingDraggedParentTr.parent.gameObject.name);
            UnitDragHandler.unitBeingDragged.transform.SetParent(transform);
            // reset position to 0/0/0/0
            // [ left - bottom ]
            UnitDragHandler.unitBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            // [ right - top ]
            UnitDragHandler.unitBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            // disable hire unit button in the destination (this) cell, if it is garnizon panel
            // structure 3PartyPanel-2Top/Middle/Bottom-1Left/Right/Wide-(this)UnitSlot
            panelMode = transform.parent.parent.parent.GetComponent<PartyPanel>().GetPanelMode();
            if (PartyPanel.PanelMode.Garnizon == panelMode)
            {
                // todo: add not wide condition check
                transform.parent.Find("HireUnitPnlBtn").gameObject.SetActive(false);
            }
            // trigger event
            // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
        }
    }
}
