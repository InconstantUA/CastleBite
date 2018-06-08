using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroParty : MonoBehaviour {
    [SerializeField]
    Faction faction;
    
    public Faction GetFaction()
    {
        return faction;
    }

    public void SetFaction(Faction value)
    {
        faction = value;
    }

    // it is created just to be able to find it in city using
    // GetComponentInChildren<HeroParty>() function
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
