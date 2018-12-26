using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorsConfig", menuName = "Config/Colors")]
public class ColorsConfig : ScriptableObject
{
    // list of colors to pick for players representation on the map
    public Color[] colors;
}
