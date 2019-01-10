using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameData : System.Object
{
    // Save data
    public SaveData saveData;
    // Map = Chapter = World
    public ChapterData chapterData;
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
    Transform gamePlayersRoot;
    [SerializeField]
    GameObject gamePlayerTemplate;
    [SerializeField]
    GameObject cityTemplate;
    [SerializeField]
    GameObject cityOnMapTemplate;
    [SerializeField]
    GameObject cityOnMapLabelTemplate;
    [SerializeField]
    GameObject heroPartyTemplate;
    [SerializeField]
    GameObject heroPartyOnMapTemplate;
    [SerializeField]
    GameObject heroPartyOnMapLabelTemplate;
    [SerializeField]
    GameObject inventoryItemTemplate;
    [SerializeField]
    GameObject inventoryItemOnMapTemplate;
    [SerializeField]
    GameObject inventoryItemOnMapLabelTemplate;

    GameMap gameMap;

    void Awake()
    {
        // verify if instance is null = not set
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            // verify if instance not is equal this = this is the new instance spawned
            if (Instance.GetInstanceID() != this.GetInstanceID())
            {
                Debug.LogError("Another instance should be destoryed before spawning this instance.");
                // destroy previous instance
                Destroy(Instance.gameObject);
                // init new instance
                Instance = this;
            }
        }
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

    public GameObject HeroPartyOnMapLabelTemplate
    {
        get
        {
            return heroPartyOnMapLabelTemplate;
        }

        set
        {
            heroPartyOnMapLabelTemplate = value;
        }
    }

    public GameMap GameMap
    {
        get
        {
            return GetComponentInChildren<GameMap>();
        }
    }

    public void CreatePlayer(PlayerData playerData)
    {
        // instantiate new player
        GamePlayer newGamePlayer = Instantiate(gamePlayerTemplate, gamePlayersRoot).GetComponent<GamePlayer>();
        // Set player data
        newGamePlayer.PlayerData = playerData;
        // rename it
        newGamePlayer.gameObject.name = playerData.givenName + " " + playerData.faction;
    }

    public GamePlayer[] GetGamePlayers()
    {
        return gamePlayersRoot.GetComponentsInChildren<GamePlayer>(true);
    }

    public void RemovePlayer(GamePlayer gamePlayer)
    {
        Destroy(gamePlayer.gameObject);
    }

    void CreateCityOnMap(City city, CityData cityData)
    {
        // Create new party on map
        MapCity newCityOnMap = Instantiate(cityOnMapTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapCity>())).GetComponent<MapCity>();
        // city to original position on map
        //newCityOnMap.GetComponent<RectTransform>().offsetMin = new Vector2(cityData.cityMapPosition.offsetMinX, cityData.cityMapPosition.offsetMinY);
        //newCityOnMap.GetComponent<RectTransform>().offsetMax = new Vector2(cityData.cityMapPosition.offsetMaxX, cityData.cityMapPosition.offsetMaxY);
        // place it to original position on map based on the tile coordinates
        Vector3 cityPositionOffset = new Vector3(8f, 8f, 0);
        newCityOnMap.transform.position = MapManager.Instance.GetWorldPositionByCoordinates(cityData.cityMapCoordinates) - cityPositionOffset;
        // create links between city on map and city
        newCityOnMap.LCity = city;
        city.LMapCity = newCityOnMap;
        // rename it
        newCityOnMap.gameObject.name = city.CityName;
        // Create city label on map
        MapObjectLabel newCityOnMapLabel = Instantiate(cityOnMapLabelTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapObjectLabel>())).GetComponent<MapObjectLabel>();
        // Link city to the lable and label to the city
        newCityOnMap.GetComponent<MapObject>().Label = newCityOnMapLabel;
        newCityOnMapLabel.MapObject = newCityOnMap.GetComponent<MapObject>();
        // activate city label on map
        newCityOnMapLabel.gameObject.SetActive(true);
        // set color according to the player color preference
        newCityOnMap.SetColor(GetPlayerByFaction(city.CityFaction).PlayerColor);
        // activate city on map
        newCityOnMap.gameObject.SetActive(true);
    }

    public void CreateCity(CityData cityData)
    {
        Debug.Log("Creating " + cityData.cityName + " city");
        // get parent Transform
        Transform citiesParentTransform = transform.Find("Map/Cities");
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
            // destroy city label on map
            Destroy(city.LMapCity.GetComponent<MapObject>().Label.gameObject);
            // destroy city on map
            Destroy(city.LMapCity.gameObject);
        }
        // destroy city
        Destroy(city.gameObject);
    }

    public void CreatePartyUnit(PartyUnitData partyUnitData, HeroParty heroParty)
    {
        // get unit template by unit type
        GameObject unitTemplate = TemplatesManager.Instance.GetPartyUnitTemplateByType(partyUnitData.unitType);
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

    public GamePlayer GetPlayerByFaction(Faction faction)
    {
        foreach (GamePlayer gamePlayer in gamePlayersRoot.GetComponentsInChildren<GamePlayer>())
        {
            // verify if faction matches
            if (gamePlayer.Faction == faction)
            {
                return gamePlayer;
            }
        }
        // default
        return null;
    }

    MapHero CreatePartyOnMap(HeroParty heroParty, PartyData partyData)
    {
        Debug.Log("Creating party on map representation");
        // Create new party on map UI
        MapHero newPartyOnMap = Instantiate(heroPartyOnMapTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapHero>())).GetComponent<MapHero>();
        // place party to original position on map
        //newPartyOnMap.GetComponent<RectTransform>().offsetMin = new Vector2(partyData.partyMapPosition.offsetMinX, partyData.partyMapPosition.offsetMinY);
        //newPartyOnMap.GetComponent<RectTransform>().offsetMax = new Vector2(partyData.partyMapPosition.offsetMaxX, partyData.partyMapPosition.offsetMaxY);
        // place it to original position on map based on the tile coordinates
        newPartyOnMap.transform.position = MapManager.Instance.GetWorldPositionByCoordinates(partyData.partyMapCoordinates);
        // Create hero label on map
        MapObjectLabel newPartyOnMapLabel = Instantiate(heroPartyOnMapLabelTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapObjectLabel>())).GetComponent<MapObjectLabel>();
        // Link hero to the lable and label to the hero
        newPartyOnMap.GetComponent<MapObject>().Label = newPartyOnMapLabel;
        newPartyOnMapLabel.MapObject = newPartyOnMap.GetComponent<MapObject>();
        // create links between party on map and hero party
        newPartyOnMap.LHeroParty = heroParty;
        heroParty.LMapHero = newPartyOnMap;
        // rename it
        newPartyOnMap.gameObject.name = heroParty.GetPartyLeader().GivenName + " " + heroParty.GetPartyLeader().UnitName + " Party";
        // set color according to the player color preference
        newPartyOnMap.SetColor(GetPlayerByFaction(heroParty.Faction).PlayerColor);
        // activate hero on map
        newPartyOnMap.gameObject.SetActive(true);
        // activate hero label on map
        newPartyOnMapLabel.gameObject.SetActive(true);
        // return result
        return newPartyOnMap;
    }

    City GetCityByID(CityID cityID)
    {
        // loop through all cites
        foreach(City city in transform.Find("Map/Cities").GetComponentsInChildren<City>())
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
            Debug.Log("Creating " + partyData.partyUnitsData[0].unitType + " party");
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
        if (partyData.linkedCityID != CityID.None)
        {
            // get parent transform by city ID
            newHeroPartyParentTransform = GetCityByID(partyData.linkedCityID).transform;
        } else if (partyData.partyUIAddress != null)
        {
            Debug.Log("Creating party at " + partyData.partyUIAddress);
            // get parent transform by UI address
            newHeroPartyParentTransform = transform.Find(partyData.partyUIAddress);
        } else
        {
            Debug.LogError("Unknown condition. Cannot find hero party parent transform");
            newHeroPartyParentTransform = transform;
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
            if (partyData.linkedCityID != CityID.None)
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
            // destroy hero party on map label
            Destroy(heroParty.LMapHero.GetComponent<MapObject>().Label.gameObject);
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
        foreach (MapItemsContainer mapItem in MapManager.Instance.GetParentTransformByType(GetComponent<MapItemsContainer>()).GetComponentsInChildren<MapItemsContainer>(true))
        {
            // loop through all items linked to this map item (chest)
            foreach (InventoryItem inventoryItem in mapItem.LInventoryItems)
            {
                // remove inv item
                Destroy(inventoryItem.gameObject);
            }
            // remove map item label
            Destroy(mapItem.GetComponent<MapObject>().Label.gameObject);
            // remove map item
            Destroy(mapItem.gameObject);
        }
    }

    public void RemoveAllParties()
    {
        // Remove all parties
        foreach (HeroParty heroParty in GetComponentsInChildren<HeroParty>(true))
        {
            RemoveHeroParty(heroParty);
        }
        Debug.LogWarning("All parties removed");
    }

    public void RemoveAllCities()
    {
        // Remove cities
        foreach (City city in GetComponentsInChildren<City>(true))
        {
            RemoveCity(city);
        }
        Debug.Log("All cities removed");
    }

    public void RemoveAllPlayers()
    {
        // remove players
        foreach (GamePlayer gamePlayer in GetGamePlayers())
        {
            RemovePlayer(gamePlayer);
        }
        Debug.Log("All Players Removed");
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

    //public MapItemsContainer CreateInventoryItemContainerOnMap(PositionOnMap positionOnMap)
    //{
    //    // create new item on map
    //    MapItemsContainer mapItem = Instantiate(inventoryItemOnMapTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapItemsContainer>())).GetComponent<MapItemsContainer>();
    //    // place item on map to original position on map
    //    mapItem.GetComponent<RectTransform>().offsetMin = new Vector2(positionOnMap.offsetMinX, positionOnMap.offsetMinY);
    //    mapItem.GetComponent<RectTransform>().offsetMax = new Vector2(positionOnMap.offsetMaxX, positionOnMap.offsetMaxY);
    //    // rename it
    //    mapItem.gameObject.name = "TreasureChest";
    //    // set it as first sibling so it does not apper in front of heroes, cities or labels
    //    // mapItem.transform.SetAsFirstSibling();
    //    // Create item label on map
    //    MapObjectLabel mapItemLabel = Instantiate(inventoryItemOnMapLabelTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapObjectLabel>())).GetComponent<MapObjectLabel>();
    //    // Link item to the lable and label to the item
    //    mapItem.GetComponent<MapObject>().Label = mapItemLabel;
    //    mapItemLabel.MapObject = mapItem.GetComponent<MapObject>();
    //    // activate item label on map
    //    mapItemLabel.gameObject.SetActive(true);
    //    // activate it
    //    mapItem.gameObject.SetActive(true);
    //    // return it as result
    //    return mapItem;
    //}

    public MapItemsContainer CreateInventoryItemContainerOnMap(MapCoordinates mapCoordinates)
    {
        // create new item on map
        MapItemsContainer mapItem = Instantiate(inventoryItemOnMapTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapItemsContainer>())).GetComponent<MapItemsContainer>();
        // set position offset
        Vector3 itemPositionOffset = new Vector3(8f, 8f, 0);
        // place it to original position on map based on the tile coordinates
        mapItem.transform.position = MapManager.Instance.GetWorldPositionByCoordinates(mapCoordinates) - itemPositionOffset;
        // rename it
        mapItem.gameObject.name = "TreasureChest";
        // set it as first sibling so it does not apper in front of heroes, cities or labels
        // mapItem.transform.SetAsFirstSibling();
        // Create item label on map
        MapObjectLabel mapItemLabel = Instantiate(inventoryItemOnMapLabelTemplate, MapManager.Instance.GetParentTransformByType(GetComponent<MapObjectLabel>())).GetComponent<MapObjectLabel>();
        // Link item to the lable and label to the item
        mapItem.GetComponent<MapObject>().Label = mapItemLabel;
        mapItemLabel.MapObject = mapItem.GetComponent<MapObject>();
        // activate item label on map
        mapItemLabel.gameObject.SetActive(true);
        // activate it
        mapItem.gameObject.SetActive(true);
        // return it as result
        return mapItem;
    }

    public City GetStartingCityByFaction(Faction faction)
    {
        
        foreach (City city in GetComponentsInChildren<City>())
        {
            // verify if city faction match players faction and that it is starting city
            if ((city.CityFaction == faction) && (city.IsStarting == 1))
            {
                return city;
            }
        }
        return null;
    }
}
