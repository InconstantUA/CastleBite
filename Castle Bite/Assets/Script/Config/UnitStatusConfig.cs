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
    public bool canBeGivenATurnInBattle;
    [SerializeField]
    [ReadOnly]
    bool canBeAttackedIfFriendly = false;
    [SerializeField]
    [ReadOnly]
    bool canBeAttackedIfEnemyAndBlocked = false;
    [SerializeField]
    bool canBeAttackedIfEnemyAndNotBlocked;

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
