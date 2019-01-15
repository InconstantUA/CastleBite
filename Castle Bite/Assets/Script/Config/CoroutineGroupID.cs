using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game/Coroutine/GroupID")]
public class CoroutineGroupID : ScriptableObject
{
    public string GUID
    {
        get
        {
            // .. note: this might be slow
            Debug.Log("CoroutineGroupID path is " + AssetDatabase.GetAssetPath(GetInstanceID()));
            // return asset GUID from this asset path
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(GetInstanceID()));
        }
    }
}
