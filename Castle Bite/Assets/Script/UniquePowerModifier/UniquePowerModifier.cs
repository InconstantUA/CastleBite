using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UniquePowerModifier : ScriptableObject
{
    [SerializeField]
    UniquePowerModifierEvents events;

    public UniquePowerModifierEvents Events
    {
        get
        {
            return events;
        }
    }

    //public abstract void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID);
    //public abstract void Apply(InventoryItem inventoryItem, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID);
    public abstract void Apply(System.Object context);
    public abstract void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData);
}
