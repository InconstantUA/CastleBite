﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public HeroPartyUI GetHeroPartyUIByMode(PartyMode partyMode, bool includeInactive = false)
    {
        // Loop through hero parties untill we find the party in required mode
        foreach (HeroPartyUI heroPartyUI in transform.GetComponentsInChildren<HeroPartyUI>(includeInactive))
        {
            // compare if hero party in in party (not in garnizon mode)
            if (heroPartyUI.LHeroParty.PartyMode == partyMode)
            {
                return heroPartyUI;
            }
        }
        return null;
    }

    public HeroParty GetHeroPartyByMode(PartyMode partyMode, bool includeInactive = false)
    {
        HeroPartyUI heroPartyUI = GetHeroPartyUIByMode(partyMode, includeInactive);
        if (heroPartyUI != null)
        {
            return heroPartyUI.LHeroParty;
        }
        else
        {
            return null;
        }
    }

    public FocusPanel GetFocusPanelByHeroParty(HeroParty heroParty)
    {
        foreach(FocusPanel focusPanel in GetComponentsInChildren<FocusPanel>())
        {
            // verify if there is focused object
            if (focusPanel.focusedObject)
                // verify if there is link party unit UI
                if (focusPanel.focusedObject.GetComponent<PartyUnitUI>())
                    // verify if party unit UI has linked party Unit
                    if (focusPanel.focusedObject.GetComponent<PartyUnitUI>().LPartyUnit)
                        // verify if HeroParty linked to focus panel is the same as we are searching for
                        if (focusPanel.focusedObject.GetComponent<PartyUnitUI>().LPartyUnit.GetComponentInParent<HeroParty>().gameObject.GetInstanceID() == heroParty.gameObject.GetInstanceID())
                        {
                            return focusPanel;
                        }
        }
        return null;
    }

    public FocusPanel GetFocusPanelByCity(City city)
    {
        foreach (FocusPanel focusPanel in GetComponentsInChildren<FocusPanel>())
        {
            // verify if there is a link to focused object
            if (focusPanel.focusedObject)
            {
                // verify if City is in focused game object
                if (focusPanel.focusedObject.GetComponent<City>())
                {
                    // verify if City linked to focus panel is the same as we are searching for
                    if (focusPanel.focusedObject.GetComponent<City>().gameObject.GetInstanceID() == city.gameObject.GetInstanceID())
                    {
                        return focusPanel;
                    }
                }
            }
        }
        return null;
    }

}
