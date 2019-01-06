using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game/Event")]
public class GameEvent : ScriptableObject
{
    // List of event listeners
    // EventListers are added / removed on their enable and disable
    //private List<EventsListener> eventsListeners = new List<EventsListener>();

    // Moved this functionality to EventsManager, because of MonoBehaviour cannot be member of ScriptableObject or passed via it as parameter
    //// These variables are used for events with parameters
    //// They should be set before event rise
    //[NonSerialized]
    //public PartyUnit partyUnit;
    //[NonSerialized]
    //public int gameObjectID;

    //public void RegisterEventsListener(EventsListener eventsListener)
    //{
    //    EventsManager.StartListening(this, eventsListener);
    //    //// Verify if listener is not already registered
    //    //if (!eventsListeners.Contains(eventsListener))
    //    //{
    //    //    // Add new listener
    //    //    eventsListeners.Add(eventsListener);
    //    //}
    //}

    //public void DeRegisterEventsListener(EventsListener eventListener)
    //{
    //    EventsManager.StopListening(this, eventListener);
    //    //// Verify if listener is registered
    //    //if (eventsListeners.Contains(eventListener))
    //    //{
    //    //    // Remove listener
    //    //    eventsListeners.Remove(eventListener);
    //    //}
    //}

    //public void Raise()
    //{
    //    // Loop through all registered listeners
    //    for (int i = eventsListeners.Count - 1; i >= 0; i--)
    //    {
    //        // Initiate actions on event listener
    //        eventsListeners[i].ActOnEvent(this);
    //    }
    //}
}
