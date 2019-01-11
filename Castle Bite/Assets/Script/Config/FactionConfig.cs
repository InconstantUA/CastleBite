using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction config", menuName = "Config/Faction")]
public class FactionConfig : ScriptableObject
{
    public Faction faction;
    public string factionDisplayName;
    public UnitType[] hireablePartyLeaders;
    public UnitType[] hireableCommonUnits;
}
