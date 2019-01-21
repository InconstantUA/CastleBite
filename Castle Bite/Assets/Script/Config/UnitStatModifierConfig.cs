﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/StatModifier")]
public class UnitStatModifierConfig : ScriptableObject
{
    // instead of creating unit stat modifier (USM) ID for each config we reference USM via its container
    // example:
    //  itemID,          USM[index]inList
    //  unitAbilityID,   USM[index]inList
    //  unitsSkillID,    USM[index]inList
    //public UnitStatModifierID unitStatModifierID; // used in UnitStatModifierData, which is used when modifier is applied to other units from unit ability, for example: paralyze
    public UnitStat unitStat;
    public ModifierAppliedTo modifierAppliedTo; // current/max stat
    public ModifierScope modifierScope; // self, party, friendly, enemy, etc. Used by items
    public int modifierPower;
    public ModifierCalculatedHow modifierCalculatedHow; // add, mult, percent
    public int duration;
    // [EnumFlag]
    public UnitStatus[] canBeAppliedToTheUnitsWithStatuses;
    public PowerSource powerSource = PowerSource.None; // required for: Unit Ability, consumable damaging Items
    // For UnitAbility:
    public int powerIncrementOnStatsUpgrade;
    // For UnitSkill
    public int skillPowerMultiplier = 1; // consider removing this parameter
}