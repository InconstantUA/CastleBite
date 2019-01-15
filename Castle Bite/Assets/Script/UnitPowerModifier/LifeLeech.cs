using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LifeLeech", menuName = "Config/Unit/PowerModifiers/LifeLeech")]
public class LifeLeech : UnitPowerModifier
{
    public override void Apply(PartyUnit activeUnit, PartyUnit destinationUnit)
    {
        Debug.Log("Applying " + GetType().Name + " unit power modifier from " + activeUnit.UnitName + " to " + destinationUnit.UnitName);
        // Get damage dealt
        int damageDealt = destinationUnit.GetAbilityDamageDealt(activeUnit);
        // verify if damage dealt is not higher than current unit health
        if (Math.Abs(damageDealt) > destinationUnit.UnitHealthCurr)
        {
            // reset damage dealt to the current unit health
            damageDealt = destinationUnit.UnitHealthCurr;
        }
        // Heal active unit to amout of damage dealt
        Debug.Log("Heal " + activeUnit.UnitName + " for " + Math.Abs(damageDealt) + " health");
        activeUnit.UnitHealthCurr += Math.Abs(damageDealt);
        // verify if event has been set
        if (gameEvent != null)
        {
            gameEvent.Raise(activeUnit.gameObject, this);
        }
        //if (OnApply.GetPersistentEventCount() > 0)
        //{
        //    // Triggerevent
        //    Debug.Log("Calling OnApply for " + unitPowerModifierConfig.modifierName + " unit power modifier");
        //    OnApply.Invoke(this);
        //}
    }
}