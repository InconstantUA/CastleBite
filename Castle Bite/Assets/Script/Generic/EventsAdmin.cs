using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CityCaptureEvent : UnityEvent<City>
{
}

public class EventsAdmin : MonoBehaviour {
    public static EventsAdmin Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // create events, which later can be handled in Unity Editor
    // Create on city has been captured event
    public CityCaptureEvent OnCityHasBeenCaptured;
    // create on player gold changed event
    public UnityEvent OnPlayerGoldHasChanged;
    // create on player gold changed event
    public UnityEvent OnPlayerManaHasChanged;

    // For city faction changes
    // This function is Triggered when city faction parameter is set
    public void IHasChanged(City city, Faction oldFaction)
    {
        // Invoke all functions, which are registered as listeners in Unity Editor
        // Object, can register its functions to be called on event
        // For this object should have public function and developer should point to it in Unity Editor
        OnCityHasBeenCaptured.Invoke(city);
    }

    // For player gold changed
    public void IHasChanged(GamePlayer gamePlayer, Gold gold)
    {
        OnPlayerGoldHasChanged.Invoke();
    }

    // For player mana changed
    public void IHasChanged(GamePlayer gamePlayer, Mana gold)
    {
        OnPlayerManaHasChanged.Invoke();
    }

}
