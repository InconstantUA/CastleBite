using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
public class ModifierConfigUpdater : ScriptableObject
{
    // default update
    public virtual UnitStatModifierConfig GetUpdatedUSMC(UnitStatModifierConfig unitStatModifierConfig, GameObject gameObject)
    {
        // by default do not do any changes
        return unitStatModifierConfig;
    }

    // default get quantitive bonus of the updater
    public virtual int GetDifference(UnitStatModifierConfig unitStatModifierConfig, GameObject gameObject)
    {
        // return difference between updated modifier power and default power
        return GetUpdatedUSMC(unitStatModifierConfig, gameObject).modifierPower - unitStatModifierConfig.modifierPower;
    }
}
