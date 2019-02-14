using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Animations/Heal")]
public class UniquePowerModifierBaseHealAnimation : UniquePowerModifierAnimation
{
    [SerializeField]
    TextAnimation textAnimation;

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if destination unit is set
            if (BattleContext.DestinationUnitSlot != null)
                // context match
                return true;
        }
        // by default context doesn't match
        return false;
    }

    public override void Run(System.Object context)
    {
        // verify if context doesn't match requirements of this animation
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this animation
            // don't run 
            return;
        }
        // verify if context matches battle context
        if (context is BattleContext)
        {
            Debug.Log("Run Heal Animation");
            // get destination unit party unit UI
            PartyUnitUI partyUnitUI = BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>();
            // verify if there is a party unit UI
            if (partyUnitUI != null)
            {
                // run text animation
                textAnimation.Run(partyUnitUI.UnitInfoPanelText);
            }
        }
    }
}
