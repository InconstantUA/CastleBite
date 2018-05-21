using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHirePanel : MonoBehaviour {
    public PartyUnit unitToHire;

    // Use this for initialization
    void Start() {
        // populate panel with information from attached unitToHire
        transform.Find("Name").GetComponent<Text>().text = "[" + unitToHire.GetUnitName() + "]";
        transform.Find("CharacteristicsValues").GetComponent<Text>().text = unitToHire.GetCost().ToString() + "\r\n" + unitToHire.GetLeadership().ToString() + "\r\n" + unitToHire.GetRole();
    }

    public PartyUnit GetUnitToHire()
    {
        return unitToHire;
    }
    //// Update is called once per frame
    //void Update () {

    //}
}
