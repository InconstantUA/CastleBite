using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitSlotDropHandler : MonoBehaviour, IDropHandler
{
    [SerializeField]
    UnitSize cellSize;
    bool isDropAllowed;
    string errorMessage;

    public UnitSize CellSize
    {
        get
        {
            return cellSize;
        }

        set
        {
            cellSize = value;
        }
    }

    //public GameObject unit
    //{
    //    get
    //    {
    //        if (transform.childCount > 0)
    //        {
    //            return transform.GetChild(0).gameObject;
    //        }
    //        return null;
    //    }
    //}


    public void SetOnDropAction(bool isDrpAlwd, string errMsg = "")
    {
        isDropAllowed = isDrpAlwd;
        errorMessage = errMsg;
    }

    void ResetPositionToZero(Transform objTransform)
    {
        // reset position to 0/0/0/0
        // [ left - bottom ]
        objTransform.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        // [ right - top ]
        objTransform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
    }

    void SwapTwoCellsContent(Transform srcCellTr, Transform dstCellTr)
    {
        // Debug.Log("Swap 2 cells: " + srcCellTr.name + " > " + dstCellTr.name);
        // swap all relevan cells content and states
        // swap UnitCanvas
        PartyUnitUI srcPartyUnitUI = srcCellTr.Find("UnitSlot").GetComponentInChildren<PartyUnitUI>();
        PartyUnitUI dstPartyUnitUI = dstCellTr.Find("UnitSlot").GetComponentInChildren<PartyUnitUI>();
        // Get source party
        // structure: 3HeroPartyUI-2PartyPanel-1Row-cell(dstCellTr)
        HeroParty srcHeroParty = srcCellTr.parent.GetComponentInParent<PartyPanel>().GetComponentInParent<HeroPartyUI>().LHeroParty;
        // Get destination party
        HeroParty dstHeroParty = dstCellTr.parent.GetComponentInParent<PartyPanel>().GetComponentInParent<HeroPartyUI>().LHeroParty;
        // verify is src unit canvas exist, it may not exist on double unit swap
        if (srcPartyUnitUI != null)
        {
            // Swap UI
            srcPartyUnitUI.transform.SetParent(dstCellTr.Find("UnitSlot"));
            ResetPositionToZero(srcPartyUnitUI.transform);
            // verify unit's party is about to change
            if (srcHeroParty.gameObject.GetInstanceID() != dstHeroParty.gameObject.GetInstanceID())
            {
                // Swap Unit between parties
                srcPartyUnitUI.LPartyUnit.transform.SetParent(dstHeroParty.transform);
            }
            // Change unit's address
            srcPartyUnitUI.LPartyUnit.UnitPPRow = srcPartyUnitUI.GetUnitRow().Row;
            srcPartyUnitUI.LPartyUnit.UnitPPCell = srcPartyUnitUI.GetUnitCell().GetComponent<PartyPanelCell>().Cell;
        }
        //  verfy that unit canvas is present, dst cell may be free
        if (dstPartyUnitUI != null)
        {
            dstPartyUnitUI.transform.SetParent(srcCellTr.Find("UnitSlot"));
            ResetPositionToZero(dstPartyUnitUI.transform);
            // verify unit's party is about to change
            if (srcHeroParty.gameObject.GetInstanceID() != dstHeroParty.gameObject.GetInstanceID())
            {
                // Swap Unit between parties
                dstPartyUnitUI.LPartyUnit.transform.SetParent(srcHeroParty.transform);
            }
            // Change unit's address
            dstPartyUnitUI.LPartyUnit.UnitPPRow = dstPartyUnitUI.GetUnitRow().Row;
            dstPartyUnitUI.LPartyUnit.UnitPPCell = dstPartyUnitUI.GetUnitCell().GetComponent<PartyPanelCell>().Cell;
        }
    }

    void SwapSingleWithDouble(Transform srcCellTr, Transform dstCellTr, bool direction)
    {
        Debug.Log("Swap horizontal panels");
        // L/l-left cell active/inactive, R/r-right, W-wide
        // activate/deactivate as below
        // LRw->lrW - direction true
        // lrW->LRw - direction false
        // swap all cells content
        // get horizontal panels for later use
        Transform srcPanelTr = srcCellTr.parent;
        Transform dstPanelTr = dstCellTr.parent;
        Transform srcL = srcPanelTr.Find(PartyPanel.Cell.Front.ToString());
        Transform srcR = srcPanelTr.Find(PartyPanel.Cell.Back.ToString());
        Transform srcW = srcPanelTr.Find(PartyPanel.Cell.Wide.ToString());
        Transform dstL = dstPanelTr.Find(PartyPanel.Cell.Front.ToString());
        Transform dstR = dstPanelTr.Find(PartyPanel.Cell.Back.ToString());
        Transform dstW = dstPanelTr.Find(PartyPanel.Cell.Wide.ToString());
        SwapTwoCellsContent(srcL, dstL);
        SwapTwoCellsContent(srcR, dstR);
        SwapTwoCellsContent(srcW, dstW);
        srcL.gameObject.SetActive(!direction);
        srcR.gameObject.SetActive(!direction);
        srcW.gameObject.SetActive(direction);
        dstL.gameObject.SetActive(direction);
        dstR.gameObject.SetActive(direction);
        dstW.gameObject.SetActive(!direction);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // verify if we are in city edit mode and not in hero edit mode
        EditPartyScreen cityScreen = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>();
        if (cityScreen != null)
        {
            // activate hire unit buttons again, after it was disabled
            cityScreen.SetHireUnitPnlButtonActive(true);
        }
        // verify if it is item being dragged
        if (InventoryItemDragHandler.itemBeingDragged != null)
        {
            // verify if there is a unit in the slot
            if (GetComponentInChildren<PartyUnitUI>() != null)
            {
                // try to apply item to the unit
                GetComponentInChildren<PartyUnitUI>().ActOnItemDrop(InventoryItemDragHandler.itemBeingDragged);
            }
            // reset cursor to normal
            CursorController.Instance.SetNormalCursor();
        }
        // verify if it is party unit being dragged
        else if (UnitDragHandler.unitBeingDraggedUI != null)
        {
            // act based on the previously set by OnDrag condition
            if (isDropAllowed)
            {
                // drop is allowed
                // act based on then draggable unit size
                // get actual unit, structure Cell-UnitCanvas(dragged->Unit link)
                PartyUnit draggedUnit = UnitDragHandler.unitBeingDraggedUI.GetComponent<PartyUnitUI>().LPartyUnit;
                Transform srcCellTr = UnitDragHandler.unitBeingDraggedUI.transform.parent.parent;
                Transform dstCellTr = transform.parent;
                if (draggedUnit.UnitSize == UnitSize.Single)
                {
                    // single unit
                    // possible states
                    // src  dst                                 result
                    // 1    free or occupied by single unit     swap single cells
                    // 1    occupied by double                  swap cells in horizontal panels
                    // act based on destination cell size
                    if (UnitSize.Single == cellSize)
                    {
                        // swap single cells
                        SwapTwoCellsContent(srcCellTr, dstCellTr);
                    }
                    else
                    {
                        // swap 2 single cells in src panel with double cell in dest panel
                        SwapSingleWithDouble(srcCellTr, dstCellTr, true);
                    }
                }
                else
                {
                    // double unit
                    if (UnitSize.Single == cellSize)
                    {
                        // swap single with double cells
                        SwapSingleWithDouble(srcCellTr, dstCellTr, false);
                    }
                    else
                    {
                        // swap 2 double cells
                        SwapTwoCellsContent(srcCellTr, dstCellTr);
                    }
                }
                // Instruct focus panels to be updated
                foreach (FocusPanel focusPanel in transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<FocusPanel>())
                {
                    focusPanel.OnChange(FocusPanel.ChangeType.UnitsPositionChange);
                }
                //// drop unit if there is no other unit already present
                //if (!unit)
                //{
                //    PartyPanelMode panelMode = UnitDragHandler.unitBeingDragged.transform.parent.parent.parent.parent.GetComponent<PartyPanel>().GetPanelMode();
                //    if (PartyPanelMode.Garnizon == panelMode)
                //    {
                //        // enable hire unit button in the original parent cell and bring it to the front
                //        // and it is not wide
                //        // todo: add not wide condition check
                //    }
                //    // change parent of the dragged unit
                //    Debug.Log("Drop unit from " + UnitDragHandler.unitBeingDraggedParentTr.parent.gameObject.name);
                //    UnitDragHandler.unitBeingDragged.transform.SetParent(transform);
                //    // disable hire unit button in the destination (this) cell, if it is garnizon panel
                //    // structure 3PartyPanel-2Top/Middle/Bottom-1Front/Back/Wide-(this)UnitSlot
                //    panelMode = transform.parent.parent.parent.GetComponent<PartyPanel>().GetPanelMode();
                //    if (PartyPanelMode.Garnizon == panelMode)
                //    {
                //        // todo: add not wide condition check
                //    }
                //    // trigger event
                //    // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
                //}
            }
            else
            {
                // drop is not allowed
                // display error message
                NotificationPopUp.Instance().DisplayMessage(errorMessage);
            }
        }
        else
        {
            Debug.LogWarning("Unknown object is being dragged");
        }
    }
}
