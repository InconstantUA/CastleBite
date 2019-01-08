using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatus", menuName = "Config/Unit/Status")]
public class UnitStatusConfig : ScriptableObject
{
    public UnitStatus unitStatus;
    [SerializeField]
    bool canBeResurectedFriendly;
    [SerializeField]
    bool canBeResurectedEnemy;
    [SerializeField]
    [ReadOnly]
    bool canBeHealedFriendlyWitFullHealth = false;
    [SerializeField]
    bool canBeHealedFriendlyWithNotFullHealth;
    [SerializeField]
    bool canBeHealedEnemy;
    [SerializeField]
    bool canBeGivenATurnInBattleDuringMainPhase;
    [SerializeField]
    bool canBeGivenATurnInBattleDuringPostWaitPhase;
    [SerializeField]
    [ReadOnly]
    bool canBeAttackedIfFriendly = false;
    [SerializeField]
    [ReadOnly]
    bool canBeAttackedIfEnemyAndBlocked = false;
    [SerializeField]
    bool canBeAttackedIfEnemyAndNotBlocked;

    public bool GetCanBeGivenATurnInBattle()
    {
        // return true if unit can be given a turn in batle during main or post-wait phase
       return canBeGivenATurnInBattleDuringMainPhase || canBeGivenATurnInBattleDuringPostWaitPhase;
    }

    public bool GetCanBeGivenATurnInBattle(BattleScreen.TurnPhase turnPhase)
    {
        switch (turnPhase)
        {
            case BattleScreen.TurnPhase.Main:
                return canBeGivenATurnInBattleDuringMainPhase;
            case BattleScreen.TurnPhase.PostWait:
                return canBeGivenATurnInBattleDuringPostWaitPhase;
            default:
                Debug.LogError("Unknown turn phase: " + turnPhase);
                return false;
        }
    }

    public bool GetCanBeResurected(bool isFriendly)
    {
        if (isFriendly)
        {
            return canBeResurectedFriendly;
        }
        else
        {
            return canBeResurectedEnemy;
        }
    }

    public bool GetCanBeHealed(bool isFriendly, bool healthIsFull)
    {
        if (isFriendly)
        {
            if (healthIsFull)
            {
                // HARDCODED: it is not possible to heal unit with full health
                return canBeHealedFriendlyWitFullHealth;
            }
            else
            {
                return canBeHealedFriendlyWithNotFullHealth;
            }
        }
        else
        {
            return canBeHealedEnemy;
        }
    }

    public bool GetCanBeAttacked(bool isFriendly, MeleUnitBlockingCondition meleUnitBlockingCondition = MeleUnitBlockingCondition.None)
    {
        if (isFriendly)
        {
            return canBeAttackedIfFriendly;
        }
        else
        {
            // verify if there is no blocking condition
            if (MeleUnitBlockingCondition.None == meleUnitBlockingCondition)
            {
                return canBeAttackedIfEnemyAndNotBlocked;
            }
            else
            {
                return canBeAttackedIfEnemyAndBlocked;
            }
        }
    }
}
