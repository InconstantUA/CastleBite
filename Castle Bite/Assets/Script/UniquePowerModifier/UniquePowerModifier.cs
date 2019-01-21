using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UniquePowerModifier : ScriptableObject
{
    public abstract void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID);
    public abstract void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData);
}
