using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Advisers/Advise Against Healing On Full Health")]
public class AdviseAgainstHealingOnFullHealth : ModifierAdviser
{
    public override bool DoAdviseAgainstUPMUsageInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // get destination context as PartyUnit
        if (dstContext is PartyUnit)
        {
            PartyUnit dstPartyUnit = (PartyUnit)dstContext;
            // verify if destination unit health is already max
            if (dstPartyUnit.UnitHealthCurr == dstPartyUnit.GetUnitEffectiveMaxHealth())
            {
                return true;
            }
        }
        // by default don't advise against
        return false;
    }
}
