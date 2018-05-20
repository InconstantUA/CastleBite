using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyLeader : MonoBehaviour {
    public int cost;

    public enum HeroType { Knight, Ranger, Archmage, Archangel, Thief, Unknown};
    public HeroType heroType;

    public void SetCost(int requiredCost)
    {
        cost = requiredCost;
    }

    public int GetCost()
    {
        return cost;
    }

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

}
