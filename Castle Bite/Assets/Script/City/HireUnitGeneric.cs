using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// When this class is called it requires special objects to be set
// This is later will be used by other sub-scripts
public class HireUnitGeneric : MonoBehaviour {
    Transform callerCell;
    GameObject unitsPanel;

    void OnDisable()
    {
        // remove all toggles for units created from template
        foreach (Transform tr in transform.Find("UnitsToHire").transform)
        {
            Destroy(tr.gameObject);
        }
        // Deactivate controls
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/HireUnitBtn").gameObject.SetActive(false);
        transform.root.Find("MiscUI/BottomControlPanel/RightControls/CloseHireUnitMenuBtn").gameObject.SetActive(false);
        // Deactivate top gold info panel
        transform.root.Find("MiscUI/TopInfoPanel/Middle/CurrentGold").gameObject.SetActive(false);
        // Deactivate unit info panel
        transform.root.Find("MiscUI").GetComponentInChildren<UnitInfoPanel>(true).gameObject.SetActive(false);
        // Deactivate intermediate background
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(false);
        // Activate city controls if city view is active
        if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>() != null)
        {
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Heal").gameObject.SetActive(true);
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Resurect").gameObject.SetActive(true);
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Dismiss").gameObject.SetActive(true);
            transform.root.Find("MiscUI/BottomControlPanel/RightControls/CityBackButton").gameObject.SetActive(true);
        }
    }

    public void SetActive(UnitType[] unitTypesToHire, Transform destinationCellTr, City destinationCity)
    {
        // Note: order is important, because of bring to front control
        // Activate intermediate background (bring to front is triggered automatically)
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(true);
        // activate this object
        gameObject.SetActive(true);
        // save destination cell for later use
        callerCell = destinationCellTr;
        // get templates manager
        TemplatesManager templatesManager = transform.root.Find("Templates").GetComponent<TemplatesManager>();
        // get unit UI toggles list parent
        Transform togglesListTransform = transform.Find("UnitsToHire");
        // get unit toggle UI template
        GameObject unitUIToggle = transform.Find("UnitTemplate").gameObject;
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
                // Activate Unit info panel with the first unit info
                transform.root.Find("MiscUI").GetComponentInChildren<UnitInfoPanel>(true).ActivateAdvance(newUnitToggle.unitToHire, UnitInfoPanel.Align.Right, false);
            }
        }
        // Activate controls
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/HireUnitBtn").gameObject.SetActive(true);
        transform.root.Find("MiscUI/BottomControlPanel/RightControls/CloseHireUnitMenuBtn").gameObject.SetActive(true);
        // Activate top gold info panel
        transform.root.Find("MiscUI/TopInfoPanel/Middle/CurrentGold").gameObject.SetActive(true);
        // Deactivate city controls if city view is active
        if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>() != null)
        {
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Heal").gameObject.SetActive(false);
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Resurect").gameObject.SetActive(false);
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Dismiss").gameObject.SetActive(false);
            transform.root.Find("MiscUI/BottomControlPanel/RightControls/CityBackButton").gameObject.SetActive(false);
        }
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
        foreach (Toggle toggle in transform.Find("UnitsToHire").GetComponentsInChildren<Toggle>())
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
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().HireUnit(callerCell, GetSelectedUnitType());
        // Deactivate this hire unit panel
        gameObject.SetActive(false);
    }

}
