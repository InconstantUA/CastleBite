using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadGameConfig", menuName = "Config/Game/LoadConfig")]
public class LoadGameConfig : ScriptableObject
{
    public string loadingGameTextString = "Loading game ...";
    public int loadingScreenExplicitDelaySeconds = 1;
}
