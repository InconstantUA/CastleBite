using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUnit : MonoBehaviour {

    PartyUnit focusedPartyUnit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ActivateUnitInfoPanel(PartyUnit partyUnit)
    {
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

    void DeactivateUnitInfoPanel()
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

    void ShowInfo(string info)
    {
        transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(info);
    }

    #region Generic/UpgradePoints
    void SetUnitUpgradePointsUI()
    {
        transform.Find("Panel/Generic/UpgradePoints/Value").GetComponent<Text>().text = focusedPartyUnit.UnitUpgradePoints.ToString();
    }

    void SetUnitUpgradePointsInfo()
    {
        string info = "Each time common unit gets new level it gains 1 upgrade point, party leaders gain 2 points."
           + "\r\n" + "Decide where to spend upgrade points: to upgrade stats (health, damage), to upgrade unit class for common units or to learn new skills for party leaders.";
        transform.Find("Panel/Generic/UpgradePoints/Info").GetComponent<TextButton>().OnClick.AddListener(delegate { ShowInfo(info); });
    }

    void InitGenericInfo()
    {
        // Display Available upgrade points
        SetUnitUpgradePointsUI();
        SetUnitUpgradePointsInfo();
    }
    #endregion Generic/UpgradePoints

    #region StatsUpgrade
    void SetUnitStatPointsValueUI()
    {
        transform.Find("Panel/StatsUpgrade/StatPoints/Value").GetComponent<Text>().text = focusedPartyUnit.UnitClassPoints.ToString();
    }

    void SetUnitStatPointMinusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/StatsUpgrade/StatPoints/Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitStatPointPlusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/StatsUpgrade/StatPoints/Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitUpgradeStatsInfoUI()
    {
        string info = "Click [+] to spend 1 stat point to upgrade unit's health, power of ability, power of unique attack modifier."
           + "\r\n" + "Once applied, it is not possible to reclaim upgrade points back."
           + "\r\n" + "Click [-] to roll back changes.";
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Info").GetComponent<TextButton>().OnClick.AddListener(delegate { ShowInfo(info); });
    }

    void SetUnitUpgradeStatsMinusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitUpgradeStatsPlusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void InitStats()
    {
        // Display current number of unitStatPoints
        SetUnitStatPointsValueUI();
        // Disable minus option, it should be enabled only if user clicked on plus
        SetUnitStatPointMinusUIInteractable(false);
        // Verify if there are free upgrade points available
        if (focusedPartyUnit.UnitUpgradePoints >= 1)
        {
            // enable plus button
            SetUnitStatPointPlusUIInteractable(true);
        }
        else
        {
            // disable plus button
            SetUnitStatPointPlusUIInteractable(false);
        }
        // Init Upgrade stats info
        SetUnitUpgradeStatsInfoUI();
        // Disable minus option, it should be enabled only if user clicked on plus
        SetUnitUpgradeStatsMinusUIInteractable(false);
        // Verify if there are free upgrade stats points available
        if (focusedPartyUnit.UnitStatPoints >= 1)
        {
            // enable plus button
            SetUnitUpgradeStatsPlusUIInteractable(true);
        }
        else
        {
            // disable plus button
            SetUnitUpgradeStatsPlusUIInteractable(false);
        }
    }
    #endregion StatsUpgrade

    #region ClassUpgrade
    void SetClassUpgradeUIActive(bool doActivate)
    {
        transform.Find("Panel/ClassUpgrade").gameObject.SetActive(doActivate);
    }
    void SetUnitClassPointsValueUI()
    {
        transform.Find("Panel/ClassUpgrade/ClassPoints/Value").GetComponent<Text>().text = focusedPartyUnit.UnitClassPoints.ToString();
    }

    void SetUnitClassPointMinusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/ClassUpgrade/ClassPoints/Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitClassPointPlusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/ClassUpgrade/ClassPoints/Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitClassInfoUI()
    {
        string info = "Click [+] to spend 1 class point to upgrade unit's class."
           + "\r\n" + "Click [-] to roll back changes."
           + "\r\n" + "Some classes require specific unit level before unit can change class."
           + "\r\n" + "First change to lower level classes. Once applied, it is not possible to change it aftwards, also other class branches on the same level will be locked."
           + "\r\n" + "Right click on a class name to get additional information.";
        transform.Find("Panel/ClassUpgrade/ChooseClassUpgradePath/Info").GetComponent<TextButton>().OnClick.AddListener(delegate { ShowInfo(info); });
    }

    void SetUnitClassMinusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/ClassUpgrade/Class/Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitClassPlusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/ClassUpgrade/Class/Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void InitClasses()
    {
        // verify if unit's class is upgradable
        if (focusedPartyUnit.ClassIsUpgradable)
        {
            // Enable Class upgrade menu
            SetClassUpgradeUIActive(true);
            // Display current number of UnitClassPoints
            SetUnitClassPointsValueUI();
            // Disable minus option, it should be enabled only if user clicked on plus
            SetUnitClassPointMinusUIInteractable(false);
            // Verify if there are free upgrade points available
            if (focusedPartyUnit.UnitUpgradePoints >= 1)
            {
                // enable plus button
                SetUnitClassPointPlusUIInteractable(true);
            }
            else
            {
                // disable plus button
                SetUnitClassPointPlusUIInteractable(false);
            }
            // Init Upgrade stats info
            SetUnitClassInfoUI();
            // Set options for all classes:
            // ..
            // Disable minus option, it should be enabled only if user clicked on plus
            SetUnitClassMinusUIInteractable(false);
            // Verify if there are free upgrade stats points available
            if (focusedPartyUnit.UnitClassPoints >= 1)
            {
                // enable plus button
                SetUnitClassPlusUIInteractable(true);
            }
            else
            {
                // disable plus button
                SetUnitClassPlusUIInteractable(false);
            }
        }
        else
        {
            // Disable Class upgrade menu
            SetClassUpgradeUIActive(false);
        }
    }
    #endregion ClassUpgrade

    #region SkillsUpgrade
    void SetSkillsUpgradeUIActive(bool doActivate)
    {
        transform.Find("Panel/SkillsUpgrade").gameObject.SetActive(doActivate);
    }
    void SetUnitSkillPointsValueUI()
    {
        transform.Find("Panel/SkillsUpgrade/SkillPoints/Value").GetComponent<Text>().text = focusedPartyUnit.UnitSkillPoints.ToString();
    }

    void SetUnitSkillPointsMinusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/SkillsUpgrade/SkillPoints/Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitSkillPointsPlusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/SkillsUpgrade/SkillPoints/Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitSkillInfoUI()
    {
        string info = "Click [+] to spend 1 skill point to learn new skill or upgrade already known skill to a higher level, if this is applicable."
           + "\r\n" + "Click [-] to roll back changes."
           + "\r\n" + "Some skills require specific hero level, before hero can learn them."
           + "\r\n" + "Right click on a skill name to get additional information.";
        transform.Find("Panel/SkillsUpgrade/ChooseAndLearnSkills/Info").GetComponent<TextButton>().OnClick.AddListener(delegate { ShowInfo(info); });
    }

    void SetUnitSkillMinusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/SkillsUpgrade/SkillsRow1/Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetUnitSkillPlusUIInteractable(bool doActivate)
    {
        transform.Find("Panel/SkillsUpgrade/SkillsRow1/Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void InitSkills()
    {
        // verify if unit's can learn new skills
        if (focusedPartyUnit.CanLearnSkills)
        {
            // Enable Skill upgrade menu
            SetSkillsUpgradeUIActive(true);
            // Display current number of UnitSkillPoints
            SetUnitSkillPointsValueUI();
            // Disable minus option, it should be enabled only if user clicked on plus
            SetUnitSkillPointsMinusUIInteractable(false);
            // Verify if there are free upgrade points available
            if (focusedPartyUnit.UnitUpgradePoints >= 1)
            {
                // enable plus button
                SetUnitSkillPointsPlusUIInteractable(true);
            }
            else
            {
                // disable plus button
                SetUnitSkillPointsPlusUIInteractable(false);
            }
            // Init Upgrade stats info
            SetUnitSkillInfoUI();
            // Set options for all classes:
            // ..
            // Disable minus option, it should be enabled only if user clicked on plus
            SetUnitSkillMinusUIInteractable(false);
            // Verify if there are free upgrade stats points available
            if (focusedPartyUnit.UnitSkillPoints >= 1)
            {
                // enable plus button
                SetUnitSkillPlusUIInteractable(true);
            }
            else
            {
                // disable plus button
                SetUnitSkillPlusUIInteractable(false);
            }
        }
        else
        {
            // Disable Skill upgrade menu
            SetSkillsUpgradeUIActive(false);
        }
    }
    #endregion SkillsUpgrade

    public void ActivateAdvance(PartyUnit partyUnit)
    {
        // Activate this object
        gameObject.SetActive(true);
        // Save Party unit for later use
        focusedPartyUnit = partyUnit;
        // Activate unit info panel
        ActivateUnitInfoPanel(partyUnit);
        // Fill in generic information
        InitGenericInfo();
        // Fill in stats
        InitStats();
        // Fill in class upgrade information
        InitClasses();
        // Fill in skills learning information
        InitSkills();
    }

    void OnEnable()
    {
        // populate UI with information from unit
    }

    void OnDisable()
    {
        DeactivateUnitInfoPanel();
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
