using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnsManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public PlayerObj GetActivePlayer()
    {
        // .. Fix
        return transform.root.Find("PlayerObj").gameObject.GetComponent<PlayerObj>();
    }
}
