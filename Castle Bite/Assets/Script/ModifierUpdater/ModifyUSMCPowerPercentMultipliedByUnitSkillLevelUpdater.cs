using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Updaters/ModifyUSMCPowerPercentMultipliedByUnitSkillLevelUpdater")]
public class ModifyUSMCPowerPercentMultipliedByUnitSkillLevelUpdater : ModifierConfigUpdater
{
    public UnitSkillID associatedUnitSkillID;
    public float percentToBase;

    // default update
    public override UnitStatModifierConfig GetUpdatedUSMC(UnitStatModifierConfig unitStatModifierConfig, GameObject gameObject)
    {
        // get party unit from gameObject
        PartyUnit partyUnit = gameObject.GetComponent<PartyUnit>();
        // verify if party unit is not null
        if (partyUnit != null)
        {
            // copy current USM config (to not make changes on default one)
            UnitStatModifierConfig newUSMConfig = Instantiate(unitStatModifierConfig);
            // get party unit stats upgrade count
            int skillLevel = partyUnit.GetUnitSkillData(associatedUnitSkillID).currentSkillLevel;
            // update to current USM config power based on calculation type
            newUSMConfig.modifierPower = Mathf.RoundToInt(newUSMConfig.modifierPower + newUSMConfig.modifierPower * skillLevel * percentToBase / 100f);
            // return new USM config
            return newUSMConfig;
        }
        else
        {
            Debug.LogError("Party Unit reference is null");
        }
        return unitStatModifierConfig;
    }
}
