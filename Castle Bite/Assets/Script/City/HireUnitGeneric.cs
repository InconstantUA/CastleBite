using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// When this class is called it requires special objects to be set
// This is later will be used by other sub-scripts
public class HireUnitGeneric : MonoBehaviour {
    [SerializeField]
    GameObject unitUIToggleTemplate;
    [SerializeField]
    ToggleGroup unitsToHireList;
    [SerializeField]
    PlayerIncomeInfo playerIncomeInfo;
    Transform callerCell;
    GameObject unitsPanel;

    public GameObject UnitUIToggleTemplate
    {
        get
        {
            return unitUIToggleTemplate;
        }
    }

    public ToggleGroup UnitsToHireList
    {
        get
        {
            return unitsToHireList;
        }
    }

    void SetCityControlsActive(bool doActivate)
    {
        // Activate city controls if city view is active
        if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>() != null)
        {
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Heal").gameObject.SetActive(doActivate);
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Resurect").gameObject.SetActive(doActivate);
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Dismiss").gameObject.SetActive(doActivate);
            transform.root.Find("MiscUI/BottomControlPanel/RightControls/CityBackButton").gameObject.SetActive(doActivate);
        }
    }

    void SetHireUnitMenuButtonsActive(bool doActivate)
    {
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/HireUnitBtn").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/RightControls/CloseHireUnitMenuBtn").gameObject.SetActive(doActivate);
    }

    void SetBackgroundActive(bool doActivate)
    {
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(doActivate);
    }

    void RemoveAllCurrentUnitsToHire()
    {
        // remove all toggles for units created from template
        foreach (Transform tr in unitsToHireList.transform)
        {
            Destroy(tr.gameObject);
        }
    }

    void OnDisable()
    {
        // remove all toggles for units created from template
        RemoveAllCurrentUnitsToHire();
        // Deactivate controls
        SetHireUnitMenuButtonsActive(false);
        // check if gold info panel should be enabled
        if (GameOptions.Instance.mapUIOpt.togglePlayerIncome == 0)
        {
            // Deactivate top gold info panel if it was disabled before activation
            playerIncomeInfo.SetActive(false);
        }
        // Deactivate unit info panel
        transform.root.Find("MiscUI").GetComponentInChildren<UnitInfoPanel>(true).gameObject.SetActive(false);
        // Deactivate intermediate background
        SetBackgroundActive(false);
        // Activate city controls
        SetCityControlsActive(true);
    }

    public void SetActive(UnitType[] unitTypesToHire, Transform destinationCellTr, UnitHirePanel.Mode mode = UnitHirePanel.Mode.Normal)
    {
        // Note: order is important, because of bring to front control
        // Activate intermediate background (bring to front is triggered automatically)
        SetBackgroundActive(true);
        // activate this object
        gameObject.SetActive(true);
        // save destination cell for later use
        callerCell = destinationCellTr;
        // get templates manager
        // TemplatesManager templatesManager = transform.root.Find("Templates").GetComponent<TemplatesManager>();
        // get unit UI toggles list parent
        // Transform togglesListTransform = transform.Find("UnitsToHire");
        // get unit toggle UI template
        // GameObject unitUIToggleTemplate = transform.Find("UnitTemplate").gameObject;
        // reset flag for first toggle activation
        bool firstToggleIsActivated = false;
        // remove all previously configured units to hire
        RemoveAllCurrentUnitsToHire();
        // create menu entry for each unit which needs to be hired
        foreach (UnitType unitType in unitTypesToHire)
        {
            // create menu entry from template
            UnitHirePanel newUnitToggle = Instantiate(unitUIToggleTemplate, unitsToHireList.transform).GetComponent<UnitHirePanel>();
            // set toggle group
            newUnitToggle.GetComponent<Toggle>().group = unitsToHireList;
            // set unit to hire
            newUnitToggle.UnitToHire = TemplatesManager.Instance.GetPartyUnitTemplateByType(unitType).GetComponent<PartyUnit>();
            // set mode
            newUnitToggle.PanelMode = mode;
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
                transform.root.Find("MiscUI").GetComponentInChildren<UnitInfoPanel>(true).ActivateAdvance(newUnitToggle.UnitToHire, UnitInfoPanel.Align.Right, false);
            }
        }
        // Force layout update
        // Note: this should be done to force all fields to be adjusted to the text size
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)unitsToHireList.transform);
        // verify if mode is not hire first hero
        if (mode != UnitHirePanel.Mode.FirstUnit)
        {
            // Activate controls
            SetHireUnitMenuButtonsActive(true);
            // Activate top gold info panel
            playerIncomeInfo.SetActive(true);
        }
        // DeActivate city controls
        SetCityControlsActive(false);
    }

    //public void ActivateAdv(Transform cCell, GameObject unitsPanelTemplate)
    //{
    //    callerCell = cCell;
    //    gameObject.SetActive(true);
    //    // and bring it to the front
    //    transform.SetAsLastSibling();
    //    // create new units panel
    //    unitsPanel = Instantiate(unitsPanelTemplate, transform.Find("Panel"));
    //    // turn it on
    //    unitsPanel.SetActive(true);
    //    // change position
    //    unitsPanel.transform.localPosition = new Vector3(0, 20, 0);
    //}

    UnitType GetSelectedUnitType()
    {
        //  Find selected toggle and get attached to it unit template
        // get unit UI toggles list parent
        foreach (Toggle toggle in unitsToHireList.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                return toggle.GetComponent<UnitHirePanel>().UnitToHire.UnitType;
            }
        }
        // if nothing found then return null
        Debug.LogError("Selected unit is not found");
        return UnitType.Unknown;
    }

    // Called from UI button
    public void HireSelectedUnit()
    {
        // Ask City to Hire unit
        //callerCity.HireUnit(callerCell, GetSelectedUnitType());
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().HireUnit(callerCell, GetSelectedUnitType());
        // Deactivate this hire unit panel
        gameObject.SetActive(false);
    }

}
