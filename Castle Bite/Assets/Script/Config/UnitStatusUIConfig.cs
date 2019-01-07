using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusUI", menuName = "Config/Unit/UIStatus")]
public class UnitStatusUIConfig : ScriptableObject
{
    public UnitStatus unitStatus;
    public string statusDisplayName;
    public Color statusTextColor        = Color.white;
    public Color currentHealthTextColor = Color.white;
    public Color maxHealthTextColor     = Color.white;
    public Color unitCanvasTextColor    = Color.white;
    // Messages normally used to display errors, can be empty, if applicable
    [SerializeField]
    string onTryToResurectFriendlyMessage;
    [SerializeField]
    string onTryToResurectEnemyMessage;
    [SerializeField]
    string onTryToHealFriendlyWithFullHealthMessage;
    [SerializeField]
    string onTryToHealFriendlyWithNotFullHealthMessage;
    [SerializeField]
    string onTryToHealEnemyMessage;
    [SerializeField]
    string onTryToAttackFriendlyMessage;
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

    public string GetOnTryToHealMessage(bool isFriendly, bool healthIsFull)
    {
        if (isFriendly)
        {
            if (healthIsFull)
            {
                return onTryToHealFriendlyWithFullHealthMessage;
            }
            else
            {
                return onTryToHealFriendlyWithNotFullHealthMessage;
            }
        }
        else
        {
            return onTryToHealEnemyMessage;
        }
    }

    public string GetOnTryToResurectMessage(bool isFriendly)
    {
        if (isFriendly)
        {
            return onTryToResurectFriendlyMessage;
        }
        else
        {
            return onTryToResurectEnemyMessage;
        }
    }

    public string GetOnTryToAttackMessage(bool isFriendly, MeleUnitBlockingCondition meleUnitBlockingCondition = MeleUnitBlockingCondition.None)
    {
        if (isFriendly)
        {
            return onTryToAttackFriendlyMessage;
        }
        else
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
    }
}
