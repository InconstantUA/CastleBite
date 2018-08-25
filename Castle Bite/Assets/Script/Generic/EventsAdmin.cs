﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CityCaptureEvent : UnityEvent<City>
{
}

public class EventsAdmin : MonoBehaviour {
    // create event, which later can be handled in Unity Editor
    public CityCaptureEvent OnCityHasBeenCaptured;

    // For city faction changes
    public void IHasChanged(City city, Faction oldFaction)
    {
        // Invoke all functions, which are registered as listeners in Unity Editor
        // Object, can register its functions to be called on event
        // For this object should have public function and developer should point to it in Unity Editor
        OnCityHasBeenCaptured.Invoke(city);
    }
}
