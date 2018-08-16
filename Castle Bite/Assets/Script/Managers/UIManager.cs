using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public HeroParty GetHeroPartyByMode(PartyMode partyMode, bool includeInactive = true)
    {
        // Loop through hero parties untill we find the party in required mode
        foreach (HeroParty heroParty in transform.GetComponentsInChildren<HeroParty>(includeInactive))
        {
            // compare if hero party in in party (not in garnizon mode)
            if (heroParty.PartyMode == partyMode)
            {
                return heroParty;
            }
        }
        return null;
    }

    public GameObject GetActiveScreen()
    {
        // verify if CityScreen is active 
        if (GetComponentInChildren<CityScreen>(false))
        {
            return GetComponentInChildren<CityScreen>(false).gameObject;
        }
        else if (GetComponentInChildren<HeroEditScreen>(false))
        {
            return GetComponentInChildren<HeroEditScreen>(false).gameObject;
        }
        else
        {
            Debug.Log("No active screen");
            return null;
        }
    }
}
