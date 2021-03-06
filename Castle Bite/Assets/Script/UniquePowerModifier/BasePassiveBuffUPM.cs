﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/BasePassiveBuffUPM")]
public class BasePassiveBuffUPM : UniquePowerModifier
{
    void ApplyUPMData(UniquePowerModifierData uniquePowerModifierData, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    {
        // .. just in case
        ValidateTriggerCondition(uniquePowerModifierData);
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
            if (sameUPM.CurrentPower != uniquePowerModifierData.CurrentPower)
            {
                // reset its power to current power (in case power of source party unit has changed over time)
                sameUPM.CurrentPower = uniquePowerModifierData.CurrentPower;
                // raise an event
                //uniquePowerModifierPowerHasBeenChangedEvent.Raise(sameUPM);
                Events.PowerHasBeenChangedEvent.Raise(sameUPM);
            }
        }
        else
        {
            // add new upm data to the list of upms on the destination unit
            dstPartyUnit.AppliedUniquePowerModifiersData.Add(uniquePowerModifierData);
            // raise an event
            //uniquePowerModifierDataHasBeenAddedEvent.Raise(dstPartyUnit.gameObject);
            Events.DataHasBeenAddedEvent.Raise(dstPartyUnit.gameObject);
        }
    }

    void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    {
        Debug.Log("Applying " + uniquePowerModifierConfig.DisplayName + " DoT " + " from " + srcPartyUnit.UnitName + " to " + dstPartyUnit.UnitName + ", origin is " + uniquePowerModifierID.modifierOrigin);
        // init upm data variable (this is required for game save / restore)
        UniquePowerModifierData uniquePowerModifierData = new UniquePowerModifierData
        {
            // set UPM ID
            UniquePowerModifierID = uniquePowerModifierID,
            // rest upm duration left to max duration
            DurationLeft = uniquePowerModifierConfig.UpmDurationMax,
            // set upm current power based on source unit stats upgrades count
            //CurrentPower = uniquePowerModifierConfig.GetUpmCurrentPower(srcPartyUnit.StatsUpgradesCount)
            CurrentPower = uniquePowerModifierConfig.GetUpmEffectivePower(srcPartyUnit)
        };
        // Apply UPM to the unit
        ApplyUPMData(uniquePowerModifierData, dstPartyUnit, uniquePowerModifierConfig, uniquePowerModifierID);
        //// find upm with the same modifier in the list of upms on destination party unit
        //UniquePowerModifierData sameUPM = dstPartyUnit.AppliedUniquePowerModifiersData.Find(e => (e.UniquePowerModifierID == uniquePowerModifierID));
        //// verify if the same UPM has already been found or applied (not null)
        //if (sameUPM != null)
        //{
        //    // verify if duration is not max already
        //    if (sameUPM.DurationLeft != uniquePowerModifierConfig.UpmDurationMax)
        //    {
        //        // reset existing UPM duration to max
        //        sameUPM.DurationLeft = uniquePowerModifierConfig.UpmDurationMax;
        //        // raise an event
        //        //uniquePowerModifierDurationHasBeenResetToMaxEvent.Raise(sameUPM);
        //        Events.DurationHasBeenResetToMaxEvent.Raise(sameUPM);
        //    }
        //    // verify if power is different
        //    if (sameUPM.CurrentPower != upmData.CurrentPower)
        //    {
        //        // reset its power to current power (in case power of source party unit has changed over time)
        //        sameUPM.CurrentPower = upmData.CurrentPower;
        //        // raise an event
        //        //uniquePowerModifierPowerHasBeenChangedEvent.Raise(sameUPM);
        //        Events.PowerHasBeenChangedEvent.Raise(sameUPM);
        //    }
        //}
        //else
        //{
        //    // add new upm data to the list of upms on the destination unit
        //    dstPartyUnit.AppliedUniquePowerModifiersData.Add(upmData);
        //    // raise an event
        //    //uniquePowerModifierDataHasBeenAddedEvent.Raise(dstPartyUnit.gameObject);
        //    Events.DataHasBeenAddedEvent.Raise(dstPartyUnit.gameObject);
        //}
    }

