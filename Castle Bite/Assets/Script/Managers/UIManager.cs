using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public HeroParty GetHeroPartyByMode(PartyMode partyMode)
    {
        // Loop through hero parties untill we find the party in required mode
        foreach (HeroParty heroParty in transform.GetComponentsInChildren<HeroParty>(true))
        {
            // compare if hero party in in party (not in garnizon mode)
            if (heroParty.PartyMode == partyMode)
            {
                return heroParty;
            }
        }
        return null;
    }
}
