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

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("OnBeginDrag");
        if (Input.GetMouseButton(0))
        {
            // Debug.Log("OnBeginDrag: left mouse");
            // on left mouse drag
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
        else if (Input.GetMouseButton(1))
        {
            // Debug.Log("OnBeginDrag: right mouse");
            // on right mouse click
            // do nothing
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");
        if (Input.GetMouseButton(0))
        {
            // on left mouse drag
            // Debug.Log("OnDrag: left mouse");
            transform.position = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            // on right mouse click
            // Debug.Log("OnDrag: right mouse");
            // do nothing
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("OnEndDrag");
        // disable drag state
        GetParentCity().SetActiveState(City.CityViewActiveState.ActiveUnitDrag, false);
        // verify if user has activated unit info panel by right click and then moved mouse
        // which triggered drag function
        GameObject unitInfoPanel = transform.root.Find("MiscUI/UnitInfoPanel").gameObject;
        if (unitInfoPanel.activeSelf)
        {
            // unit info panel was active
            // Debug.LogWarning("OnEndDrag right mouse");
            // deactivate it
            unitInfoPanel.SetActive(false);
        }
        else
        {
            // unit info panel was not active and it most probably was normal left mouse drag operation
            //Debug.LogWarning("OnEndDrag left mouse");
            // on left mouse drag
            // clean up unit being dragged
            unitBeingDragged = null;
            // enable blocksRaycasts
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // reset position to original if parent has not changed
            if (transform.parent == startParent)
            {
                transform.position = startPosition;
            }
            // verify if we are in city edit mode and not in hero edit mode
            if (GetParentCity().transform.Find("CityGarnizon"))
            {
                // activate hire unit buttons again, after it was disabled
                // this is should be done in City Garnizon panel
                PartyPanel garnizonPanel = GetParentCity().transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>();
                garnizonPanel.SetHireUnitPnlButtonActive(true);
            }
        }
    }

}