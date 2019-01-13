using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TextAnimation : ScriptableObject
{
    public TextAnimationConfig textAnimationConfig;
    public abstract void Run(Text text, CoroutineQueue coroutineQueue);
}
