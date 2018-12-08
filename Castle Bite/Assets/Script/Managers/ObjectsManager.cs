using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class GameData : System.Object
{
    // Map (Scene)
    // ..
    // Turns: active player, turn number
    public TurnsData turnsData;
    // Players
    public PlayerData[] playersData;
    // Game data
    public MapData mapData;
    // Cities
    public CityData[] citiesData;
    // Parties with units
    public PartyData[] partiesData;
}

public class ObjectsManager : MonoBehaviour {
    public static ObjectsManager Instance { get; private set; }

    [SerializeField]
    GameObject gamePlayerTemplate;
    [SerializeField]
    GameObject cityTemplate;
    [SerializeField]
    GameObject cityOnMapTemplate;
    [SerializeField]
    GameObject heroPartyTemplate;
    [SerializeField]
    GameObject heroPartyOnMapTemplate;
    [SerializeField]
    GameObject inventoryItemTemplate;
    [SerializeField]
    GameObject inventoryItemOnMapTemplate;

    void Awake()
    {
        Instance = this;
    }

    public GameObject HeroPartyTemplate
    {
        get
        {
            return heroPartyTemplate;
        }
        // set via Unity Editor
    }

    public GameObject HeroPartyOnMapTemplate
    {
        get
        {
            return heroPartyOnMapTemplate;
        }
        // set via Unity Editor
    }

    public void CreatePlayer(PlayerData playerData)
    {
        // Get players root
        Transform gamePlayersRoot = transform.root.Find("GamePlayers");
        // instantiate new player
        GamePlayer newGamePlayer = Instantiate(gamePlayerTemplate, gamePlayersRoot).GetComponent<GamePlayer>();
        // Set player data
        newGamePlayer.PlayerData = playerData;
        // rename it
        newGamePlayer.gameObject.name = playerData.givenName + " " + playerData.faction;
    }

    public void RemovePlayer(GamePlayer gamePlayer)
    {
        Destroy(gamePlayer.gameObject);
    }

    void CreateCityOnMap(City city, CityData cityData)
    {
        // Create new party on map
        MapCity newCityOnMap = Instantiate(cityOnMapTemplate, MapManager.Instance.transform).GetComponent<MapCity>();
        // city to original position on map
        newCityOnMap.GetComponent<RectTransform>().offsetMin = new Vector2(cityData.cityMapPosition.offsetMinX, cityData.cityMapPosition.offsetMinY);
        newCityOnMap.GetComponent<RectTransform>().offsetMax = new Vector2(cityData.cityMapPosition.offsetMaxX, cityData.cityMapPosition.offsetMaxY);
        // create links between city on map and city
        newCityOnMap.LCity = city;
        city.LMapCity = newCityOnMap;
        // rename it
        newCityOnMap.gameObject.name = city.CityName;
        // activate city on map
        newCityOnMap.gameObject.SetActive(true);
    }

    public void CreateCity(CityData cityData)
    {
        Debug.Log("Creating " + cityData.cityName + " city");
        // get parent Transform
        Transform citiesParentTransform = transform.root.Find("Map/Cities");
        // create city from tempalte
        City newCity = Instantiate(cityTemplate, citiesParentTransform).GetComponent<City>();
        // set city data
        newCity.CityData = cityData;
        // rename city to look nicer in ui
        newCity.gameObject.name = newCity.CityName;
        // activate city
        newCity.gameObject.SetActive(true);
        // Create city on map representation
        CreateCityOnMap(newCity, cityData);
    }

    public void RemoveCity(City city)
    {
        Debug.Log("Removing " + city.CityName + " city");
        // verify if there is linked party on map
        if (city.LMapCity != null)
        {
            // destroy city on map
            Destroy(city.LMapCity.gameObject);
        }
        // destroy city
        Destroy(city.gameObject);
    }

