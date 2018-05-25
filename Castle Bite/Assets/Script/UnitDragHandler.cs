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

    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        unitBeingDragged = gameObject;
        unitBeingDraggedParentTr = transform.parent;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        // set parent panel to the top layer
        // hierarchy [City]-CityGarnizon-PartyPanel-[Top/Middle/Bottom]-[Left/Right/Wide]-UnitSlot-(this)UnitCanvas
        transform.parent.SetAsLastSibling(); // unit slot
        transform.parent.parent.SetAsLastSibling(); // left/right/wide panel
        transform.parent.parent.parent.SetAsLastSibling(); // Top/Middle/Bottom panel
        transform.parent.parent.parent.parent.SetAsLastSibling(); // PartyPanel
        transform.parent.parent.parent.parent.parent.SetAsLastSibling(); // CityGarnizon
    }
    #endregion
    #region IDragHandler implementation
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        // disable all add unit buttons to avoid unnecessary interraction
        PartyPanel partyPanel = transform.parent.parent.parent.parent.GetComponent<PartyPanel>();
        partyPanel.SetHireUnitPnlButtonActive(false);
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
        PartyPanel partyPanel = transform.parent.parent.parent.parent.GetComponent<PartyPanel>();
        partyPanel.SetHireUnitPnlButtonActive(true);
    }

    #endregion
}