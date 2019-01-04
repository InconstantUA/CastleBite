using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/StatModifier")]
public class UnitStatModifierConfig : ScriptableObject
{
    public UnitStatModifierType unitStatModifierType; // used in UnitStatModifierData, which is used when modifier is applied to other units from unit ability, for example: paralyze
    public UnitStat unitStat;
    public ModifierAppliedTo modifierAppliedTo;
    public ModifierScope modifierScope;
    public int modifierPower;
    public int skillPowerMultiplier = 1;
    public ModifierCalculatedHow modifierCalculatedHow;
    public int duration;
    [EnumFlag]
    public UnitStatuses canBeAppliedToTheUnitsWithStatuses;
}
