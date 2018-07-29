using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When this class is called it requires special objects to be set
// This is later will be used by other sub-scripts
public class HireUnitGeneric : MonoBehaviour {
    Transform callerCell;
    GameObject unitsPanel;

    public Transform GetcallerCell()
    {
        return callerCell;
    }

    public void DeactivateAdv()
    {
        // delete units panel, because later it may change to other
        // for example: from hire party leader to hire unit
        Destroy(unitsPanel);
        gameObject.SetActive(false);
    }

    public void ActivateAdv(Transform cCell, GameObject unitsPanelTemplate)
    {
        callerCell = cCell;
        gameObject.SetActive(true);
        // and bring it to the front
        transform.SetAsLastSibling();
        // create new units panel
        unitsPanel = Instantiate(unitsPanelTemplate, transform.Find("Panel"));
        // turn it on
        unitsPanel.SetActive(true);
        // change position
        unitsPanel.transform.localPosition = new Vector3(0, 20, 0);
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
