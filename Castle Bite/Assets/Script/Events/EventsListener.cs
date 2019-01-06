using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventAndAction
{
    public string name;
    public GameEvent gameEvent;
    public UnityEvent genericAction;
    public PartyUnitEvent partyUnitEvent;
    //public EventWithTriggeredObjectReference eventWithTriggeredObjectReference;

    public void Invoke(GameObject gameObject = null)
    {
        // Check if at least 1 object is listening for the event
        if (genericAction.GetPersistentEventCount() >= 1)
        {
            genericAction.Invoke();
        }
        // we cannot invoke MonoBehaviour from ScriptableObject
        // send it as System.Object
        // Check if at least 1 object is listening for the event
        if (partyUnitEvent.GetPersistentEventCount() >= 1)
        {
            Debug.LogWarning("Invoking event for " + gameObject.GetComponent<PartyUnit>().UnitName + " unit");
            partyUnitEvent.Invoke(gameObject.GetComponent<PartyUnit>());
        }
        //// Check if at least 1 object is listening for the event
        //if (eventWithTriggeredObjectReference.GetPersistentEventCount() >= 1)
        //{
        //    Debug.LogWarning("Invoking event for " + gameEvent.partyUnit.UnitName + " unit");
        //    // partyUnitEvent.Invoke(gameEvent.partyUnit);
        //    eventWithTriggeredObjectReference.Invoke(gameEvent.partyUnit);
        //}
    }
}

public class EventsListener : MonoBehaviour
{
    //[Reorderable]
    [SerializeField]
    List<EventAndAction> eventsAndActions = new List<EventAndAction>();

    private void OnEnable()
    {
        // loop through each event and response
        foreach (EventAndAction eventAndAction in eventsAndActions)
        {
            // Register this event listener in defined GameEvent
            // EventsManager.StartListening(eventAndAction.gameEvent, this);
            eventAndAction.gameEvent.RegisterEventsListener(this);
        }
    }

    private void OnDisable()
    {
        // loop through each event and response
        foreach (EventAndAction eventAndAction in eventsAndActions)
        {
            // Deregister this event listener in defined GameEvent
            // EventsManager.StopListening(eventAndAction.gameEvent, this);
            eventAndAction.gameEvent.DeRegisterEventsListener(this);
        }
    }

    [ContextMenu("Execute all actions")]
    public void ActOnEvent(GameEvent raisedGameEvent, GameObject gameObject = null)
    {
        // loop from the lately registered to the earlier registered events
        for (int i = eventsAndActions.Count - 1; i >= 0; i--)
        {
            // Check if this is raised GameEvent
            if (raisedGameEvent == eventsAndActions[i].gameEvent)
            {
                // Uncomment the line below for debugging the event listens and other details
                Debug.Log("Called Event named: " + eventsAndActions[i].name + " on GameObject: " + gameObject.name);
                eventsAndActions[i].Invoke(gameObject);
            }
        }
    }
}