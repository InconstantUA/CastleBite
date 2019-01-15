using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Healing Unit Ability", menuName = "Config/Unit/Abilities/Base/Healing")]
public class BaseHealingUnitAbility : UnitAbility
{
    public override void Apply(PartyUnit activeUnit, PartyUnit destinationUnit)
    {
        Debug.Log("Applying " + GetType().Name + " Base Healing from " + activeUnit.UnitName + " to " + destinationUnit.UnitName);
        // Get heal amount
        int healAmount = activeUnit.GetUnitEffectivePower();
        // Heal destination unit
        Debug.Log("Heal " + destinationUnit.UnitName + " for " + healAmount + " health");
        destinationUnit.UnitHealthCurr += healAmount;
        // verify if event has been set
        if (gameEvent != null)
        {
            gameEvent.Raise(destinationUnit.gameObject, this);
        }
    }
}