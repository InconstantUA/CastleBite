using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When this class is called it requires special objects to be set
// This is later will be used by other sub-scripts
public class HireUnitGeneric : MonoBehaviour {
    public bool isHigheredUnitPartyLeader;
    public Transform newUnitParent;
    public GameObject callerObjectToDisableOnHire;
    GameObject unitsPanel;

    public void DeactivateAdv()
    {
        // delete units panel, because later it may change to other
        Destroy(unitsPanel);
        gameObject.SetActive(false);
    }

    public void ActivateAdv(bool isHUPL, Transform nUP, GameObject cOTDOH, GameObject uP)
    {
        isHigheredUnitPartyLeader = isHUPL;
        newUnitParent = nUP;
        callerObjectToDisableOnHire = cOTDOH;
        gameObject.SetActive(true);
        // create new units panel
        unitsPanel = Instantiate(uP, transform.Find("Panel"));
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
