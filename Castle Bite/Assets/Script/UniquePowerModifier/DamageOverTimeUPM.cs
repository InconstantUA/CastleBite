﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/DamageOverTime")]
public class DamageOverTimeUPM : UniquePowerModifier
{
    // event for UI when UPM data is added
    [SerializeField]
    GameEvent uniquePowerModifierDataHasBeenAddedEvent;
    // event for UI when UPM duration has been reset to max
    [SerializeField]
    GameEvent uniquePowerModifierDurationHasBeenResetToMaxEvent;
    // event for UI when UPM power has been changed
    [SerializeField]
    GameEvent uniquePowerModifierPowerHasBeenChangedEvent;

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
            CurrentPower = uniquePowerModifierConfig.GetUpmCurrentPower(srcPartyUnit.StatsUpgradesCount)
        };
        // find upm with the same modifier in the list of upms on destination party unit
        UniquePowerModifierData sameUPM = dstPartyUnit.UniquePowerModifiersData.Find(e => (e.UniquePowerModifierID == uniquePowerModifierID));
        // verify if the same UPM has already been found or applied (not null)
        if (sameUPM != null)
        {
            // verify if duration is not max already
            if (sameUPM.DurationLeft != uniquePowerModifierConfig.UpmDurationMax)
            {
                // reset existing UPM duration to max
                sameUPM.DurationLeft = uniquePowerModifierConfig.UpmDurationMax;
                // raise an event
                uniquePowerModifierDurationHasBeenResetToMaxEvent.Raise(sameUPM);
            }
            // verify if power is different
            if (sameUPM.CurrentPower != upmData.CurrentPower)
            {
                // reset its power to current power (in case power of source party unit has changed over time)
                sameUPM.CurrentPower = upmData.CurrentPower;
                // raise an event
                uniquePowerModifierPowerHasBeenChangedEvent.Raise(sameUPM);
            }
        }
        else
        {
            // add new upm data to the list of upms on the destination unit
            dstPartyUnit.UniquePowerModifiersData.Add(upmData);
            // raise an event
            uniquePowerModifierDataHasBeenAddedEvent.Raise(dstPartyUnit.gameObject);
        }
    }

}