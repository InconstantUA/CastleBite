using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapFocusPanel : MonoBehaviour {
    GameObject focusedObject;
    List<MapHero> mapHeroesList;
    List<MapCity> mapCitiesList;

    void OnEnable()
    {
        // check if focused object is not defined yet
        if (focusedObject == null)
        {
            ReleaseFocus();
        }
    }

    void SetHeroNameUI(MapHero mapHero)
    {
        transform.Find("Name").GetComponent<Text>().text = mapHero.LHeroParty.GetPartyLeader().GivenName + "\r\n<size=12>" + mapHero.LHeroParty.GetPartyLeader().UnitName + "</size>";
    }

    public void SetActive(MapHero mapHero)
    {
        // first do cleanup in case if previously were selected other object
        if (focusedObject != null)
        {
            ReleaseFocus();
        }
        // Set hero name and unit name information
        Debug.Log("Update map focus panel with " + mapHero.name);
        focusedObject = mapHero.gameObject;
        SetHeroNameUI(mapHero);
        // Set list of map heroes
        mapHeroesList = new List<MapHero>();
        // Loop through all map heroes
        foreach (MapHero mHero in transform.root.Find("MapScreen/Map").GetComponentsInChildren<MapHero>())
        {
            // verify if mHero faction is the same as for active hero
            if (mapHero.LHeroParty.Faction == mHero.LHeroParty.Faction)
            {
                mapHeroesList.Add(mHero);
            }
        }
        // verify if there is more than 1 hero of the same faction
        if (mapHeroesList.Count > 1)
        {
            // activate next and previous controls
            transform.Find("Previous").GetComponent<TextButton>().SetInteractable(true);
            transform.Find("Next").GetComponent<TextButton>().SetInteractable(true);
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
        Debug.LogError("Failed to find seleted hero in the list");
        return 0;
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

    void ChangeHeroFocus(MapHero nextHero)
    {
        focusedObject = nextHero.gameObject;
        // set name UI
        SetHeroNameUI(nextHero);
        // select hero on map
        transform.root.Find("MapScreen/Map").GetComponent<MapManager>().SetSelection(MapManager.Selection.PlayerHero, nextHero);
        // reset cursor to normal, because it is changed by MapManager
        transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
    }

    void ChangeCityFocus(MapCity nextCity)
    {
        focusedObject = nextCity.gameObject;
        // set name UI
        SetCityNameUI(nextCity);
        // select city on map
        transform.root.Find("MapScreen/Map").GetComponent<MapManager>().SetSelection(MapManager.Selection.PlayerCity, nextCity);
        // reset cursor to normal, because it is changed by MapManager
        transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
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
            MapHero previousHero = mapHeroesList[GetPreviousIndex(GetSelectedHeroIndex(), mapHeroesList.Count)];
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
        string additionalInfo = "City";
        // verify cityType
        if (mapCity.LCity.CityType == CityType.Capital)
        {
            additionalInfo = "Captial";
        }
        transform.Find("Name").GetComponent<Text>().text = mapCity.LCity.CityName + "\r\n<size=12>" + additionalInfo + "</size>";
    }

    public void SetActive(MapCity mapCity)
    {
        // first do cleanup in case if previously were selected other object
        if (focusedObject != null)
        {
            ReleaseFocus();
        }
        // Set focused object
        Debug.Log("Update map focus panel with " + mapCity.name);
        focusedObject = mapCity.gameObject;
        SetCityNameUI(mapCity);
        // Set list of map heroes
        mapCitiesList = new List<MapCity>();
        // Loop through all map cities
        foreach (MapCity mCity in transform.root.Find("MapScreen/Map").GetComponentsInChildren<MapCity>())
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
            transform.Find("Previous").GetComponent<TextButton>().SetInteractable(true);
            transform.Find("Next").GetComponent<TextButton>().SetInteractable(true);
        }
    }

    public void ReleaseFocus()
    {
        Debug.Log("Realease map focus panel focus");
        // clear focused object
        focusedObject = null;
        // clear name
        transform.Find("Name").GetComponent<Text>().text = "";
        // dimm left and right arrows
        transform.Find("Previous").GetComponent<TextButton>().SetInteractable(false);
        transform.Find("Next").GetComponent<TextButton>().SetInteractable(false);
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
        //transform.root.Find("MapScreen/Map").GetComponent<MapManager>().SetSelection(MapManager.Selection.None);
        // reset cursor to normal, because it is changed by MapManager
        //transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
    }

}
