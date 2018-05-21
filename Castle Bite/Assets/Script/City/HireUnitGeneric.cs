using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When this class is called it requires special objects to be set
// This is later will be used by other sub-scripts
public class HireUnitGeneric : MonoBehaviour {
    bool isHigheredUnitPartyLeader;
    Transform newUnitParent;
    GameObject callerObjectToDisableOnHire;
    GameObject unitsPanel;

    public bool GetisHigheredUnitPartyLeader()
    {
        return isHigheredUnitPartyLeader;
    }
    public Transform GetnewUnitParent()
    {
        return newUnitParent;
    }
    public GameObject GetcallerObjectToDisableOnHire()
    {
        return callerObjectToDisableOnHire;
    }

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
        // and bring it to the front
        transform.SetAsLastSibling();
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
