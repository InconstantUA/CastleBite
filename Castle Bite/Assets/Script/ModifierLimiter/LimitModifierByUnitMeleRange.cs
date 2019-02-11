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
    // cache mele unit blocking condition for OnLimitMessage
    private MeleUnitBlockingCondition meleUnitBlockingConditionCache = MeleUnitBlockingCondition.None;

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

    public override bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // verify if source or destination context do not match requirements of this limiter
        if (!DoesContextMatch(srcContext, dstContext))
        {
            // context is not in scope of this limiter
            // don't limit
            return false;
        }
        // get and cache mele unit blocking condition
        meleUnitBlockingConditionCache = GetMeleUnitBlockingCondition(srcContext, dstContext);
        // verify if mele blocking condition has been triggered
        if (MeleUnitBlockingCondition.None == meleUnitBlockingConditionCache)
        {
            // destination cell can be reached - dont' limit modifier
            return false;
        }
        // Default: destination cell is not reachable - discard modifier
        return true;
    }

    public override string OnLimitMessage
    {
        get
        {
            switch (meleUnitBlockingConditionCache)
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
                    Debug.LogError("Unknown MeleUnitBlockingCondition: " + meleUnitBlockingConditionCache);
                    return onTryToAttackNonBlockedEnemyMessage;
            }
        }
    }
}
