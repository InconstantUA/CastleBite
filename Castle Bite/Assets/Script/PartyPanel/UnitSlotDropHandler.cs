using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    bool isDropAllowed;
    string errorMessage;

    City GetParentCity()
    {
        // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-(this)UnitSlot
        return transform.parent.parent.parent.parent.parent.GetComponent<City>();
    }

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
        // swap HPPanel values
        string srcHPcurr = srcCellTr.Find("HPPanel/HPcurr").GetComponent<Text>().text;
        string dstHPcurr = dstCellTr.Find("HPPanel/HPcurr").GetComponent<Text>().text;
        srcCellTr.Find("HPPanel/HPcurr").GetComponent<Text>().text = dstHPcurr;
        dstCellTr.Find("HPPanel/HPcurr").GetComponent<Text>().text = srcHPcurr;
        string srcHPmax = srcCellTr.Find("HPPanel/HPmax").GetComponent<Text>().text;
        string dstHPmax = dstCellTr.Find("HPPanel/HPmax").GetComponent<Text>().text;
        srcCellTr.Find("HPPanel/HPmax").GetComponent<Text>().text = dstHPmax;
        dstCellTr.Find("HPPanel/HPmax").GetComponent<Text>().text = srcHPmax;
        // swap UnitCanvas
        UnitDragHandler srcUnitCanvas = srcCellTr.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
        UnitDragHandler dstUnitCanvas = dstCellTr.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
        // verify is src unit canvas exist, it may not exist on double unit swap
        if (srcUnitCanvas)
        {
            srcUnitCanvas.transform.SetParent(dstCellTr.Find("UnitSlot"));
            ResetPositionToZero(srcUnitCanvas.transform);
        }
        //  verfy that unit canvas is present, dst cell may be free
        if (dstUnitCanvas)
        {
            dstUnitCanvas.transform.SetParent(srcCellTr.Find("UnitSlot"));
            ResetPositionToZero(dstUnitCanvas.transform);
        }
        //// swap HireUnitPnlBtn state
        //bool srcHireUnitPnlBtn = srcCellTr.Find("HireUnitPnlBtn").gameObject.activeSelf;
        //bool dstHireUnitPnlBtn = dstCellTr.Find("HireUnitPnlBtn").gameObject.activeSelf;
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
        Transform srcL = srcPanelTr.Find("Front");
        Transform srcR = srcPanelTr.Find("Back");
        Transform srcW = srcPanelTr.Find("Wide");
        Transform dstL = dstPanelTr.Find("Front");
        Transform dstR = dstPanelTr.Find("Back");
        Transform dstW = dstPanelTr.Find("Wide");
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
        // act based on the previously set by OnDrag condition
        if (isDropAllowed)
        {
            // drop is allowed
            // act based on then draggable unit size
            // get actual unit, structure Cell-UnitCanvas(dragged)->Unit
            PartyUnit draggedUnit = UnitDragHandler.unitBeingDragged.GetComponentInChildren<PartyUnit>();
            Transform srcCellTr = UnitDragHandler.unitBeingDragged.transform.parent.parent;
            Transform dstCellTr = transform.parent;
            PartyUnit.UnitSize dstCellSize = transform.parent.GetComponent<UnitCell>().GetCellSize();
            if (draggedUnit.GetUnitSize() == PartyUnit.UnitSize.Single)
            {
                // single unit
                // possible states
                // src  dst                                 result
                // 1    free or occupied by single unit     swap single cells
                // 1    occupied by double                  swap cells in horizontal panels
                // act based on destination cell size
                if (PartyUnit.UnitSize.Single == dstCellSize)
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
                if (PartyUnit.UnitSize.Single == dstCellSize)
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
            //// drop unit if there is no other unit already present
            //if (!unit)
            //{
            //    PartyPanel.PanelMode panelMode = UnitDragHandler.unitBeingDragged.transform.parent.parent.parent.parent.GetComponent<PartyPanel>().GetPanelMode();
            //    if (PartyPanel.PanelMode.Garnizon == panelMode)
            //    {
            //        // enable hire unit button in the original parent cell and bring it to the front
            //        // and it is not wide
            //        // todo: add not wide condition check
            //        UnitDragHandler.unitBeingDragged.transform.parent.parent.Find("HireUnitPnlBtn").gameObject.SetActive(true);
            //        UnitDragHandler.unitBeingDragged.transform.parent.parent.Find("HireUnitPnlBtn").SetAsLastSibling();
            //    }
            //    // change parent of the dragged unit
            //    Debug.Log("Drop unit from " + UnitDragHandler.unitBeingDraggedParentTr.parent.gameObject.name);
            //    UnitDragHandler.unitBeingDragged.transform.SetParent(transform);
            //    // disable hire unit button in the destination (this) cell, if it is garnizon panel
            //    // structure 3PartyPanel-2Top/Middle/Bottom-1Front/Back/Wide-(this)UnitSlot
            //    panelMode = transform.parent.parent.parent.GetComponent<PartyPanel>().GetPanelMode();
            //    if (PartyPanel.PanelMode.Garnizon == panelMode)
            //    {
            //        // todo: add not wide condition check
            //        transform.parent.Find("HireUnitPnlBtn").gameObject.SetActive(false);
            //    }
            //    // trigger event
            //    // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
            //}
        }
        else
        {
            // drop is not allowed
            // display error message
            transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errorMessage);
        }
    }
}
