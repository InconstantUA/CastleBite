﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxDisplayCurrentGoldValue : MonoBehaviour {
    PlayerObj player;
    Text txt;

    // Use this for initialization
    void Start () {
        player = transform.root.Find("PlayerObj").gameObject.GetComponent<PlayerObj>();
        txt = gameObject.GetComponent<Text>();
        txt.text = player.PlayerGold.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        txt.text = player.PlayerGold.ToString();
    }
}
