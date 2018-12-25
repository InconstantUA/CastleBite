using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCitiesInfoPanel : MonoBehaviour
{
    [SerializeField]
    GameObject playerCityInfoTemplate;
    [SerializeField]
    Transform playerCitiesListTransform;


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
                    // create and activate city info 
                    Instantiate(playerCityInfoTemplate, playerCitiesListTransform).GetComponent<PlayerCityInfo>().SetActive(city);
                }
            }

        }
        else
        {
            // remove all cities infos
            foreach (PlayerCityInfo playerCityInfo in GetComponentsInChildren<PlayerCityInfo>())
            {
                Destroy(playerCityInfo.gameObject);
            }
        }
    }

}
