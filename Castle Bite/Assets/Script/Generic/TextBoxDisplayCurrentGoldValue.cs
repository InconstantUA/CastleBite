using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxDisplayCurrentGoldValue : MonoBehaviour {
    Text txt;

    // Use this for initialization
    void Start () {
        txt = gameObject.GetComponent<Text>();
        txt.text = TurnsManager.Instance.GetActivePlayer().PlayerGold.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        txt.text = TurnsManager.Instance.GetActivePlayer().PlayerGold.ToString();
    }
}
