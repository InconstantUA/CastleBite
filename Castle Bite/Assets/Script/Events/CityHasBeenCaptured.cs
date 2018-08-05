using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// Add new event to EventSystems
namespace UnityEngine.EventSystems
{
    // Return true if Faction of City or Unit has changed
    public interface IFactionHasChanged : IEventSystemHandler
    {
        bool FactionHasChanged();
    }
}

[RequireComponent(typeof(MapCity))]
public class CityHasBeenCaptured : MonoBehaviour, IFactionHasChanged
{
    // create event, which later can be handled in Unity Editor
    public UnityEvent OnCityHasBeenCaptured;
    // create variable to hold current faction, and later use it to compare with city faction
    Faction faction;
    // create variable to hold link to City
    City city;

    void Awake()
    {
        city = GetComponent<MapCity>().LinkedCityTr.GetComponent<City>();
        faction = city.Faction;
    }

    public bool FactionHasChanged()
    {
        if (city.Faction != faction)
        {
            // update faction, so this function stops triggering on every update
            faction = city.Faction;
            return true;
        }
        else
        {
            return false;
        }
    }

    void Update()
    {
        if (FactionHasChanged())
        {
            Debug.Log("FactionHasChanged");
            // Invoke all functions, which are registered as listeners in Unity Editor
            // Object, can register its functions to be called on event
            // For this object should have public function and developer should point to it in Unity Editor
            OnCityHasBeenCaptured.Invoke();
        }
        else
        {
            //Debug.Log("FactionHasNotChanged");
        }
    }
}