    public void CreatePartyUnit(PartyUnitData partyUnitData, HeroParty heroParty)
    {
        // get unit template by unit type
        GameObject unitTemplate = transform.root.GetComponentInChildren<TemplatesManager>().GetPartyUnitTemplateByType(partyUnitData.unitType);
        if (unitTemplate != null)
        {
            Debug.Log("Creating unit of [" + partyUnitData.unitType + "] type from " + unitTemplate.name + " template");
            // create unit in HeroParty
            PartyUnit partyUnit = Instantiate(unitTemplate, heroParty.transform).GetComponent<PartyUnit>();
            // set party unit data
            partyUnit.PartyUnitData = partyUnitData;
            // create items for party unit
            foreach(InventoryItemData inventoryItemData in partyUnit.PartyUnitData.unitIventory)
            {
                CreateInventoryItem(inventoryItemData, partyUnit.transform);
            }
        }
        else
        {
            Debug.LogError("Could no find template for " + partyUnitData.unitType.ToString() + " unit type.");
        }
    }

    MapHero CreatePartyOnMap(HeroParty heroParty, PartyData partyData)
    {
        Debug.Log("Creating party on map representation");
        // Create new party on map UI
        MapHero newPartyOnMap = Instantiate(heroPartyOnMapTemplate, MapManager.Instance.transform).GetComponent<MapHero>();
        // place party to original position on map
        newPartyOnMap.GetComponent<RectTransform>().offsetMin = new Vector2(partyData.partyMapPosition.offsetMinX, partyData.partyMapPosition.offsetMinY);
        newPartyOnMap.GetComponent<RectTransform>().offsetMax = new Vector2(partyData.partyMapPosition.offsetMaxX, partyData.partyMapPosition.offsetMaxY);
        // create links between party on map and hero party
        newPartyOnMap.LHeroParty = heroParty;
        heroParty.LMapHero = newPartyOnMap;
        // send it backwards, because city UI should be on top
        newPartyOnMap.transform.SetAsFirstSibling();
        // rename it
        newPartyOnMap.gameObject.name = heroParty.GetPartyLeader().GivenName + " " + heroParty.GetPartyLeader().UnitName + " Party";
        // activate hero on map
        newPartyOnMap.gameObject.SetActive(true);
        // return result
        return newPartyOnMap;
    }

    City GetCityByID(int cityID)
    {
        // loop through all cites
        foreach(City city in transform.root.Find("Map/Cities").GetComponentsInChildren<City>())
        {
            // compare city id to searchable id
            if (cityID == city.CityID)
            {
                return city;
            }
        }
        Debug.LogError("Failed to find city by " + cityID.ToString() + " ID");
        return null;
    }

    public void CreateHeroParty(PartyData partyData)
    {
        // verify if there are units in party
        if (partyData.partyUnitsData.Length >= 1)
        {
            Debug.Log("Creating " + partyData.partyUnitsData[0].unitName + " party");
        } else
        {
            // empty can be only garnizon in non-capital city
            Debug.Log("Creating garnizon");
        }
        // define new hero party variable
        HeroParty newHeroParty;
        // define new hero party parent transform variable
        Transform newHeroPartyParentTransform;
        // create hero party from tempalte in required location: City or UI address
        // verify if party was in city
        if (partyData.linkedCityID != -1)
        {
            // get parent transform by city ID
            newHeroPartyParentTransform = GetCityByID(partyData.linkedCityID).transform;
        } else if (partyData.partyUIAddress != null)
        {
            // get parent transform by UI address
            newHeroPartyParentTransform = transform.root.Find(partyData.partyUIAddress);
        } else
        {
            Debug.LogError("Unknown condition. Cannot find hero party parent transform");
            newHeroPartyParentTransform = transform.root;
        }
        newHeroParty = Instantiate(heroPartyTemplate, newHeroPartyParentTransform).GetComponent<HeroParty>();
        // set hero party data
        newHeroParty.PartyData = partyData;
        // create units
        // Note: Party leader should be created before party representation on the map is set
        foreach (PartyUnitData partyUnitData in partyData.partyUnitsData)
        {
            CreatePartyUnit(partyUnitData, newHeroParty);
        }
        // verify if party is in garnizon mode
        if (partyData.partyMode == PartyMode.Garnizon)
        {
            // skip creating party on map represetnation
            // City garnizon does not have representation on map
        }
        else
        {
            // create party on map
            MapHero newMapHero = CreatePartyOnMap(newHeroParty, partyData);
            // verify if party has linked city
            if (partyData.linkedCityID != -1)
            {
                // get city by city ID
                MapCity mapCity = GetCityByID(partyData.linkedCityID).LMapCity;
                // create links between city on map and hero on map if needed (if they are on the same tile or if hero party is in city)
                mapCity.LMapHero = newMapHero;
                newMapHero.lMapCity = mapCity;
            }
        }
        // create items for party
        foreach (InventoryItemData inventoryItemData in newHeroParty.PartyData.partyInventory)
        {
            CreateInventoryItem(inventoryItemData, newHeroParty.transform);
        }
        // rename hero party
        if (partyData.partyUnitsData.Length >= 1 && newHeroParty.PartyMode != PartyMode.Garnizon)
        {
            // Name party by the leader name
            newHeroParty.gameObject.name = newHeroParty.GetPartyLeader().GivenName + " " + newHeroParty.GetPartyLeader().UnitName + " Party";
        }
        else
        {
            // Name party "Garnizon"
            newHeroParty.gameObject.name = "Garnizon";
        }
        // activate hero party
        newHeroParty.gameObject.SetActive(true);
    }

