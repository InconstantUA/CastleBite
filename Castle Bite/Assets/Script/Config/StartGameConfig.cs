using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StartGameConfig", menuName = "Config/Game/StartConfig")]
public class StartGameConfig : ScriptableObject
{
    public string startingGameTextString = "Starting game ...";
    public int startingScreenExplicitDelaySeconds = 1;
}
