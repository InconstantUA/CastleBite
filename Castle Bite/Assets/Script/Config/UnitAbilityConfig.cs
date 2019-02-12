﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Unit Ability", menuName = "Config/Unit/Abilities/Config")]
public class UnitAbilityConfig : ScriptableObject
{
    public string abilityDisplayName;
    public string abilityDescription;
    public UnitAbilityID unitAbilityID;
    public UnitAbility unitAbility;
    [EnumFlag]
    public UnitAbilityTypes unitAbilityTypes;
    public UnitAbilityRange unitAbilityRange;
    //public bool isMassScopeAbility;
    //public PowerSource powerSource;
    //public UnitPowerScope unitPowerScope;
    // main ability modifier
    public List<UnitPowerModifier> preActionUnitPowerModifiers;
    // main ability power
    //public UnitStatModifierConfig unitStatModifierConfig;
    public UniquePowerModifierConfig primaryUniquePowerModifierConfig;
    // Unique power modifiers
    public List<UniquePowerModifierConfig> postActionUniquePowerModifierConfigs;
    public List<UniquePowerModifierConfig> uniquePowerModifierConfigs;
    //public List<UniquePowerModifierConfig> postActionUniquePowerModifierConfigs;

    //public bool IsApplicableToUnit(PartyUnit partyUnit)
    //{
    //    // loop through all applicable statuses in unitStatModifierConfig
    //    foreach (UnitStatus unitStatus in unitStatModifierConfig.canBeAppliedToTheUnitsWithStatuses)
    //    {
    //        // verify if unit status match
    //        if (partyUnit.UnitStatus == unitStatus)
    //        {
    //            return true;
    //        }
    //    }
    //    // if no one status matches, then return false
    //    return false;
    //}
}
