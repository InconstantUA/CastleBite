using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseYourFirstHero : MonoBehaviour {
    public GameObject unitsGroupToHire;

    // Use this for initialization
    void Start () {
        gameObject.GetComponentInChildren<HireUnitGeneric>().ActivateAdv(null, unitsGroupToHire);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
