using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/BaseBuffUPM")]
public class BaseBuffUPM : UniquePowerModifier
{
    public override void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    {
        Debug.Log("Applying " + uniquePowerModifierConfig.DisplayName + " DoT " + " from " + srcPartyUnit.UnitName + " to " + dstPartyUnit.UnitName + ", origin is " + uniquePowerModifierID.modifierOrigin);
        // init upm data variable (this is required for game save / restore)
        UniquePowerModifierData upmData = new UniquePowerModifierData
        {
            // set UPM ID
            UniquePowerModifierID = uniquePowerModifierID,
            // rest upm duration left to max duration
            DurationLeft = uniquePowerModifierConfig.UpmDurationMax,
            // set upm current power based on source unit stats upgrades count
            //CurrentPower = uniquePowerModifierConfig.GetUpmCurrentPower(srcPartyUnit.StatsUpgradesCount)
            CurrentPower = uniquePowerModifierConfig.GetUpmEffectivePower(srcPartyUnit.gameObject)
        };
        // find upm with the same modifier in the list of upms on destination party unit
        UniquePowerModifierData sameUPM = dstPartyUnit.AppliedUniquePowerModifiersData.Find(e => (e.UniquePowerModifierID == uniquePowerModifierID));
        // verify if the same UPM has already been found or applied (not null)
        if (sameUPM != null)
        {
            // verify if duration is not max already
            if (sameUPM.DurationLeft != uniquePowerModifierConfig.UpmDurationMax)
            {
                // reset existing UPM duration to max
                sameUPM.DurationLeft = uniquePowerModifierConfig.UpmDurationMax;
                // raise an event
                //uniquePowerModifierDurationHasBeenResetToMaxEvent.Raise(sameUPM);
                Events.DurationHasBeenResetToMaxEvent.Raise(sameUPM);
            }
            // verify if power is different
            if (sameUPM.CurrentPower != upmData.CurrentPower)
            {
                // reset its power to current power (in case power of source party unit has changed over time)
                sameUPM.CurrentPower = upmData.CurrentPower;
                // raise an event
                //uniquePowerModifierPowerHasBeenChangedEvent.Raise(sameUPM);
                Events.PowerHasBeenChangedEvent.Raise(sameUPM);
            }
        }
        else
        {
            // add new upm data to the list of upms on the destination unit
            dstPartyUnit.AppliedUniquePowerModifiersData.Add(upmData);
            // raise an event
            //uniquePowerModifierDataHasBeenAddedEvent.Raise(dstPartyUnit.gameObject);
            Events.DataHasBeenAddedEvent.Raise(dstPartyUnit.gameObject);
        }
    }

    public override void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        Debug.Log("Trigger " + uniquePowerModifierData.GetOriginDisplayName() + " UPM");
        // get unique power modifier config
        UniquePowerModifierConfig uniquePowerModifierConfig = uniquePowerModifierData.GetUniquePowerModifierConfig();
        // verify if it is active or passive UPM
        if (uniquePowerModifierConfig.ModifierAppliedHow == ModifierAppliedHow.Active)
        {
            // normally this is somethins which buffs current unit stats, for example: health (heal unit)
            Debug.LogWarning(".. Apply active buff");
        }
        else
        {
            // this is passive
            // nothing to do here, because buff is calculated and applied automatically, for example: defense bonus
        }
        // Decrement DoT current duration
        uniquePowerModifierData.DurationLeft -= 1;
        // note: order is important. Trigger should be last, because it may also remove UPM status icon
        // Trigger on duration changed event
        //uniquePowerModifierDurationHasChangedEvent.Raise(uniquePowerModifierData);
        Events.DurationHasChangedEvent.Raise(uniquePowerModifierData);
        // Trigger on UPM has been triggered
        //uniquePowerModifierHasBeenTriggeredEvent.Raise(uniquePowerModifierData);
        Events.HasBeenTriggeredEvent.Raise(uniquePowerModifierData);
    }

}
