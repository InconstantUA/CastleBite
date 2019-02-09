using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
public class ModifierConfigUpdater : ScriptableObject
{
    public virtual bool DoesContextMatch(System.Object context)
    {
        // context matches
        return true;
    }

    // default update
    public virtual UnitStatModifierConfig GetUpdatedUSMC(UnitStatModifierConfig unitStatModifierConfig, System.Object context)
    {
        if (DoesContextMatch(context))
        {
            // do changes
            // return updated UnitStatModifierConfig
        }
        // by default do not do any changes
        return unitStatModifierConfig;
    }

    // default get quantitive bonus of the updater
    public virtual int GetDifference(UnitStatModifierConfig unitStatModifierConfig, System.Object context)
    {
        // return difference between updated modifier power and default power
        return GetUpdatedUSMC(unitStatModifierConfig, context).modifierPower - unitStatModifierConfig.modifierPower;
    }
}
