using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // Unique power modifiers
    public List<UniquePowerModifierConfig> postActionUniquePowerModifierConfigs;
    public List<UniquePowerModifierConfig> uniquePowerModifierConfigs;

    [NonSerialized]
    private UniquePowerModifierConfig primaryUniquePowerModifierConfig;
    [NonSerialized]
    private List<UniquePowerModifierConfig> uniquePowerModifierConfigsSortedByExecutionOrder;

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

    public UniquePowerModifierConfig PrimaryUniquePowerModifierConfig
    {
        get
        {
            // verify if primary upm is not set yet
            if (primaryUniquePowerModifierConfig == null)
            {
                // get UPM which has primary attribute set (should be only one)
                primaryUniquePowerModifierConfig = uniquePowerModifierConfigs.Find(e => e.IsPrimary == true);
            }
            return primaryUniquePowerModifierConfig;
            // return uniquePowerModifierConfigs.Find(e => e.IsPrimary == true); ;
        }
    }

    public List<UniquePowerModifierConfig> UniquePowerModifierConfigsSortedByExecutionOrder
    {
        get
        {
            // verify if uniquePowerModifierConfigsSortedByExecutionOrder is not set yet
            if (uniquePowerModifierConfigsSortedByExecutionOrder == null || uniquePowerModifierConfigsSortedByExecutionOrder.Count == 0)
            {
                // set it
                Debug.LogWarning("Set UniquePowerModifierConfigsSortedByExecutionOrder");
                uniquePowerModifierConfigsSortedByExecutionOrder = uniquePowerModifierConfigs.OrderBy(o => o.ExecutionOrder).ToList();
                foreach(UniquePowerModifierConfig upmc in uniquePowerModifierConfigsSortedByExecutionOrder)
                {
                    Debug.LogWarning("UPMC " + upmc.DisplayName);
                }
            }
            // return uniquePowerModifierConfigs Sorted By Execution Order
            return uniquePowerModifierConfigsSortedByExecutionOrder;
        }
    }

}
