using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/StatModifier")]
public class UnitStatModifierConfig : ScriptableObject
{
    public UnitStatModifierType unitStatModifierType;
    public UnitStat unitStat;
    public ModifierAppliedTo modifierAppliedTo;
    public ModifierScope modifierScope;
    public int modifierPower;
    public int skillPowerMultiplier = 1;
    public ModifierCalculatedHow modifierCalculatedHow;
    public ModifierAppliedHow modifierAppliedHow;
    public int duration;
    [EnumFlag]
    public UnitStatuses canBeAppliedToTheUnitsWithStatuses;
}
