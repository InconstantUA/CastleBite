using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionSelection : MonoBehaviour {
    Faction faction;

    public Faction Faction
    {
        get
        {
            return faction;
        }

        set
        {
            faction = value;
        }
    }
}
