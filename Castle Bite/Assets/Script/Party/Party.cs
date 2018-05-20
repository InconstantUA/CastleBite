using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour {
    public PartyUnit[] party;

    // Use this for initialization
    void Start () {
        // there can be no more than 6 units in a party, so init it with this size
        party = new PartyUnit[6];
    }
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
