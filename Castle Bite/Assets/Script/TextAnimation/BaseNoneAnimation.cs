using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Config/Text/Animations/Base/NoneAnimation")]
public class BaseNoneAnimation : TextAnimation
{
    public override void Run(Text text)
    {
        // don't do anything
    }
}
