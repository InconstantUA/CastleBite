using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Config/Achievement")]
public class AchievementConfig : ScriptableObject
{
    public Achievement achievement;
    public string displayName;
    [TextArea]
    public string description;
}
