using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class HireUnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Text txt;
    Button btn;
    Color tmpColor;

    void Start()
    {
        // init text object
        txt = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        btn = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        // enable mouse on its move, if it was disabled before by keyboard activity
        if (((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0)) & (!Cursor.visible))
        {
            Cursor.visible = true;
            // highlight button if it was highlighted before
            //if (CompareColors(btn.colors.highlightedColor, txt.color))
            //{
            //    DimmAllOtherMenus();
            //    SetHighlightedStatus();
            //}
            // Highlight button, if needed by triggering on point enter
            OnPointerEnter(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        // keep state On
        ActOnClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // return to previous toggle state
        SetNormalStatus();
    }

    bool CompareColors(Color a, Color b)
    {
        bool result = false;
        if (((int)(a.r * 1000) == (int)(b.r * 1000)) || ((int)(a.g * 1000) == (int)(b.g * 1000)) || ((int)(a.b * 1000) == (int)(b.b * 1000)))
        {
            result = true;
        }
        return result;
    }

    void SetHighlightedStatus()
    {
        // avoid double job
        if (!CompareColors(btn.colors.highlightedColor, txt.color))
        {
            // change to highlighted color
            if (btn.interactable)
            {
                tmpColor = btn.colors.highlightedColor;
            }
            else
            {
                tmpColor = btn.colors.disabledColor;
            }
            tmpColor.a = 1;
            txt.color = tmpColor;
            // Debug.Log("SetHighlightedStatus " + btn.name + " button");
        }
    }

    void SetPressedStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.pressedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        // Debug.Log("SetPressedStatus " + btn.name + " button");
    }

    void SetNormalStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.normalColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        // Debug.Log("SetNormalStatus " + btn.name + " button");
    }

    void DimmAllOtherMenus()
    {
        // to make it easier dimm just all menus
        GameObject[] allControls = GameObject.FindGameObjectsWithTag("HighlightableControlsHirePartyLeader");
        foreach (GameObject control in allControls)
        {
            Text tmpTxt = control.GetComponentInChildren<Text>();
            Button tmpBtn = control.GetComponentInChildren<Button>();
            tmpTxt.color = tmpBtn.colors.normalColor;
        }
        // Debug.Log("DimmAllOtherMenus");
    }

    #region OnClick

    PartyUnit GetSelectedUnit()
    {
        //  Find selected toggle and get attached to it unit template
        //  Hierarchy HirePartyLeader-Panel-Controls-(this)HireBtn
        Toggle[] toggles = transform.parent.parent.GetComponentInChildren<ToggleGroup>().GetComponentsInChildren<Toggle>();
        PartyUnit selectedUnit = null;
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                selectedUnit = toggle.GetComponent<UnitHirePanel>().GetUnitToHire();
            }
        }
        return selectedUnit;
    }

    bool VerifySingleUnitHire(PartyUnit selectedUnit)
    {
        bool result = true;
        // this is actually not required, because
        // if number of units in city reaches maximum, 
        // then hire unit button is disabled
        return result;
    }

    bool VerifyDoubleUnitHire(PartyUnit selectedUnit)
    {
        bool result = false;
        Transform cityTr = transform.parent.parent.parent.parent;
        PartyPanel partyPanel = cityTr.Find("CityGarnizon/PartyPanel").GetComponent<PartyPanel>();
        // if all checks passed, then verify result is success
        if(partyPanel.VerifyCityCapacityOverflowOnDoubleUnitHire())
        {
            // verify if player is highring double unit near occupied single unit cell
            // this is not possible and will cause double unit to be displayed on top of single unit
            // we should avoid this
            HireUnitGeneric hireUnitPanel = transform.parent.parent.parent.GetComponent<HireUnitGeneric>();
            Transform callerCell = hireUnitPanel.GetcallerCell();
            if (partyPanel.VerifyDoubleHireNearOccupiedSingleCell(callerCell))
            {
                result = true;
            }
        }
        return result;
    }

    bool VerifyUnitHire(PartyUnit selectedUnit)
    {
        bool result = true;
        if (selectedUnit.GetUnitSize() == PartyUnit.UnitSize.Single)
        {
            result = VerifySingleUnitHire(selectedUnit);
        }
        else
        {
            result = VerifyDoubleUnitHire(selectedUnit);
        }
        return result;
    }

    void ActOnClick()
    {
        // First get input from parent:
        HireUnitGeneric hireUnitPanel = transform.parent.parent.parent.GetComponent<HireUnitGeneric>();
        bool isHigheredUnitPartyLeader = hireUnitPanel.GetisHigheredUnitPartyLeader();
        Transform callerCell = hireUnitPanel.GetcallerCell();
        GameObject callerObjectToDisableOnHire = hireUnitPanel.GetcallerObjectToDisableOnHire();
        // Act based on the leader type
        PartyUnit selectedUnit = GetSelectedUnit();
        //  Get attached leader type
        //  Get conditions based on the hero type
        //  I instantiate hero types templates in Game->Templates->PartyLeaderTmplts
        int requiredGold = selectedUnit.GetCost();
        //  Verify if player has enough gold
        PlayerObj player = transform.root.Find("PlayerObj").gameObject.GetComponent<PlayerObj>();
        if (player.GetTotalGold() >= requiredGold)
        {
            // Verify if other conditions are met
            if (VerifyUnitHire(selectedUnit)) {
                // take gold from player
                player.SetTotalGold(player.GetTotalGold() - requiredGold);
                // Create, if required, parent transform for new Unit 
                // (this is needed iF new party is created, when leader is highered)
                Transform newUnitParentSlot = null;
                Transform cityTr = transform.parent.parent.parent.parent;
                if (isHigheredUnitPartyLeader)
                {
                    // create and update Hero Party panel in UI, parent it to city UI
                    GameObject heroPartyPanelTemplate = transform.root.Find("Templates").Find("UI").Find("HeroParty").gameObject;
                    GameObject newPartyUIPanel = Instantiate(heroPartyPanelTemplate, cityTr);
                    //  activate new party UI panel
                    newPartyUIPanel.SetActive(true);
                    //  set hero's equipment button to be part of city control panel ToggleGroup
                    //  this should be unset on hero leaving city
                    ToggleGroup toggleGroup = cityTr.Find("CtrlPnlCity").GetComponent<ToggleGroup>();
                    Toggle heroEquipmentToggle = newPartyUIPanel.transform.Find("HeroEquipmentBtn").GetComponent<Toggle>();
                    heroEquipmentToggle.group = toggleGroup;
                    //  set HeroEquipmentBtn Toggle within CityControlPanel, so it can dimm or deselect it when other Toggles in group are activated
                    //  this should be set to null on hero leaving or accessed outside of the city.
                    toggleGroup.GetComponent<CityControlPanel>().SetHeroEquipmentToggle(heroEquipmentToggle);
                    //  set middle right panel as hero's parent transform. Place it to the canvas, which later will be dragg and droppable
                    newUnitParentSlot = newPartyUIPanel.GetComponentInChildren<PartyPanel>().GetUnitSlotTr("Middle", "Right");
                }
                else
                {
                    PartyPanel partyPanel = callerCell.parent.parent.GetComponent<PartyPanel>();
                    newUnitParentSlot = partyPanel.GetUnitSlotTr(callerCell, selectedUnit);
                }
                //  create new instance of unity draggable canvas and set it as unit's parent
                GameObject unitCanvasTemplate = transform.root.Find("Templates").Find("UI").Find("UnitCanvas").gameObject;
                Transform newUnitParentTr = Instantiate(unitCanvasTemplate, newUnitParentSlot).transform;
                // enable it
                newUnitParentTr.gameObject.SetActive(true);
                // Create new unit and place it in parent transform
                PartyUnit newPartyUnit = Instantiate(selectedUnit, newUnitParentTr);
                // Update UI with information from new unit;
                // If this is for new leader highering, then we also should activate required UIs
                // and also fill in city's left focus panels
                if (isHigheredUnitPartyLeader)
                {
                    //  activate hero HeroEquipmentBtn
                    //   cityTr.Find("CtrlPnlCity/HeroEquipmentBtn").gameObject.SetActive(true);
                    // link party leader to the Left Focus panel
                    // so it can useit to fill in information
                    cityTr.Find("LeftFocus").GetComponent<FocusPanel>().focusedObject = newPartyUnit.gameObject;
                    // fill in city's left focus with information from the hero
                    // Focus panel wil automatically detect changes and update info
                    cityTr.Find("LeftFocus").GetComponent<FocusPanel>().OnChange();
                } else
                {
                    // fill in city's right focus with information from the hero
                    // Focus panel wil automatically detect changes and update info
                    cityTr.Find("RightFocus").GetComponent<FocusPanel>().OnChange();
                    // update party panel
                    cityTr.Find("CityGarnizon/PartyPanel").GetComponent<PartyPanel>().OnChange();
                }
                // deactivate new unit hire selection pannel with the list of units to hire
                // which is parent of this button
                transform.parent.parent.parent.GetComponent<HireUnitGeneric>().DeactivateAdv();
                // deactivate required menu (we set it in Unity UI)
                // we do it here and not on DeactivateAdv because DeactivateAdv is also used
                // just on close button and on close we do not need to deactivate caller
                // because nothing has changed
                callerObjectToDisableOnHire.SetActive(false);
            }
        }
        else
        {
            // display message that is not enough gold
            NotificationPopUp notificationPopup = btn.transform.root.Find("MiscUI").Find("NotificationPopUp").GetComponent<NotificationPopUp>();
            if (isHigheredUnitPartyLeader)
            {
                notificationPopup.DisplayMessage("More gold is needed to hire this party leader.");
            }
            else
            {
                notificationPopup.DisplayMessage("More gold is needed to hire this party member.");
            }
        }
    }

    #endregion OnClick
}