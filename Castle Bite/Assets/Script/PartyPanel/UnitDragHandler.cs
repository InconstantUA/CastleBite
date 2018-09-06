using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject unitBeingDraggedUI;
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

    EditPartyScreen GetCityScreen()
    {
        // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-UnitSlot-(this)UnitCanvas
        //return transform.parent.parent.parent.parent.parent.parent.GetComponent<City>();
        // structure: 5MiscUI-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-UnitSlot-(this)UnitCanvas
        //             MiscUI-EditPartyScreen(link to City)
        return transform.parent.parent.parent.parent.parent.parent.GetComponentInChildren<EditPartyScreen>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("OnBeginDrag");
        if (Input.GetMouseButton(0))
        {
            // Debug.Log("OnBeginDrag: left mouse");
            // on left mouse drag
            // initialize required variables
            unitBeingDraggedUI = gameObject;
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
            GetCityScreen().SetActiveState(EditPartyScreenActiveState.ActiveUnitDrag, true);
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
        // verify if user clicked left or right mouse button
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse up
            // disable drag state
            GetCityScreen().SetActiveState(EditPartyScreenActiveState.ActiveUnitDrag, false);
            // verify if user has activated unit info panel by right click and then moved mouse, which has triggered drag function
            // Get unit info panel
            GameObject unitInfoPanel = transform.root.Find("MiscUI/UnitInfoPanel").gameObject;
            // verify if unit infor panel is active
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
                unitBeingDraggedUI = null;
                // enable blocksRaycasts
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                // reset position to original if parent has not changed
                if (transform.parent == startParent)
                {
                    transform.position = startPosition;
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse up
        }
    }

}