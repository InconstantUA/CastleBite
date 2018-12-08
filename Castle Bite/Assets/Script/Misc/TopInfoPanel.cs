using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopInfoPanel : MonoBehaviour {
    
    public void UpdateInfo()
    {
        GetComponentInChildren<TextBoxDisplayCurrentGoldValue>(true).UpdateGoldValue();
    }

}