    void Apply(InventoryItem inventoryItem, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    {
        Debug.LogWarning("Applying " + uniquePowerModifierConfig.DisplayName + " from " + inventoryItem.ItemName + " to " + dstPartyUnit.UnitName + ", origin is " + uniquePowerModifierID.modifierOrigin);
        // init upm data variable (this is required for Trigger)
        UniquePowerModifierData uniquePowerModifierData = new UniquePowerModifierData
        {
            // set UPM ID
            UniquePowerModifierID = uniquePowerModifierID,
            // rest upm duration left to max duration
            DurationLeft = uniquePowerModifierConfig.UpmDurationMax,
            // set upm current power based on source unit stats upgrades count
            //CurrentPower = uniquePowerModifierConfig.GetUpmCurrentPower(srcPartyUnit.StatsUpgradesCount)
            CurrentPower = uniquePowerModifierConfig.GetUpmEffectivePower(inventoryItem)
        };
        // Apply UPM to the unit
        ApplyUPMData(uniquePowerModifierData, dstPartyUnit, uniquePowerModifierConfig, uniquePowerModifierID);
    }

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // .. this can be skipped because this verification (should be) done before upm is applied
            // verify if destination slot has a unit UI
            if (BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>() != null)
            {
                // context match
                return true;
            }
        }
        // verify if context matches battle context
        if (context is EditPartyScreenContext)
        {
            // .. this can be skipped because this verification (should be) done before upm is applied
            // verify if destination slot has a unit UI
            if (EditPartyScreenContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>() != null)
            {
                // context match
                return true;
            }
        }
        // by default context doesn't match
        return false;
    }

    public override void Apply(System.Object context)
    {
        // verify if context doesn't match requirements of this UPM
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this UPM
            // skip all actions
            return;
        }
        if (context is BattleContext)
        {
            // verify if source context is PartyUnit
            if (BattleContext.ActivePartyUnitUI != null)
            {
                // verify if item has been dragged (if we are here, then it means that Item has been dropped onto the unit slot)
                if (BattleContext.ItemBeingUsed != null)
                {
                    // apply item UPM from Battle Context
                    InventoryItem srcInventoryItem = BattleContext.ItemBeingUsed;
                    PartyUnit dstPartyUnit = BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
                    UniquePowerModifierConfig uniquePowerModifierConfig = srcInventoryItem.InventoryItemConfig.UniquePowerModifierConfigsSortedByExecutionOrder[BattleContext.ActivatedUPMConfigIndex];
                    UniquePowerModifierID uniquePowerModifierID = BattleContext.UniquePowerModifierID;
                    Apply(srcInventoryItem, dstPartyUnit, uniquePowerModifierConfig, uniquePowerModifierID);
                }
                else
                {
                    // apply unit ability UPM
                    PartyUnit srcPartyUnit = BattleContext.ActivePartyUnitUI.LPartyUnit;
                    PartyUnit dstPartyUnit = BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
                    UniquePowerModifierConfig uniquePowerModifierConfig = srcPartyUnit.UnitAbilityConfig.UniquePowerModifierConfigsSortedByExecutionOrder[BattleContext.ActivatedUPMConfigIndex];
                    UniquePowerModifierID uniquePowerModifierID = BattleContext.UniquePowerModifierID;
                    Apply(srcPartyUnit, dstPartyUnit, uniquePowerModifierConfig, uniquePowerModifierID);
                }
            }
            else
            {
                Debug.LogError("Unknown source context");
            }
        }
        if (context is EditPartyScreenContext)
        {
            // apply item UPM from edit party screen context
            InventoryItem srcInventoryItem = EditPartyScreenContext.ItemBeingUsed;
            PartyUnit dstPartyUnit = EditPartyScreenContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
            UniquePowerModifierConfig uniquePowerModifierConfig = srcInventoryItem.InventoryItemConfig.UniquePowerModifierConfigsSortedByExecutionOrder[EditPartyScreenContext.ActivatedUPMConfigIndex];
            UniquePowerModifierID uniquePowerModifierID = EditPartyScreenContext.UniquePowerModifierID;
            Apply(srcInventoryItem, dstPartyUnit, uniquePowerModifierConfig, uniquePowerModifierID);
        }
    }

    //public override void Apply(InventoryItem inventoryItem, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    //{
    //    throw new System.NotImplementedException();
    //}

    void ValidateTriggerCondition(UniquePowerModifierData uniquePowerModifierData)
    {
        // get unique power modifier config
        UniquePowerModifierConfig uniquePowerModifierConfig = uniquePowerModifierData.GetUniquePowerModifierConfig();
        // validate if condition has been configured correctly
        if (uniquePowerModifierConfig.TriggerCondition != TriggerCondition.NonePassive)
        {
            Debug.LogWarning("Trigger condition should be set to None Passive for this UPM");
        }
    }

    public override void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        Debug.Log("Trigger " + uniquePowerModifierData.GetOriginDisplayName() + " UPM");
        // .. just in case
        ValidateTriggerCondition(uniquePowerModifierData);
        // Decrement current duration
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
