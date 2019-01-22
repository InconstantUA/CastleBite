using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/Abilities/Defense Default Ability")]
public class DefenseUnitAbility : UnitAbility
{
    public override void Apply(PartyUnit activeUnit, PartyUnit destinationUnit)
    {
        // Note: this is never triggered because we call UPM in this ability manually via SetDefenseBuffActive() in PartyUnitUI.cs
        // We need this to be defined as ability to set UniquePowerModifierID
        Debug.Log("Applying defense default unit ability to " + destinationUnit.UnitName);
    }
}