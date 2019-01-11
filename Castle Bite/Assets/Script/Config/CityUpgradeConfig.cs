using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityUpgradeConfig", menuName = "Config/City/Upgrade Config")]
public class CityUpgradeConfig : ScriptableObject
{
    public int minCityLevel = 1;
    public int maxCityLevel = 5;
    public int capitalCityLevel = 6;
    public int[] cityUpgradeCostPerCityLevel;
    public int[] cityGoldIncomePerCityLevel;
    public int[] cityManaIncomePerCityLevel;
}