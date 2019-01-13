﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game/Event")]
public class GameEvent : ScriptableObject
{
    // TODO
    // ..
    // if it works, then post reply in questions, if relevant
    // https://answers.unity.com/questions/1555927/scriptableobject-event-parameter.html
    // https://stackoverflow.com/questions/52493579/unity-scriptable-object-event-pass-parameter
    // https://gamedev.stackexchange.com/questions/149066/scriptableobject-referencing-a-gameobject-in-scene

    // List of event listeners
    // EventListers are added / removed on their enable and disable
    [NonSerialized]
    private List<EventsListener> eventsListeners = new List<EventsListener>();

    //// These variables are used for events with parameters
    //// They should be set before event rise
    //[NonSerialized]
    //public PartyUnit partyUnit;
    //[NonSerialized]
    //public int gameObjectID;

    public void RegisterEventsListener(EventsListener eventsListener)
    {
        // Verify if listener is not already registered
        if (!eventsListeners.Contains(eventsListener))
        {
            // Add new listener
            eventsListeners.Add(eventsListener);
        }
    }

    public void DeRegisterEventsListener(EventsListener eventListener)
    {
        // Verify if listener is registered
        if (eventsListeners.Contains(eventListener))
        {
            // Remove listener
            eventsListeners.Remove(eventListener);
        }
    }

    public void Raise(GameObject gameObject = null)
    {
        // Loop through all registered listeners
        for (int i = eventsListeners.Count - 1; i >= 0; i--)
        {
            // Initiate actions on event listener
            eventsListeners[i].ActOnEvent(this, gameObject);
        }
    }

    public void Raise(GameObject gameObject, int difference)
    {
        // Loop through all registered listeners
        for (int i = eventsListeners.Count - 1; i >= 0; i--)
        {
            // Initiate actions on event listener
            eventsListeners[i].ActOnEvent(this, gameObject, difference);
        }
    }

    public void Raise(GameObject gameObject, ScriptableObject scriptableObject)
    {
        // Loop through all registered listeners
        for (int i = eventsListeners.Count - 1; i >= 0; i--)
        {
            // Initiate actions on event listener
            eventsListeners[i].ActOnEvent(this, gameObject, scriptableObject);
        }
    }
}
