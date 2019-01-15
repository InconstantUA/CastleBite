﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Unit Ability", menuName = "Config/Unit/Abilities/Config")]
public class UnitAbilityConfig : ScriptableObject
{
    public UnitAbilityID unitAbilityID;
    public UnitAbility unitAbility;
    public string abilityDisplayName;
    public string abilityDescription;
    public UnitAbilityRange unitAbilityRange;
    //public PowerSource powerSource;
    //public UnitPowerScope unitPowerScope;
    // main ability power
    public UnitStatModifierConfig unitStatModifierConfig;
    // additional ability powers
    public List<UnitPowerConfig> unitPowerConfigs;
    public List<UnitPowerModifier> unitPowerModifiers;

    public bool isApplicableToUnit(PartyUnit partyUnit)
    {
        // loop through all applicable statuses in unitStatModifierConfig
        foreach (UnitStatus unitStatus in unitStatModifierConfig.canBeAppliedToTheUnitsWithStatuses)
        {
            // verify if unit status match
            if (partyUnit.UnitStatus == unitStatus)
            {
                return true;
            }
        }
        // if no one status matches, then return false
        return false;
    }
}
