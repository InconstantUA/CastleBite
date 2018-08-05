using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Faction { Dominion, Greenskin };

public class Relationships : MonoBehaviour {
    public enum State { Allies, AtWar, Neutral, SameFaction };
    public static Relationships Instance { get; private set; }

    void Awake()
    {
        // verify if instance already initialized
        if (Instance == null)
        {
            // prevent this game object (game options) to be destroyed on scenes change
            // this only works on root objects in a scene, so disabled for now
            // DontDestroyOnLoad(gameObject);
            // initialize instance with this game object
            Instance = this;
        }
        // verify if instance were instantiated by some other scene, when there is already instance present
        else if (Instance != this)
        {
            // destroy this instance of game options to keep only one instance
            Destroy(gameObject);
        }
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
