using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Events")]
public class UniquePowerModifierEvents : ScriptableObject
{
    // event for UI when UPM data is added
    [SerializeField]
    private GameEvent dataHasBeenAddedEvent;
    // event for UI when UPM data is removed
    [SerializeField]
    private GameEvent dataHasBeenRemovedEvent;
    // event for UI when UPM duration has been reset to max
    [SerializeField]
    private GameEvent durationHasBeenResetToMaxEvent;
    // event for UI when UPM duration has been changed
    [SerializeField]
    private GameEvent durationHasChangedEvent;
    // event for UI when UPM power has been changed
    [SerializeField]
    private GameEvent powerHasBeenChangedEvent;
    // event for UI when UPM has been triggered
    [SerializeField]
    private GameEvent hasBeenTriggeredEvent;

    public GameEvent DataHasBeenAddedEvent
    {
        get
        {
            return dataHasBeenAddedEvent;
        }
    }

    public GameEvent DurationHasBeenResetToMaxEvent
    {
        get
        {
            return durationHasBeenResetToMaxEvent;
        }
    }

    public GameEvent DurationHasChangedEvent
    {
        get
        {
            return durationHasChangedEvent;
        }
    }

    public GameEvent PowerHasBeenChangedEvent
    {
        get
        {
            return powerHasBeenChangedEvent;
        }
    }

    public GameEvent HasBeenTriggeredEvent
    {
        get
        {
            return hasBeenTriggeredEvent;
        }
    }

    public GameEvent DataHasBeenRemovedEvent
    {
        get
        {
            return dataHasBeenRemovedEvent;
        }
    }
}
