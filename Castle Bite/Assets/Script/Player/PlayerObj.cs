using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : MonoBehaviour {

    [SerializeField]
    int totalGold;
    [SerializeField]
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetTotalGold()
    {
        return totalGold;
    }

    public void SetTotalGold(int newTotalGoldValue)
    {
        totalGold = newTotalGoldValue;
        // Trigger gold update event to update gold value in UI
    }
}
