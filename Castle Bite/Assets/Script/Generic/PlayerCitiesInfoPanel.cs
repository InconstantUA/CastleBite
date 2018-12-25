using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCitiesInfoPanel : MonoBehaviour
{
    [SerializeField]
    GameObject cityInfoTemplate;


    public void SetActive(bool doActivate)
    {
        if (doActivate)
        {
            // get active player faction
            Faction activePlayerFaction = TurnsManager.Instance.GetActivePlayer().Faction;
            // loop through all cities
            foreach (City city in ObjectsManager.Instance.GetComponentsInChildren<City>(true))
            {
                // verify if city belongs to the player's faction
                if (city.CityFaction == activePlayerFaction)
                {
                    // create city info 
                    // ..
                }
            }

        }
        else
        {
            // remove all cities
            // ..
        }
    }

}
