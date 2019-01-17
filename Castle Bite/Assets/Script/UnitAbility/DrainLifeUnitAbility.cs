using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Drain Life", menuName = "Config/Unit/Abilities/Drain Life")]
public class DrainLifeUnitAbility: UnitAbility
{
    public override void Apply(PartyUnit activeUnit, PartyUnit destinationUnit)
    {
        Debug.Log("Applying " + GetType().Name + " from " + activeUnit.UnitName + " to " + destinationUnit.UnitName);
        // Get damage dealt
        int damageDealt = destinationUnit.GetAbilityDamageDealt(activeUnit);
        // Apply damage dealt to the destination unit
        Debug.Log("Deal " + Math.Abs(damageDealt) + " damage to " + destinationUnit.UnitName);
        destinationUnit.UnitHealthCurr += damageDealt; // damageDealt is negative
        //// verify if event has been set
        //if (gameEvent != null)
        //{
        //    gameEvent.Raise(destinationUnit.gameObject, this);
        //}
    }
}