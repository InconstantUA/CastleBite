using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// When this class is called it requires special objects to be set
// This is later will be used by other sub-scripts
public class HireUnitGeneric : MonoBehaviour {
    Transform callerCell;
    //City callerCity;
    GameObject unitsPanel;

    //public Transform GetCallerCell()
    //{
    //    return callerCell;
    //}

    //public City GetCallerCity()
    //{
    //    return callerCity;
    //}

    public void DeactivateAdv()
    {
        // remove all toggles for units created from template
        foreach (Transform tr in transform.Find("Panel/UnitsToHire").transform)
        {
            Destroy(tr.gameObject);
        }
        // delete units panel, because later it may change to other
        // for example: from hire party leader to hire unit
        //Destroy(unitsPanel);
        gameObject.SetActive(false);
    }

    public void ActivateHireUnitMenu(UnitType[] unitTypesToHire, Transform destinationCellTr, City destinationCity)
    {
        // save destination cell for later use
        callerCell = destinationCellTr;
        // save destination city for later use
        //callerCity = destinationCity;
        // get templates manager
        TemplatesManager templatesManager = transform.root.Find("Templates").GetComponent<TemplatesManager>();
        // get unit UI toggles list parent
        Transform togglesListTransform = transform.Find("Panel/UnitsToHire");
        // get unit toggle UI template
        GameObject unitUIToggle = transform.Find("Panel/UnitTemplate").gameObject;
        // reset flag for first toggle activation
        bool firstToggleIsActivated = false;
        // create menu entry for each unit which needs to be hired
        foreach (UnitType unitType in unitTypesToHire)
        {
            // create menu entry from template
            UnitHirePanel newUnitToggle = Instantiate(unitUIToggle, togglesListTransform).GetComponent<UnitHirePanel>();
            // set unit to hire
            newUnitToggle.unitToHire = templatesManager.GetPartyUnitTemplateByType(unitType).GetComponent<PartyUnit>();
            // activate toggle
            newUnitToggle.gameObject.SetActive(true);
            // set toggle selection state if this is first toggle
            if (! firstToggleIsActivated)
            {
                // activate first toggle in the list
                newUnitToggle.GetComponent<Toggle>().isOn = true;
                // set flag to true, so it does not trigger anymore
                firstToggleIsActivated = true;
            }
        }
        // activate this object
        gameObject.SetActive(true);
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

    UnitType GetSelectedUnitType()
    {
        //  Find selected toggle and get attached to it unit template
        // get unit UI toggles list parent
        foreach (Toggle toggle in transform.Find("Panel/UnitsToHire").GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                return toggle.GetComponent<UnitHirePanel>().GetUnitToHire().UnitType;
            }
        }
        // if nothing found then return null
        Debug.LogError("Selected unit is not found");
        return UnitType.Unknown;
    }

    public void HireSelectedUnit()
    {
        // Ask City to Hire unit
        //callerCity.HireUnit(callerCell, GetSelectedUnitType());
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<CityScreen>().HireUnit(callerCell, GetSelectedUnitType());
        // Deactivate hire unit panel
        DeactivateAdv();
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
