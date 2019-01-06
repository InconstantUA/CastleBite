using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatus", menuName = "Config/Unit/Status")]
public class UnitStatusConfig : ScriptableObject
{
    public UnitStatus unitStatus;
    public string statusDisplayName;
    public Color statusTextColor        = Color.white;
    public Color currentHealthTextColor = Color.white;
    public Color maxHealthTextColor     = Color.white;
    public Color unitCanvasTextColor    = Color.white;
}
