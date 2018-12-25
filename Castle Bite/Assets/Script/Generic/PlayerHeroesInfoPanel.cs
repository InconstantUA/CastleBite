using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeroesInfoPanel : MonoBehaviour
{
    [SerializeField]
    GameObject playerHeroInfoTemplate;
    [SerializeField]
    Transform playerHeroesListTransform;

    public void SetActive(bool doActivate)
    {
        if (doActivate)
        {
            // get active player faction
            Faction activePlayerFaction = TurnsManager.Instance.GetActivePlayer().Faction;
            // loop through all heroparties
            foreach (HeroParty heroParty in ObjectsManager.Instance.GetComponentsInChildren<HeroParty>(true))
            {
                // verify if hero party belongs to the player's faction and if it is not garnizon
                if ((heroParty.Faction == activePlayerFaction) && (heroParty.PartyMode == PartyMode.Party))
                {
                    // get leader
                    PartyUnit partyLeaderUnit = heroParty.GetPartyLeader();
                    // create and activate leader info
                    Instantiate(playerHeroInfoTemplate, playerHeroesListTransform).GetComponent<PlayerHeroInfo>().SetActive(partyLeaderUnit);
                }
            }
        }
        else
        {
            // remove all heroes infos
            foreach(PlayerHeroInfo playerHeroInfo in GetComponentsInChildren<PlayerHeroInfo>())
            {
                Destroy(playerHeroInfo.gameObject);
            }
        }
    }
}
