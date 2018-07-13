using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateAdvance(PartyUnit partyUnit)
    {
        // Activate this object
        gameObject.SetActive(true);
        // Activate unit info panel
        Transform unitInfoPanelTr = transform.root.Find("MiscUI/UnitInfoPanel");
        UnitInfoPanel unitInfoPanel = unitInfoPanelTr.GetComponent<UnitInfoPanel>();
        unitInfoPanel.ActivateAdvance(partyUnit);
        // Adjust position of unit info panel, so it appears on the right side
        RectTransform unitInfoPanelRT = unitInfoPanelTr.GetComponent<RectTransform>();
        unitInfoPanelRT.offsetMin = new Vector2(370, 0); // left, bottom
        unitInfoPanelRT.offsetMax = new Vector2(0, 0); // -right, -top
        // Disable unit info panel background
        unitInfoPanelTr.Find("Background").gameObject.SetActive(false);
        // Prevent Unit info panel from reacting on right clicks and closing
        unitInfoPanelTr.GetComponent<UnitInfoPanel>().interactable = false;

    }

    void OnEnable()
    {
        // populate UI with information from unit
    }

    void OnDisable()
    {
        // Deactivate unit info panel
        Transform unitInfoPanelTr = transform.root.Find("MiscUI/UnitInfoPanel");
        unitInfoPanelTr.gameObject.SetActive(false);
        // Adjust position of unit info panel, so it appears on the middle of the screen
        RectTransform unitInfoPanelRT = unitInfoPanelTr.GetComponent<RectTransform>();
        unitInfoPanelRT.offsetMin = new Vector2(0, 0); // left, bottom
        unitInfoPanelRT.offsetMax = new Vector2(0, 0); // -right, -top
        // Enable unit info panel background
        unitInfoPanelTr.Find("Background").gameObject.SetActive(true);
        // Allow Unit info panel to react on right clicks and close
        unitInfoPanelTr.GetComponent<UnitInfoPanel>().interactable = true;
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        gameObject.SetActive(false);
    }

    public void Apply()
    {
        Debug.Log("Apply");
        gameObject.SetActive(false);
    }
}
