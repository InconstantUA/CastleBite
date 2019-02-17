using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Advisers/Advise Against Healing On Full Health")]
public class AdviseAgainstHealingOnFullHealth : ModifierAdviser
{
    //public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    //{
    //    return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    //}

    //public bool DoesSourceContextMatch(System.Object srcContext)
    //{
    //    // ignore source context
    //    return true;
    //}

    //public bool DoesDestinationContextMatch(System.Object dstContext)
    //{
    //    if (dstContext is PartyUnit)
    //    {
    //        // match
    //        return true;
    //    }
    //    Debug.Log("Destination context doesn't match");
    //    // doesn't match
    //    return false;
    //}

    //public override bool DoAdviseAgainstUPMUsageInContextOf(System.Object srcContext, System.Object dstContext)
    //{
    //    // verify if source or destination context do not match requirements of this limiter
    //    if (!DoesContextMatch(srcContext, dstContext))
    //    {
    //        // context is not in scope of this limiter - don't limit
    //        return false;
    //    }
    //    // get destination context as PartyUnit
    //    if (dstContext is PartyUnit)
    //    {
    //        PartyUnit dstPartyUnit = (PartyUnit)dstContext;
    //        // verify if destination unit health is already max
    //        if (dstPartyUnit.UnitHealthCurr == dstPartyUnit.GetUnitEffectiveMaxHealth())
    //        {
    //            return true;
    //        }
    //    }
    //    // by default don't advise against
    //    return false;
    //}

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if destination unit slot is set
            if (BattleContext.DestinationUnitSlot != null)
                // verify if destination slot has unit
                if (BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>())
                    // context match
                    return true;
        }
        // by default context doesn't match
        return false;
    }

    public override bool DoAdviseAgainstUPMUsageInContextOf(System.Object context)
    {
        // verify if source or destination context do not match requirements of this limiter
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this limiter - don't advise against
            return false;
        }
        // get destination context as PartyUnit
        PartyUnit dstPartyUnit = BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
        // verify if destination unit health is already max
        if (dstPartyUnit.UnitHealthCurr == dstPartyUnit.GetUnitEffectiveMaxHealth())
        {
            return true;
        }
        // by default don't advise against
        return false;
    }
}
