using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapFocusPanel : MonoBehaviour {
    [SerializeField]
    TextButton previousButton;
    [SerializeField]
    TextButton nextButton;
    [SerializeField]
    Text focusedObjectNameText;
    [SerializeField]
    Text focusedObjectInfoText;
    GameObject focusedObject;
    List<MapHero> mapHeroesList;
    List<MapCity> mapCitiesList;
    int removedHeroIndex;

    void OnEnable()
    {
        // check if focused object is not defined yet
        if (focusedObject == null)
        {
            ReleaseFocus();
        }
    }

    void SetFocusedObjectName(string name)
    {
        focusedObjectNameText.text = name;
    }

    void SetHeroNameUI(MapHero mapHero)
    {
        SetFocusedObjectName(mapHero.LHeroParty.GetPartyLeader().GivenName + "\r\n<size=12>" + mapHero.LHeroParty.GetPartyLeader().UnitName + "</size>");
        // set camera focus
        Camera.main.GetComponent<CameraController>().SetCameraFocus(mapHero);
    }

    void ActivateNextAndPreviousControls()
    {
        previousButton.SetInteractable(true);
        nextButton.SetInteractable(true);
    }

    public void SetActive(MapHero mapHero)
    {
        // first do cleanup in case if previously were selected other object
        if (focusedObject != null)
        {
            // reset focus panel
            ReleaseFocus();
        }
        // Set focused object
        Debug.Log("Update map focus panel with " + mapHero.name);
        focusedObject = mapHero.gameObject;
        // save currently focused object id to game player it will be needed for game load and turns switch
        TurnsManager.Instance.GetActivePlayer().FocusedObjectID = focusedObject.gameObject.GetInstanceID();
        // transform.root.Find("Managers").GetComponent<TurnsManager>().GetActivePlayer().FocusedObjectID = focusedObject.gameObject.GetInstanceID();
        // Set hero name
        SetHeroNameUI(mapHero);
        // Set hero additinal info
        SetAdditionalInfo(mapHero);
        // verify if party is not on hold
        if (mapHero.LHeroParty.HoldPosition != true)
        {
            // Activate hold position control
            transform.Find("FocusedObjectControl/HoldPosition").gameObject.SetActive(true);
        }
        else
        {
            // Display that party is on hold information
            transform.Find("FocusedObjectControl/HoldingPositionInfo").gameObject.SetActive(true);
        }
        // Set list of map heroes
        mapHeroesList = new List<MapHero>();
        // Loop through all map heroes
        foreach (MapHero checkedHero in MapManager.Instance.GetComponentsInChildren<MapHero>())
        {
            // verify if mHero faction is the same as for active hero and that hero is not on hold
            if (mapHero.LHeroParty.Faction == checkedHero.LHeroParty.Faction && checkedHero.LHeroParty.HoldPosition != true )
            {
                mapHeroesList.Add(checkedHero);
            }
        }
        // verify if there is more than 1 hero of the same faction
        if (mapHeroesList.Count > 1)
        {
            // activate next and previous controls
            ActivateNextAndPreviousControls();
        }
    }

    int GetSelectedHeroIndex()
    {
        for (int i = 0; i < mapHeroesList.Count; i++)
        {
            if (mapHeroesList[i].gameObject.GetInstanceID() == focusedObject.GetInstanceID())
            {
                return i;
            }
        }
        // in this case most probably hero was removed from the list by set on hold, get the index of hero which was specially saved for this
        // verify if removed hero index is not higher than array size
        if (removedHeroIndex >= mapHeroesList.Count)
        {
            return mapHeroesList.Count;
        }
        else
        {
            return removedHeroIndex;
        }
        //Debug.LogError("Failed to find seleted hero in the list");
        //return 0;
    }

    int GetSelectedCityIndex()
    {
        for (int i = 0; i < mapCitiesList.Count; i++)
        {
            if (mapCitiesList[i].gameObject.GetInstanceID() == focusedObject.GetInstanceID())
            {
                return i;
            }
        }
        Debug.LogError("Failed to find seleted city in the list");
        return 0;
    }

    int GetNextIndex(int currentIndex, int listSize)
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex >= listSize)
        {
            return 0;
        }
        else
        {
            return nextIndex;
        }
    }

    int GetPreviousIndex(int currentIndex, int listSize)
    {
        int previousIndex = currentIndex - 1;
        if (previousIndex < 0)
        {
            return listSize - 1;
        }
        else
        {
            return previousIndex;
        }
    }

    //int GetPreviousIndex(MapHero focuseddMapHero, int currentIndex)
    //{
    //    // get previous index
    //    int previousIndex = GetPreviousIndex(currentIndex, mapHeroesList.Count);
    //    // verify if hero on this index is n
    //}

    void ChangeHeroFocus(MapHero nextHero)
    {
        focusedObject = nextHero.gameObject;
        // set name UI
        SetHeroNameUI(nextHero);
        // select hero on map
        MapManager.Instance.SetSelection(MapManager.Selection.PlayerHero, nextHero);
        // reset cursor to normal, because it is changed by MapManager
        CursorController.Instance.SetNormalCursor();
    }

    void ChangeCityFocus(MapCity nextCity)
    {
        focusedObject = nextCity.gameObject;
        // set name UI
        SetCityNameUI(nextCity);
        // select city on map
        MapManager.Instance.SetSelection(MapManager.Selection.PlayerCity, nextCity);
        // reset cursor to normal, because it is changed by MapManager
        CursorController.Instance.SetNormalCursor();
    }

    public void ShowNext()
    {
        // verify focused object type
        if (focusedObject.GetComponent<MapHero>())
        {
            Debug.Log("Show next hero");
            // get next hero from the list
            MapHero nextHero = mapHeroesList[GetNextIndex(GetSelectedHeroIndex(), mapHeroesList.Count)];
            // set focused object to new hero
            ChangeHeroFocus(nextHero);
        }
        else if (focusedObject.GetComponent<MapCity>())
        {
            Debug.Log("Show next city");
            // get next city from the list
            MapCity nextCity = mapCitiesList[GetNextIndex(GetSelectedCityIndex(), mapCitiesList.Count)];
            // set focused object to new city
            ChangeCityFocus(nextCity);
        }
        else
        {
            Debug.LogError("Unknwon focused object type");
        }
    }

    public void ShowPrevious()
    {
        // verify focused object type
        if (focusedObject.GetComponent<MapHero>())
        {
            Debug.Log("Show previous hero");
            // get previous hero from the list
            MapHero previousHero = mapHeroesList[GetPreviousIndex(GetSelectedHeroIndex(), GetSelectedHeroIndex())];
            // set focused object to new hero
            ChangeHeroFocus(previousHero);
        }
        else if (focusedObject.GetComponent<MapCity>())
        {
            Debug.Log("Show previous city");
            // get previous city from the list
            MapCity previousCity = mapCitiesList[GetPreviousIndex(GetSelectedCityIndex(), mapCitiesList.Count)];
            // set focused object to new city
            ChangeCityFocus(previousCity);
        }
        else
        {
            Debug.LogError("Unknwon focused object type");
        }
    }

    void SetCityNameUI(MapCity mapCity)
    {
        // initialize additional info string
        string additionalInfo = "";
        // verify cityType
        if (mapCity.LCity.CityType == CityType.Capital)
        {
            additionalInfo = mapCity.LCity.CityFaction.ToString() +  " Captial";
        } else
        {
            additionalInfo = "City";
        }
        // Set UI text
        SetFocusedObjectName(mapCity.LCity.CityName + "\r\n<size=12>" + additionalInfo + "</size>");
        // set camera focus
        Camera.main.GetComponent<CameraController>().SetCameraFocus(mapCity);
    }

    public void UpdateMovePointsInfo()
    {
        SetPartyLeaderMovePointsInfo(focusedObject.GetComponent<MapHero>());
    }

    void SetAdditionalInfo(MapCity mapCity)
    {
        // initialize additional info string
        string additionalInfo = "";
        // verify cityType
        if (mapCity.LCity.CityType == CityType.Capital)
        {
            additionalInfo = "";
        }
        else
        {
            additionalInfo = mapCity.LCity.CityLevelCurrent + " Level";
        }
        // Set UI text
        focusedObjectInfoText.text = additionalInfo;
    }

    void SetPartyLeaderMovePointsInfo(MapHero mapHero)
    {
        // get party leader
        PartyUnit partyLeader = mapHero.LHeroParty.GetPartyLeader();
        // Set UI text
        focusedObjectInfoText.text = partyLeader.MovePointsCurrent + "/" + partyLeader.GetEffectiveMaxMovePoints() + " Move Points";
    }

    void SetAdditionalInfo(MapHero mapHero)
    {
        SetPartyLeaderMovePointsInfo(mapHero);
    }

    MapCity GetFirstCityOnMap()
    {
        // Get active player
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        // get first city of the player
        foreach (MapCity mapCity in MapManager.Instance.GetComponentsInChildren<MapCity>(true))
        {
            // verify if city belongs to the active player
            if (mapCity.LCity.CityFaction == activePlayer.Faction)
            {
                // return map city
                return mapCity;
            }
        }
        // default
        return null;
    }

    public void FocusOnCity()
    {
        // get first city
        MapCity mapCity = GetFirstCityOnMap();
        // verify if mapCity is not null
        if (mapCity != null)
        {
            // activate focus on a map city
            SetActive(mapCity);
        }
    }

    MapHero GetFirstHeroOnMap()
    {
        // Get active player
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        // get first city of the player
        foreach (MapHero mapHero in MapManager.Instance.GetComponentsInChildren<MapHero>(true))
        {
            // verify if city belongs to the active player
            if (mapHero.LHeroParty.Faction == activePlayer.Faction)
            {
                // return map city
                return mapHero;
            }
        }
        // default
        return null;
    }

    public void FocusOnHero()
    {
        // get first city
        MapHero mapHero = GetFirstHeroOnMap();
        // verify if mapCity is not null
        if (mapHero != null)
        {
            // activate focus on a map city
            SetActive(mapHero);
        }
    }

    public void SetActive(MapCity mapCity)
    {
        // first do cleanup in case if previously were selected other object
        if (focusedObject != null)
        {
            ReleaseFocus();
        }
        // Get active player
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        // Set focused object
        Debug.Log("Update map focus panel with " + mapCity.name);
        focusedObject = mapCity.gameObject;
        // save currently focused object id to game player it will be needed for game load and turns switch
        activePlayer.FocusedObjectID = focusedObject.gameObject.GetInstanceID();
        // transform.root.Find("Managers").GetComponent<TurnsManager>().GetActivePlayer().FocusedObjectID = focusedObject.gameObject.GetInstanceID();
        // Set city name
        SetCityNameUI(mapCity);
        // Set additional info
        SetAdditionalInfo(mapCity);
        // Set list of map cities
        mapCitiesList = new List<MapCity>();
        // Loop through all map cities
        foreach (MapCity mCity in MapManager.Instance.GetComponentsInChildren<MapCity>())
        {
            // verify if faction is the same
            if (mapCity.LCity.CityFaction == mCity.LCity.CityFaction)
            {
                mapCitiesList.Add(mCity);
            }
        }
        // verify if there is more than 1 city of the same faction
        if (mapCitiesList.Count > 1)
        {
            // activate next and previous controls
            previousButton.SetInteractable(true);
            nextButton.SetInteractable(true);
        }
        // Activate enter city control
        transform.Find("FocusedObjectControl/EnterCity").gameObject.SetActive(true);
    }

    public void EnterCityOnMap()
    {
        Debug.Log("Enter city on map");
        MapManager.Instance.EnterCityOnMap(focusedObject.GetComponent<MapCity>());
    }

    public void HoldPosition()
    {
        Debug.Log("Hold position");
        // set hero party hold position flag active
        focusedObject.GetComponent<MapHero>().LHeroParty.HoldPosition = true;
        // get removed hero index for later use
        removedHeroIndex = GetSelectedHeroIndex();
        // remove hero party from the list
        mapHeroesList.Remove(focusedObject.GetComponent<MapHero>());
        // Deactivate hold position control
        transform.Find("FocusedObjectControl/HoldPosition").gameObject.SetActive(false);
        // Display that party is on hold information
        transform.Find("FocusedObjectControl/HoldingPositionInfo").gameObject.SetActive(true);
    }

    public void BreakHoldPosition(MapHero mapHero)
    {
        // stop holding position
        mapHero.LHeroParty.HoldPosition = false;
        // add hero party to the list
        mapHeroesList.Add(mapHero);
        // Activate hold position control
        transform.Find("FocusedObjectControl/HoldPosition").gameObject.SetActive(true);
        // Deactivate that party is on hold information
        transform.Find("FocusedObjectControl/HoldingPositionInfo").gameObject.SetActive(false);
        // verify if there is more than 1 hero of the same faction
        if (mapHeroesList.Count > 1)
        {
            // activate next and previous controls
            ActivateNextAndPreviousControls();
        }
    }

    public void ReleaseFocus()
    {
        Debug.Log("Realease map focus panel focus");
        // clear focused object
        focusedObject = null;
        // reset currently focused object id
        // transform.root.Find("Managers").GetComponent<TurnsManager>().GetActivePlayer().FocusedObjectID = 0;
        // clear name and additional info
        focusedObjectNameText.text = "";
        focusedObjectInfoText.text = "";
        // dimm left and right arrows
        previousButton.SetInteractable(false);
        nextButton.SetInteractable(false);
        //transform.Find("Next").gameObject.SetActive(false);
        // disable all focused object controls
        foreach (Transform childTransform in transform.Find("FocusedObjectControl"))
        {
            childTransform.gameObject.SetActive(false);
        }
        // activate empty control
        transform.Find("FocusedObjectControl/EmptyControl").gameObject.SetActive(true);
        // clear map heroes and cities lists
        mapCitiesList = null;
        mapHeroesList = null;
        // reset selection on Map
        //MapManager.Instance.SetSelection(MapManager.Selection.None);
        // reset cursor to normal, because it is changed by MapManager
        //CursorController.Instance.SetNormalCursor();
    }

}
