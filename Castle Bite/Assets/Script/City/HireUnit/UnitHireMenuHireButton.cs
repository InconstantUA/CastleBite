using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class UnitHireMenuHireButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
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
        Debug.Log("OnPointerEnter");
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
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
            Debug.Log("SetHighlightedStatus " + btn.name + " button");
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
        Debug.Log("SetPressedStatus " + btn.name + " button");
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
        Debug.Log("SetNormalStatus " + btn.name + " button");
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
        Debug.Log("DimmAllOtherMenus");
    }

    #region OnClick

    void ActOnClick()
    {
        // Act based on the leader type
        //  Find selected toggle and get attached to it unit template
        //  Hierarchy HirePartyLeader-Panel-Controls-(this)HireBtn
        Transform hirePartyLeaderPanel = transform.parent.parent;
        Toggle[] toggles = hirePartyLeaderPanel.Find("UnitsGroup").GetComponentsInChildren<Toggle>();
        PartyUnit selectedUnit = null;
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                selectedUnit = toggle.GetComponent<UnitHirePanel>().GetUnitToHire();
            }
        }
        //  Get attached leader type
        //  Get conditions based on the hero type
        //  I instantiate hero types templates in Game->Templates->PartyLeaderTmplts
        int requiredGold = selectedUnit.GetCost();
        // Verify if conditions are met
        //  Verify if player has enough gold
        PlayerObj player = transform.root.Find("PlayerObj").gameObject.GetComponent<PlayerObj>();
        if (player.GetTotalGold() >= requiredGold)
        {
            // take gold from player
            player.SetTotalGold(player.GetTotalGold() - requiredGold);
            // create instance of the party leader and place it in to the party object
            Party partyTemplate = transform.root.Find("Templates").Find("Obj").Find("Party").GetComponent<Party>();
            Transform playerParties = transform.root.Find("PlayerParties");
            Party newParty = Instantiate(partyTemplate, playerParties);
            PartyUnit newPartyUnit = Instantiate(selectedUnit, newParty.transform);
            newParty.party[0] = newPartyUnit;
            // create and update Hero Party panel in UI, parent it to Game UI
            Transform cityTr = transform.parent.parent.parent.parent;
            GameObject heroPartyPanelTemplate = transform.root.Find("Templates").Find("UI").Find("HeroParty").gameObject;
            GameObject newPartyUIPanel = Instantiate(heroPartyPanelTemplate, cityTr);
            //  activate new party UI panel
            newPartyUIPanel.SetActive(true);
            //  populate middle right panel with information from the highered leader
            Transform middeRightUnitPanel = newPartyUIPanel.transform.Find("PartyPanel").Find("Middle").Find("Right");
            middeRightUnitPanel.Find("HPPanel").Find("HPcurr").GetComponent<Text>().text = newPartyUnit.GetHealthCurr().ToString();
            middeRightUnitPanel.Find("HPPanel").Find("HPmax").GetComponent<Text>().text = newPartyUnit.GetHealthMax().ToString();
            middeRightUnitPanel.Find("UnitCanvas").Find("Name").GetComponent<Text>().text = newPartyUnit.GetGivenName().ToString() + "\r\n" + newPartyUnit.GetUnitName().ToString();
            //  activate hero HeroEquipmentBtn
            cityTr.Find("HeroEquipmentBtn").gameObject.SetActive(true);
            // fill in city's left focus with information from the hero
            //  first deactivate NoPartyInfo and activate FocusedName and BriefInfo
            cityTr.Find("LeftFocus").Find("NoPartyInfo").gameObject.SetActive(false);
            cityTr.Find("LeftFocus").Find("FocusedName").gameObject.SetActive(true);
            cityTr.Find("LeftFocus").Find("BriefInfo").gameObject.SetActive(true);
            //  populate with info from hero
            cityTr.Find("LeftFocus").Find("FocusedName").GetComponent<Text>().text = newPartyUnit.GetGivenName();
            cityTr.Find("LeftFocus").Find("BriefInfo").Find("LevelValue").GetComponent<Text>().text = newPartyUnit.GetLevel().ToString();
            cityTr.Find("LeftFocus").Find("BriefInfo").Find("LeadershipValue").GetComponent<Text>().text = newPartyUnit.GetLeadership().ToString();
            // deactivate HirePartyLeader menu 
            // Structure Cities-[city]-HirePartyLeader-Panel-Controls-thisButton
            btn.transform.parent.parent.parent.gameObject.SetActive(false);
            // deactivate Hire Hero pannel-button
            cityTr.Find("HireHeroPanelBtn").gameObject.SetActive(false);
        }
        else
        {
            // display message that is not enough gold
            Debug.Log("need more gold");
            GameObject notificationPopup = btn.transform.root.Find("MiscUI").Find("NotificationPopUp").gameObject;
            notificationPopup.SetActive(true);
            notificationPopup.GetComponentInChildren<Transform>().GetComponentInChildren<Text>().text = "More gold is needed to hire this party leader.";
        }
    }

    #endregion OnClick
}