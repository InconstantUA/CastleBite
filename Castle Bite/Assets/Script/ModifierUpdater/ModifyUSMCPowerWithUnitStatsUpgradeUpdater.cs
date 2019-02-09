using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Updaters/Modify USMC Power by x with Unit stats upgrade")]
public class ModifyUSMCPowerWithUnitStatsUpgradeUpdater : ModifierConfigUpdater
{
    public int powerIncrementOnStatsUpgrade;

    public override bool DoesContextMatch(object context)
    {
        return context is PartyUnit;
    }

    // default update
    public override UnitStatModifierConfig GetUpdatedUSMC(UnitStatModifierConfig unitStatModifierConfig, System.Object context)
    {
        // get party unit from gameObject
        //PartyUnit partyUnit = context.GetComponent<PartyUnit>();
        // verify if party unit is not null
        //if (partyUnit != null)
        // verify if context match
        if (DoesContextMatch(context))
        {
            // init PartyUnit from context
            PartyUnit partyUnit = (PartyUnit)context;
            // copy current USM config (to not make changes on default one)
            UnitStatModifierConfig newUSMConfig = Instantiate(unitStatModifierConfig);
            // get party unit stats upgrade count
            int statsUpgradeCount = partyUnit.StatsUpgradesCount;
            // update to current USM config power
            newUSMConfig.modifierPower += powerIncrementOnStatsUpgrade * statsUpgradeCount;
            // return new USM config
            return newUSMConfig;
        }
        else
        {
            // Debug.LogError("Party Unit reference is null");
            Debug.LogError("Context doesn't match");
        }
        return unitStatModifierConfig;
    }
}
