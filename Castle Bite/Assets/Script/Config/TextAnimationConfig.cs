using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextAnimationConfig", menuName = "Config/Text/AnimationConfig")]
public class TextAnimationConfig : ScriptableObject
{
    public Color textColor;
    public float duration;
    public CoroutineGroupID coroutineGroupID;
}
