using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UniqueAbility", menuName = "Config/Player/Unique Ability")]
public class UniqueAbilityConfig : ScriptableObject
{
    public PlayerUniqueAbility playerUniqueAbility;
    public string displayName;
    public bool doLimitUsageByFaction;
    public Faction[] applicableToFactions;
    public string description;
    public int level;
    public int currentLearningPoints;
    public UnitStatModifier unitStatModifier;
    public UnitStatModifierConfig unitStatModifierConfig; // duration left is not applicable

    public int GetLearningPointsRequiredToReachNextLevel ()
    {
        Debug.LogWarning("Write formula here .. ");
        return 10;
    }
}
