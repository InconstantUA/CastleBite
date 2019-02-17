using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Range is Mele", menuName = "Config/Unit/UniquePowerModifiers/Limiters/Range is Mele")]
public class LimitModifierByUnitMeleRange : ModifierLimiter
{
    // note: this can be actually skipped
    [ReadOnly, SerializeField]
    UnitAbilityRange unitAbilityRange = UnitAbilityRange.Mele;
    [SerializeField]
    string onTryToAttackNonBlockedEnemyMessage;
    [SerializeField]
    string onTryToAttackEnemyIfMeleAttackerIsBlockedByFriendlyFrontRowUnitsMessage;
    [SerializeField]
    string onTryToAttackEnemyIfMeleAttackerAndTargetIsProtectedByFriendlyUnitsInFrontRowMessage;
    [SerializeField]
    string onTryToAttackEnemyIfMeleAttackerAndTargetIsProtectedByUnitAboveMessage;
    [SerializeField]
    string onTryToAttackEnemyIfMeleAttackerAndTargetIsProtectedByUnitBelowMessage;

    public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    {
        return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    }

    public bool DoesSourceContextMatch(System.Object srcContext)
    {
        if (srcContext is PartyPanelCell)
        {
            // match
            return true;
        }
        Debug.Log("Source context doesn't match");
        // doesn't match
        return false;
    }

    public bool DoesDestinationContextMatch(System.Object dstContext)
    {
        if (dstContext is PartyPanelCell)
        {
            // match
            return true;
        }
        Debug.Log("Destination context doesn't match");
        // doesn't match
        return false;
    }

    MeleUnitBlockingCondition GetMeleUnitBlockingCondition(System.Object srcContext, System.Object dstContext)
    {
        // set context to Inventory Item
        PartyPanelCell srcPartyPanelCell = (PartyPanelCell)srcContext;
        // set context to PartyPanelCell
        PartyPanelCell dstPartyPanelCell = (PartyPanelCell)dstContext;
        // get active mele unit blocking condition Cell targetUnitCell, Row targetUnitRow, Row activeMeleUnitRow
        return dstPartyPanelCell.GetComponentInParent<PartyPanel>().GetMeleUnitBlockingCondition(srcPartyPanelCell, dstPartyPanelCell);
    }

    ValidationResult DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // verify if source or destination context do not match requirements of this limiter
        if (!DoesContextMatch(srcContext, dstContext))
        {
            // context is not in scope of this limiter
            // don't limit
            return ValidationResult.Pass();
        }
        // get and cache mele unit blocking condition
        MeleUnitBlockingCondition meleUnitBlockingCondition = GetMeleUnitBlockingCondition(srcContext, dstContext);
        // verify if mele blocking condition has been triggered
        if (MeleUnitBlockingCondition.None == meleUnitBlockingCondition)
        {
            // destination cell can be reached - dont' limit modifier
            return ValidationResult.Pass();
        }
        // Default: destination cell is not reachable - discard modifier
        return ValidationResult.Discard(GetOnLimitMessage(meleUnitBlockingCondition));
    }

    string GetOnLimitMessage(MeleUnitBlockingCondition meleUnitBlockingCondition)
    {
        switch (meleUnitBlockingCondition)
        {
            case MeleUnitBlockingCondition.None:
                return onTryToAttackNonBlockedEnemyMessage;
            case MeleUnitBlockingCondition.AttackerIsBlockedByFriendlyFrontRowUnits:
                return onTryToAttackEnemyIfMeleAttackerIsBlockedByFriendlyFrontRowUnitsMessage;
            case MeleUnitBlockingCondition.TargetUnitIsProtectedByFriendlyUnitsInFrontRow:
                return onTryToAttackEnemyIfMeleAttackerAndTargetIsProtectedByFriendlyUnitsInFrontRowMessage;
            case MeleUnitBlockingCondition.TargetUnitIsProtectedByUnitAbove:
                return onTryToAttackEnemyIfMeleAttackerAndTargetIsProtectedByUnitAboveMessage;
            case MeleUnitBlockingCondition.TargetUnitIsProtectedByUnitBelow:
                return onTryToAttackEnemyIfMeleAttackerAndTargetIsProtectedByUnitBelowMessage;
            default:
                Debug.LogError("Unknown MeleUnitBlockingCondition: " + meleUnitBlockingCondition);
                return onTryToAttackNonBlockedEnemyMessage;
        }
    }

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if source and destination are set
            if (BattleContext.ActivePartyUnitUI != null && BattleContext.DestinationUnitSlot != null)
            {
                // context match
                return true;
            }
        }
        // by default context doesn't match
        return false;
    }

    public override ValidationResult DoDiscardModifierInContextOf(System.Object context)
    {
        // verify if context doesn't match requirements of this limiter
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this limiter
            // don't limit
            return ValidationResult.Pass();
        }
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if we need to discard modifier
            return DoDiscardModifierInContextOf(BattleContext.ActivePartyUnitUI.GetComponentInParent<PartyPanelCell>(), BattleContext.DestinationUnitSlot.GetComponentInParent<PartyPanelCell>());
        }
        // don't limit
        return ValidationResult.Pass();
    }

}
