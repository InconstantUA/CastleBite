using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Ability", menuName = "Config/Unit/Ability")]
public class UnitAbilityConfig : ScriptableObject
{
    public UnitAbility unitAbility;
    public string abilityDisplayName;
    public string abilityDescription;
    public UnitPowerDistance unitPowerDistance;
    public UnitPowerScope unitPowerScope;
    public UnitStatModifierConfig unitStatModifierConfig;
}
