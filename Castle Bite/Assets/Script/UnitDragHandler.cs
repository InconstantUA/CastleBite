using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject unitBeingDragged;
    public static Transform unitBeingDraggedParentTr;
    Vector3 startPosition;
    Transform startParent;

    void BringUnitToFront()
    {
        // set parent panel to the top layer
        // up to the city garnizon or hero party level
        // there is no need to set city to higher level
        // hierarchy [City]-[CityGarnizon/HeroParty]-PartyPanel-[Top/Middle/Bottom]-[Front/Back/Wide]-UnitSlot-(this)UnitCanvas
        transform.parent.SetAsLastSibling(); // unit slot
        transform.parent.parent.SetAsLastSibling(); // left/right/wide panel
        transform.parent.parent.parent.SetAsLastSibling(); // Top/Middle/Bottom panel
        transform.parent.parent.parent.parent.SetAsLastSibling(); // PartyPanel
        transform.parent.parent.parent.parent.parent.SetAsLastSibling(); // CityGarnizon/HeroParty
    }

    City GetParentCity()
    {
        // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-UnitSlot-(this)UnitCanvas
        return transform.parent.parent.parent.parent.parent.parent.GetComponent<City>();
    }

    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        // initialize required variables
        unitBeingDragged = gameObject;
        unitBeingDraggedParentTr = transform.parent;
        startPosition = transform.position;
        startParent = transform.parent;
        // Make sure that draggable unit do not block rays
        // which are goiong from the mouse to the underlying objects (unit drop slots)
        // so we can detect where we drop unit and what is below the mouse pointer
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        // make unit the top most, so when you drag it, it appears on top of other units
        BringUnitToFront();
        // enter city to drag state to highlight panels which can be drop targets
        // and do additional required gui adjustments
        GetParentCity().SetActiveState(City.CityViewActiveState.ActiveUnitDrag, true);
    }
    #endregion
    #region IDragHandler implementation
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    #endregion
    #region IEndDragHandler implementation
    public void OnEndDrag(PointerEventData eventData)
    {
        // clean up unit being dragged
        unitBeingDragged = null;
        // enable blocksRaycasts
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        // reset position to original if parent has not changed
        if (transform.parent == startParent)
        {
            transform.position = startPosition;
        }
        // activate hire unit buttons again, after it was disabled
        // this is should be done in City Garnizon panel
        PartyPanel garnizonPanel = GetParentCity().transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>();
        garnizonPanel.SetHireUnitPnlButtonActive(true);
    }

    #endregion
}