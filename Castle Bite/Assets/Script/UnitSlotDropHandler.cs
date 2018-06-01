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
        // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Left/Right/Wide]Panel-(this)UnitSlot
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

    void SwapSingleCells(Transform srcCellTr, Transform dstCellTr)
    {
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
        srcUnitCanvas.transform.SetParent(transform);
        ResetPositionToZero(srcUnitCanvas.transform);
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

    public void OnDrop(PointerEventData eventData)
    {
        // disable drag state
        GetParentCity().SetActiveState(City.CityViewActiveState.ActiveUnitDrag, false);
        // act based on the previously set by OnDrag condition
        if (isDropAllowed)
        {
            // drop is allowed
            // act based on then draggable unit size
            // get actual unit, structure Cell-UnitCanvas(dragged)->Unit
            PartyUnit draggedUnit = UnitDragHandler.unitBeingDragged.GetComponentInChildren<PartyUnit>();
            if (draggedUnit.GetUnitSize() == PartyUnit.UnitSize.Single)
            {
                // single unit
                // possible states
                // src  dst                                 result
                // 1    free or occupied by single unit     swap single cells
                // 1    occupied by double                  swap cells in horizontal panels
                // act based on destination cell size
                PartyUnit.UnitSize dstCellSize = transform.parent.GetComponent<UnitCell>().GetCellSize();
                if (PartyUnit.UnitSize.Single == dstCellSize)
                {
                    // swap single cells
                    Transform srcCellTr = UnitDragHandler.unitBeingDragged.transform.parent.parent;
                    Transform dstCellTr = transform.parent;
                    SwapSingleCells(srcCellTr, dstCellTr);
                }
                else
                {
                    // swap cells in horizontal panels
                }
            }
            else
            {
                // double unit
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
            //    // structure 3PartyPanel-2Top/Middle/Bottom-1Left/Right/Wide-(this)UnitSlot
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
