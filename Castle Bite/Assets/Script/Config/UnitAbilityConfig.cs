using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Unit Ability", menuName = "Config/Unit/Ability")]
public class UnitAbilityConfig : ScriptableObject
{
    public UnitAbilityID unitAbilityID;
    public string abilityDisplayName;
    public string abilityDescription;
    public UnitAbilityRange unitAbilityRange;
    // public PowerSource powerSource;
    //public UnitPowerScope unitPowerScope;
    public List<UnitStatModifierConfig> unitStatModifierConfigs;
}
