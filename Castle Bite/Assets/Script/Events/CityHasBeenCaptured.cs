using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(MapCity))]
public class CityHasBeenCaptured : MonoBehaviour, IFactionHasChanged
{
    public UnityEvent OnCityHasBeenCaptured;
    Faction faction;
    City city;

    void Awake()
    {
        city = GetComponent<MapCity>().linkedCityTr.GetComponent<City>();
        faction = city.GetFaction();
    }

    public bool FactionHasChanged()
    {
        if (city.GetFaction() != faction)
        {
            // update faction, so this function stops triggering on every update
            faction = city.GetFaction();
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
            OnCityHasBeenCaptured.Invoke();
        }
        else
        {
            //Debug.Log("FactionHasNotChanged");
        }
    }
}


namespace UnityEngine.EventSystems
{
    public interface IFactionHasChanged : IEventSystemHandler
    {
        bool FactionHasChanged();
    }
}