using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PartyUnitEvent : UnityEvent<PartyUnit>
{
}

[System.Serializable]
public class PartyUnitHealthHasChangedEvent : UnityEvent<PartyUnit, int>
{
}

[System.Serializable]
public class UnitPowerModifierEvent : UnityEvent<PartyUnit, UnitPowerModifier>
{
}

[System.Serializable]
public class UnitAbilityEvent : UnityEvent<PartyUnit, UnitAbility>
{
}

[System.Serializable]
public class EventAndAction
{
    public string name;
    public GameEvent gameEvent;
    public UnityEvent genericAction;
    public PartyUnitEvent partyUnitEvent;
    public PartyUnitHealthHasChangedEvent partyUnitHealthHasChangedEvent;
    public UnitPowerModifierEvent unitPowerModifierEvent;
    public UnitAbilityEvent unitAbilityEvent;
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
            // Debug.Log("Invoking event for " + gameObject.GetComponent<PartyUnit>().UnitName + " unit");
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

    public void Invoke(GameObject gameObject, int difference)
    {
        // Check if at least 1 object is listening for the event
        if (partyUnitHealthHasChangedEvent.GetPersistentEventCount() >= 1)
        {
            // Debug.Log("Invoking event for " + gameObject.GetComponent<PartyUnit>().UnitName + " unit, difference is " + difference);
            partyUnitHealthHasChangedEvent.Invoke(gameObject.GetComponent<PartyUnit>(), difference);
        }
    }

    public void Invoke(GameObject gameObject, ScriptableObject scriptableObject)
    {
        // Check if at least 1 object is listening for the event
        if (unitPowerModifierEvent.GetPersistentEventCount() >= 1)
        {
            // Debug.Log("Invoking event for " + gameObject.GetComponent<PartyUnit>().UnitName + " and " + scriptableObject.name + " scriptable object");
            unitPowerModifierEvent.Invoke(gameObject.GetComponent<PartyUnit>(), (UnitPowerModifier)scriptableObject);
        }
        // Check if at least 1 object is listening for the event
        if (unitAbilityEvent.GetPersistentEventCount() >= 1)
        {
            Debug.Log("Invoking event for " + gameObject.GetComponent<PartyUnit>().UnitName + " and " + scriptableObject.name + " ability");
            unitAbilityEvent.Invoke(gameObject.GetComponent<PartyUnit>(), (UnitAbility)scriptableObject);
        }
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
                if (gameObject != null)
                {
                    // Debug.Log("Called Event named: " + eventsAndActions[i].name + " on GameObject: " + gameObject.name);
                }
                else
                {
                    // Debug.Log("Called Event named: " + eventsAndActions[i].name);
                }
                eventsAndActions[i].Invoke(gameObject);
            }
        }
    }

    public void ActOnEvent(GameEvent raisedGameEvent, GameObject gameObject, int difference)
    {
        // loop from the lately registered to the earlier registered events
        for (int i = eventsAndActions.Count - 1; i >= 0; i--)
        {
            // Check if this is raised GameEvent
            if (raisedGameEvent == eventsAndActions[i].gameEvent)
            {
                // Uncomment the line below for debugging the event listens and other details
                //Debug.Log("Called Event named: " + eventsAndActions[i].name + " on GameObject: " + gameObject.name + ", difference is " + difference);
                eventsAndActions[i].Invoke(gameObject, difference);
            }
        }
    }

    public void ActOnEvent(GameEvent raisedGameEvent, GameObject gameObject, ScriptableObject scriptableObject)
    {
        // loop from the lately registered to the earlier registered events
        for (int i = eventsAndActions.Count - 1; i >= 0; i--)
        {
            // Check if this is raised GameEvent
            if (raisedGameEvent == eventsAndActions[i].gameEvent)
            {
                // Uncomment the line below for debugging the event listens and other details
                Debug.Log("Called Event named: " + eventsAndActions[i].name + " on GameObject: " + gameObject.name + " and on ScriptableObject: " + scriptableObject.name);
                eventsAndActions[i].Invoke(gameObject, scriptableObject);
            }
        }
    }

    //public void ActOnEvent(GameEvent raisedGameEvent, GameObject gameObject, UnitAbilityID unitAbilityID)
    //{
    //    // loop from the lately registered to the earlier registered events
    //    for (int i = eventsAndActions.Count - 1; i >= 0; i--)
    //    {
    //        // Check if this is raised GameEvent
    //        if (raisedGameEvent == eventsAndActions[i].gameEvent)
    //        {
    //            // Uncomment the line below for debugging the event listens and other details
    //            Debug.Log("Called Event named: " + eventsAndActions[i].name + " on GameObject: " + gameObject.name + " and UnitAbilityID: " + unitAbilityID);
    //            eventsAndActions[i].Invoke(gameObject, unitAbilityID);
    //        }
    //    }
    //}
}