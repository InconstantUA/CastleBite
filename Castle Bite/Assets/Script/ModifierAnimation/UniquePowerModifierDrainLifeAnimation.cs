using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Animations/Drain Life")]
public class UniquePowerModifierDrainLifeAnimation : UniquePowerModifierAnimation
{
    [SerializeField]
    TextAnimation sourceUnitTextAnimation;
    [SerializeField]
    TextAnimation destinationUnitTextAnimation;

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if active and destination units are set
            if (BattleContext.ActivePartyUnitUI != null && BattleContext.DestinationUnitSlot != null)
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
            // get destination unit party unit UI
            PartyUnitUI activePartyUnitUI = BattleContext.ActivePartyUnitUI;
            // verify if there is a party unit UI
            if (activePartyUnitUI != null)
            {
                Debug.Log("Run Heal Animation on active unit");
                // run text animation
                sourceUnitTextAnimation.Run(activePartyUnitUI.UnitInfoPanelText);
            }
            // get destination unit party unit UI
            PartyUnitUI destinationPartyUnitUI = BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>();
            // verify if there is a party unit UI
            if (destinationPartyUnitUI != null)
            {
                Debug.Log("Run Damage Animation on destination unit");
                // run text animation
                destinationUnitTextAnimation.Run(destinationPartyUnitUI.UnitInfoPanelText);
            }
        }
    }
}
