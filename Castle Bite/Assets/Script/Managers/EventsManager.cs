using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

// Note: this functionality has been moved to GameEvent
public class EventsManager : MonoBehaviour
{
    private Dictionary<GameEvent, List<EventsListener>> eventsDictionary;

    private static EventsManager eventsManager;
    public static EventsManager Instance
    {
        get
        {
            if (!eventsManager)
            {
                eventsManager = FindObjectOfType(typeof(EventsManager)) as EventsManager;

                if (!eventsManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventsManager.Init();
                }
            }
            return eventsManager;
        }
    }

    void Init()
    {
        if (eventsDictionary == null)
        {
            eventsDictionary = new Dictionary<GameEvent, List<EventsListener>>();
        }
    }

    public static void StartListening(GameEvent gameEvent, EventsListener listener)
    {
        List<EventsListener> eventsListeners = null;
        // Verify if game event already exists in the dictionary
        if (Instance.eventsDictionary.TryGetValue(gameEvent, out eventsListeners))
        {
            eventsListeners.Add(listener);
        }
        else
        {
            eventsListeners = new List<EventsListener>
            {
                listener
            };
            Instance.eventsDictionary.Add(gameEvent, eventsListeners);
        }
    }

    public static void StopListening(GameEvent gameEvent, EventsListener listener)
    {
        if (eventsManager == null) return;
        List<EventsListener> eventsListeners = null;
        if (Instance.eventsDictionary.TryGetValue(gameEvent, out eventsListeners))
        {
            eventsListeners.Remove(listener);
        }
    }

    public static void TriggerEvent(GameEvent gameEvent, GameObject gameObject = null)
    {
        List<EventsListener> eventsListeners = null;
        if (Instance.eventsDictionary.TryGetValue(gameEvent, out eventsListeners))
        {
            // Loop through all registered listeners
            for (int i = eventsListeners.Count - 1; i >= 0; i--)
            {
                // Initiate actions on event listener
                eventsListeners[i].ActOnEvent(gameEvent, gameObject);
            }
        }
    }
}