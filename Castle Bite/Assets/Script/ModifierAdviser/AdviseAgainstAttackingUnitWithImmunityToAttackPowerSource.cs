using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Advisers/Advise Against Attacking Unit With Immunity To Attack Power Source")]
public class AdviseAgainstAttackingUnitWithImmunityToAttackPowerSource : ModifierAdviser
{
    [SerializeField]
    PowerSource immunityToPowerSource;

    //public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    //{
    //    return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    //}

    //public bool DoesSourceContextMatch(System.Object srcContext)
    //{
    //    // always match
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
    //        // context is not in scope of this limiter - don't advise against
    //        return false;
    //    }
    //    // get destination context as PartyUnit
    //    if (dstContext is PartyUnit)
    //    {
    //        PartyUnit dstPartyUnit = (PartyUnit)dstContext;
    //        // verify if unit is immunte to specific power source (resistance is more than or equal to 100%)
    //        if (dstPartyUnit.GetUnitEffectiveResistance(immunityToPowerSource) >= 100)
    //        {
    //            // advise against
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
        // verify if unit is immunte to specific power source (resistance is more than or equal to 100%)
        if (dstPartyUnit.GetUnitEffectiveResistance(immunityToPowerSource) >= 100)
        {
            // advise against
            return true;
        }
        // by default don't advise against
        return false;
    }

}
