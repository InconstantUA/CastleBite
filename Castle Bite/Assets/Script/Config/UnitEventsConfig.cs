using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitPropertyEvents
{
    public GameEvent HasChanged;
}

[CreateAssetMenu(menuName = "Config/Unit/Events")]
public class UnitEventsConfig : ScriptableObject
{
    public UnitPropertyEvents unitStatus;
    public UnitPropertyEvents unitHealthCurr;
    public UnitPropertyEvents unitHealthMax;
}
