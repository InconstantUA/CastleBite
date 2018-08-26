using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxDisplayCurrentGoldValue : MonoBehaviour {

    void OnEnable ()
    {
        UpdateGoldValue();
    }
	
	// this called by event admin
	public void UpdateGoldValue ()
    {
        // verify if there is active player present
        if (TurnsManager.Instance.GetActivePlayer())
            // update gold value in UI
            GetComponent<Text>().text = TurnsManager.Instance.GetActivePlayer().PlayerGold.ToString();
    }
}
