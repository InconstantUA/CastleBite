﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEquipmentButton : MonoBehaviour {

    class OriginalState
    {
        public GameObject[] FocusPanelsLinkedObject = new GameObject[2];
        public bool HireHeroPanel;
        public bool HireCommonUnitButtons;
    }

    OriginalState originalState = new OriginalState();

    public void SetRequiredMenusActive(bool doActivate)
    {
        // Get city transform
        // structure: 6City-5HeroParty-4PartyPanel-3Row-2Cell-1UnitEquipmentControl-EquipmentButton(this)
        //City city = transform.parent.parent.parent.parent.parent.parent.GetComponent<City>();
        // Get city screen
        EditPartyScreen editPartyScreen = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true);
        // verify if City screen is active = we are in a city
        if (editPartyScreen)
        {
            //City city = cityScreen.City;
            // do actions for each HeroParty
            foreach (HeroPartyUI heroPartyUI in transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<HeroPartyUI>(true))
            {
                // verify if heroPartyUI has linked Party
                if (heroPartyUI.LHeroParty != null)
                {
                    // activate/deactivate party panel
                    heroPartyUI.GetComponentInChildren<PartyPanel>(true).gameObject.SetActive(doActivate);
                }
            }
            // verify if we are in city or hero party edit mode
            if (editPartyScreen.LCity != null)
            {
                // activate/deactivate city control butons
                transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<CityHealButton>(true)[0].gameObject.SetActive(doActivate);
                transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<CityResurectButton>(true)[0].gameObject.SetActive(doActivate);
            }
            transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<CityDismissButton>(true)[0].gameObject.SetActive(doActivate);
            // activate/deactivate ReturnBtn
            transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<CityBackButton>(true)[0].gameObject.SetActive(doActivate);
            // activate/deactivate Unit Equipment back button
            transform.root.Find("MiscUI/BottomControlPanel/RightControls/HeroEquipmentBackButton").gameObject.SetActive(!doActivate);
            // activate/deactivate focus panels
            FocusPanel[] focusPanels = transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<FocusPanel>(true);
            for (int i = 0; i < focusPanels.Length; i++)
            {
                // verify if focus panel is about to be disabled
                if (false == doActivate)
                {
                    // save original linked object of each focus panel
                    originalState.FocusPanelsLinkedObject[i] = focusPanels[i].focusedObject;
                    // disable focus panel
                    focusPanels[i].gameObject.SetActive(doActivate);
                }
                else
                {
                    // verify if there was linked game object to focus panel
                    if (originalState.FocusPanelsLinkedObject[i] != null)
                    {
                        // restore original state of the focus panel (before disable)
                        focusPanels[i].focusedObject = originalState.FocusPanelsLinkedObject[i];
                        focusPanels[i].gameObject.SetActive(true);
                    }
                }
            }
            // activate/deactivate HireHeroPanel
            if (false == doActivate)
            {
                originalState.HireHeroPanel = transform.root.GetComponentInChildren<UIManager>().transform.Find("HireHeroPanel").gameObject.activeSelf;
                transform.root.GetComponentInChildren<UIManager>().transform.Find("HireHeroPanel").gameObject.SetActive(doActivate);
            }
            else
            {
                transform.root.GetComponentInChildren<UIManager>().transform.Find("HireHeroPanel").gameObject.SetActive(originalState.HireHeroPanel);
            }
            // activate/deactivate HireCommonUnitButtons
            if (false == doActivate)
            {
                originalState.HireCommonUnitButtons = transform.root.GetComponentInChildren<UIManager>().transform.Find("HireCommonUnitButtons").gameObject.activeSelf;
                transform.root.GetComponentInChildren<UIManager>().transform.Find("HireCommonUnitButtons").gameObject.SetActive(doActivate);
            }
            else
            {
                transform.root.GetComponentInChildren<UIManager>().transform.Find("HireCommonUnitButtons").gameObject.SetActive(originalState.HireCommonUnitButtons);
            }
            // Activate/deactivate unit info panel
            UnitInfoPanel unitInfoPanel = transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>();
            if (!doActivate)
            {
                // activate unit info panel
                // get party unit, structure: 2UnitCanvas(Clone)-1UnitEquipmentControl-EquipmentButton(this)
                PartyUnit partyUnit = transform.parent.parent.GetComponent<PartyUnitUI>().LPartyUnit;
                // activate unit info panel
                unitInfoPanel.ActivateAdvance(partyUnit, UnitInfoPanel.Align.Right, false, UnitInfoPanel.ContentMode.Short);
            }
            else
            {
                // deactivate unit info panel
                unitInfoPanel.gameObject.SetActive(false);
            }
            // Disable/Enable unit info panel background
            //unitInfoPanel.transform.Find("Background").gameObject.SetActive(doActivate);
        }
    }

    public void ShowUnitEquipmentMenu()
    {
        Debug.Log("Show unit equipment menu");
        // activate HeroEquipment menu
        transform.root.Find("MiscUI/HeroEquipment").GetComponent<HeroEquipment>().ActivateAdvance(this);
    }
}
