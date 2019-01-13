using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerConfig", menuName = "Config/Unit/Power")]
public class UnitPowerConfig : ScriptableObject
{
    public string displayName;
    public string description;
    public List<UnitStatModifierConfig> unitStatModifierConfigs;
}
