using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Updaters/ModifyUSMCPowerWithCityLevelUpdater")]
public class ModifyUSMCPowerWithCityLevelUpdater : ModifierConfigUpdater
{
    public int[] powerPerCityLevel = new int[7];

    // default update
    public override UnitStatModifierConfig GetUpdatedUSMC(UnitStatModifierConfig unitStatModifierConfig, GameObject gameObject)
    {
        // get party unit from gameObject
        City city = gameObject.GetComponent<City>();
        // verify if party unit is not null
        if (city != null)
        {
            // copy current USM config (to not make changes on default one)
            UnitStatModifierConfig newUSMConfig = Instantiate(unitStatModifierConfig);
            // verify if city level config has been defined
            if (powerPerCityLevel.Length - 1 >= city.CityLevelCurrent)
            {
                // update to current USM config power based on the city current level
                newUSMConfig.modifierPower = powerPerCityLevel[city.CityLevelCurrent];
            }
            else
            {
                Debug.LogError("Config for city with [" + city.CityLevelCurrent + "] level has not been defined");
            }
            // return new USM config
            return newUSMConfig;
        }
        else
        {
            Debug.LogError("City reference is null");
        }
        return unitStatModifierConfig;
    }
}
