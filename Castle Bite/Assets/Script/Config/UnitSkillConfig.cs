using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitSkill", menuName = "Config/Unit Skill")]
public class UnitSkillConfig : ScriptableObject
{
    public UnitSkill unitSkill;
    public string skillDisplayName;
    public int maxSkillLevel;
    public int requiredHeroLevel;
    public int levelUpEveryXHeroLevels; // skill can be learned only after this number of levels has passed
    [TextArea]
    public string description;
}
