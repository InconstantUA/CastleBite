using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { Dominion, Greenskin };

public class Relationships : MonoBehaviour {
    public enum State { Allies, AtWar, Neutral, SameFaction };
    public static Relationships Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
		// set relationships between factions
	}

    public State GetRelationships(Faction faction1, Faction faction2)
    {
        if (faction1 == faction2)
        {
            return State.SameFaction;
        } else
        {
            // Dominion and Greenskin are at war
            if ( (Faction.Dominion == faction1) && (Faction.Greenskin == faction2) ) {
                return State.AtWar;
            }
        }
        // default state:
        return State.Neutral;
    }
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