    public void RemoveHeroParty(HeroParty heroParty)
    {
        // verify if there is linked party on map
        if (heroParty.LMapHero != null)
        {
            // destroy hero party on map
            Destroy(heroParty.LMapHero.gameObject);
        }
        // destroy hero party
        // .. garnizon does not have leader, do check for this before enabling debug log
        // Debug.Log("Removing " + heroParty.GetPartyLeader().GivenName + " party");
        Destroy(heroParty.gameObject);
    }

    public void RemoveAllInventoryItemsOnMap()
    {
        // Loop through transforms 1 level below map (=belongs to the map)
        foreach (Transform childTransform in MapManager.Instance.transform)
        {
            // get map item (chest)
            MapItemsContainer mapItem = childTransform.GetComponent<MapItemsContainer>();
            // verify if it is not null
            if (mapItem != null)
            {
                // loop through all items linked to this map item (chest)
                foreach (InventoryItem inventoryItem in mapItem.LInventoryItems)
                {
                    // remove inv item
                    Destroy(inventoryItem.gameObject);
                }
                // remove map item
                Destroy(mapItem.gameObject);
            }
        }
    }

    public InventoryItem CreateInventoryItem(InventoryItemData inventoryItemData, Transform parentTransform)
    {
        // create new item on map
        InventoryItem inventoryItem = Instantiate(inventoryItemTemplate, parentTransform).GetComponent<InventoryItem>();
        // set item data
        inventoryItem.InventoryItemData = inventoryItemData;
        // rename it
        inventoryItem.gameObject.name = inventoryItem.ItemName;
        // return item
        return inventoryItem;
    }

    public MapItemsContainer CreateInventoryItemContainerOnMap(PositionOnMap positionOnMap)
    {
        // create new item on map
        MapItemsContainer mapItem = Instantiate(inventoryItemOnMapTemplate, MapManager.Instance.transform).GetComponent<MapItemsContainer>();
        // place item on map to original position on map
        mapItem.GetComponent<RectTransform>().offsetMin = new Vector2(positionOnMap.offsetMinX, positionOnMap.offsetMinY);
        mapItem.GetComponent<RectTransform>().offsetMax = new Vector2(positionOnMap.offsetMaxX, positionOnMap.offsetMaxY);
        // rename it
        mapItem.gameObject.name = "TreasureChest";
        // set it as first sibling so it does not apper in front of heroes, cities or labels
        mapItem.transform.SetAsFirstSibling();
        // activate it
        mapItem.gameObject.SetActive(true);
        // return it as result
        return mapItem;
    }
}
