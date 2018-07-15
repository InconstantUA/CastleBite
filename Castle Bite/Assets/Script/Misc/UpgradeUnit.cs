using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUnit : MonoBehaviour {

    GameObject unitBackupGameObject;
    PartyUnit focusedPartyUnit;
    UnitInfoPanel unitInfoPanel;
    int statsUpgradeCount;
    //[SerializeField]
    //int classUIPosition = -89;  // starting position + current position for iteration
    //[SerializeField]
    //int classUIStep = -16;      // next class will be displayed in UI after this pixels

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ActivateUnitInfoPanel(PartyUnit partyUnit)
    {
        Transform unitInfoPanelTr = transform.root.Find("MiscUI/UnitInfoPanel");
        unitInfoPanel = unitInfoPanelTr.GetComponent<UnitInfoPanel>();
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
        transform.Find("Panel/Generic/UpgradePoints/Info").GetComponent<TextButton>().OnClick.RemoveAllListeners();
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

    void AddUpgradePoint()
    {
        // add 1 point from upgrade points
        focusedPartyUnit.UnitUpgradePoints += 1;
        // update UI
        SetUnitUpgradePointsUI();
        // enable plus on stats, class and skill points
        // enable plus on stat points
        SetUnitStatPointPlusUIInteractable(true);
        // enable plus on skill points, because it use the same pool as stat points
        SetUnitSkillPointsPlusUIInteractable(true);
        // enable plus on class points, because it use the same pool as stat points
        SetUnitClassPointPlusUIInteractable(true);
    }

    void WithdrawUpgradePoint()
    {
        // remove 1 point from upgrade points
        focusedPartyUnit.UnitUpgradePoints -= 1;
        // update UI
        SetUnitUpgradePointsUI();
        // verify if upgrade pool does not reach 0 points
        if (focusedPartyUnit.UnitUpgradePoints == 0)
        {
            // disable plus on stat points
            SetUnitStatPointPlusUIInteractable(false);
            // disable plus on skill points, because it use the same pool as stat points
            SetUnitSkillPointsPlusUIInteractable(false);
            // disable plus on class points, because it use the same pool as stat points
            SetUnitClassPointPlusUIInteractable(false);
        }
    }
    #endregion Generic/UpgradePoints

    #region StatsUpgrade
    void SetUnitStatPointsValueUI()
    {
        transform.Find("Panel/StatsUpgrade/StatPoints/Value").GetComponent<Text>().text = focusedPartyUnit.UnitStatPoints.ToString();
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
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Info").GetComponent<TextButton>().OnClick.RemoveAllListeners();
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

    #region Stat points
    void AddStatPoint()
    {
        // add 1 point to stat points
        focusedPartyUnit.UnitStatPoints += 1;
        // update UI
        SetUnitStatPointsValueUI();
        // enable plus on upgrade stats
        SetUnitUpgradeStatsPlusUIInteractable(true);
        // enable minus on stat points, so user can revert changes back
        SetUnitStatPointMinusUIInteractable(true);
    }

    void WithdrawStatPoint()
    {
        // remove 1 point to stat points
        focusedPartyUnit.UnitStatPoints -= 1;
        // update UI
        SetUnitStatPointsValueUI();
        // verify if we still have points to spend on stats upgrades
        if (focusedPartyUnit.UnitStatPoints == 0)
        {
            // disable plus on upgrade stats
            SetUnitUpgradeStatsPlusUIInteractable(false);
            // disable minus on stat pints, so we do not go to negative values
            SetUnitStatPointMinusUIInteractable(false);
        }
    }

    void UnitStatPointPlusUIActions()
    {
        Debug.Log("UnitStatPointPlusUIActions");
        WithdrawUpgradePoint();
        AddStatPoint();
    }

    void SetUnitStatPointPlusUIActions()
    {
        transform.Find("Panel/StatsUpgrade/StatPoints/Plus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        transform.Find("Panel/StatsUpgrade/StatPoints/Plus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitStatPointPlusUIActions(); });
    }

    void UnitStatPointMinusUIActions()
    {
        Debug.Log("UnitStatPointMinusUIActions");
        WithdrawStatPoint();
        AddUpgradePoint();
    }

    void SetUnitStatPointMinusUIActions()
    {
        transform.Find("Panel/StatsUpgrade/StatPoints/Minus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        transform.Find("Panel/StatsUpgrade/StatPoints/Minus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitStatPointMinusUIActions(); });
    }
    #endregion Stat points

    #region Upgrade stats

    #region Health
    void UpdateHealthInfo()
    {
        // upgrade health in unit Info UI
        unitInfoPanel.SetHealthPreview(
            focusedPartyUnit.GetHealthCurr(),
            focusedPartyUnit.GetHealthMax(),
            focusedPartyUnit.GetHealthMaxIncrementOnLevelUp() * statsUpgradeCount
        );
    }

    void UpgradeHealth()
    {
        // upgrade health in unit object
        focusedPartyUnit.SetHealthCurr(focusedPartyUnit.GetHealthCurr() + focusedPartyUnit.GetHealthMaxIncrementOnLevelUp());
        focusedPartyUnit.SetHealthMax(focusedPartyUnit.GetHealthMax() + focusedPartyUnit.GetHealthMaxIncrementOnLevelUp());
        // upgrade health in unit Info UI
        UpdateHealthInfo();
    }

    void DowngradeHealth()
    {
        // downgrade health in unit object
        focusedPartyUnit.SetHealthCurr(focusedPartyUnit.GetHealthCurr() - focusedPartyUnit.GetHealthMaxIncrementOnLevelUp());
        focusedPartyUnit.SetHealthMax(focusedPartyUnit.GetHealthMax() - focusedPartyUnit.GetHealthMaxIncrementOnLevelUp());
        // upgrade health in unit Info UI
        UpdateHealthInfo();
    }
    #endregion Health

    #region Ability Power
    void UpdateAbilityPowerInfo()
    {
        // upgrade Ability Power in unit Info UI
        unitInfoPanel.SetAbilityPowerPreview(
            focusedPartyUnit.GetPower(),
            focusedPartyUnit.GetPowerIncrementOnLevelUp() * statsUpgradeCount
        );
    }

    void UpgradeAbilityPower()
    {
        // upgrade Ability Power in unit object
        focusedPartyUnit.SetPower(focusedPartyUnit.GetPower() + focusedPartyUnit.GetPowerIncrementOnLevelUp());
        // upgrade Ability Power in unit Info UI
        UpdateAbilityPowerInfo();
    }

    void DowngradeAbilityPower()
    {
        // downgrade Ability Power in unit object
        focusedPartyUnit.SetPower(focusedPartyUnit.GetPower() - focusedPartyUnit.GetPowerIncrementOnLevelUp());
        // upgrade Ability Power in unit Info UI
        UpdateAbilityPowerInfo();
    }
    #endregion Ability Power

    #region Unique power modifiers
    void UpdateUniquePowerModifiersInfo()
    {
        UniquePowerModifier[] uniquePowerModifiers = focusedPartyUnit.GetComponentsInChildren<UniquePowerModifier>();
        for (int i = 1; i <= uniquePowerModifiers.Length; i++)
        {
            unitInfoPanel.SetUniquePowerModifiersPreview(
                i,
                uniquePowerModifiers[i - 1].Power,
                uniquePowerModifiers[i - 1].PowerIncrementOnLevelUp * statsUpgradeCount,
                uniquePowerModifiers[i - 1].Chance,
                uniquePowerModifiers[i - 1].ChanceIncrementOnLevelUp * statsUpgradeCount
            );
        }
    }

    void UpgradeUniquePowerModifiers()
    {
        // upgrade Unique power modifier object
        foreach (UniquePowerModifier upm in focusedPartyUnit.GetComponentsInChildren<UniquePowerModifier>())
        {
            upm.Power += upm.PowerIncrementOnLevelUp;
            upm.Chance += upm.ChanceIncrementOnLevelUp;
        }
        // upgrade Unique power modifier Info UI
        UpdateUniquePowerModifiersInfo();
    }

    void DowngradeUniquePowerModifiers()
    {
        // downgrade Unique power modifier object
        foreach (UniquePowerModifier upm in focusedPartyUnit.GetComponentsInChildren<UniquePowerModifier>())
        {
            upm.Power -= upm.PowerIncrementOnLevelUp;
            upm.Chance -= upm.ChanceIncrementOnLevelUp;
        }
        // upgrade Unique power modifier Info UI
        UpdateUniquePowerModifiersInfo();
    }
    #endregion Unique power modifiers

    void UpgradeStats()
    {
        // consume stat point
        WithdrawStatPoint();
        // increment stats upgrade count
        statsUpgradeCount += 1;
        // enable minus to roll back upgrade
        SetUnitUpgradeStatsMinusUIInteractable(true);
        // upgrade unit stats
        UpgradeHealth();
        UpgradeAbilityPower();
        UpgradeUniquePowerModifiers();
    }

    void DowngradeStats()
    {
        // release stat point
        AddStatPoint();
        // decrement stats upgrade count
        statsUpgradeCount -= 1;
        // enable plus to do upgrade
        SetUnitUpgradeStatsPlusUIInteractable(true);
        // verify if minus should be deactivated, when we reach 0, so we do not downgrade more than we upgraded
        if (statsUpgradeCount == 0)
        {
            // disable minus
            SetUnitUpgradeStatsMinusUIInteractable(false);
        }
        // downgrade unit stats
        DowngradeHealth();
        DowngradeAbilityPower();
        DowngradeUniquePowerModifiers();
    }

    void UnitStatsUpgradePlusUIActions()
    {
        Debug.Log("UnitStatsUpgradePlusUIActions");
        UpgradeStats();
    }

    void SetUnitStatsUpgradePlusUIActions()
    {
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Plus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Plus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitStatsUpgradePlusUIActions(); });
    }

    void UnitStatsUpgradeMinusUIActions()
    {
        Debug.Log("UnitStatsUpgradeMinusUIActions");
        DowngradeStats();
    }

    void SetUnitStatsUpgradeMinusUIActions()
    {
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Minus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        transform.Find("Panel/StatsUpgrade/UpgradeStats/Minus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitStatsUpgradeMinusUIActions(); });
    }
    #endregion Upgrade stats

    void InitStats()
    {
        // Display current number of unitStatPoints
        SetUnitStatPointsValueUI();
        // verify if unit has stats points
        if (focusedPartyUnit.UnitStatPoints >= 1)
        {
            // allow user to convert those points back to upgrade points, if needed
            // enable minus option
            SetUnitStatPointMinusUIInteractable(true);
        }
        else
        {
            // Disable minus option, it will be enabled if use click on plus button
            SetUnitStatPointMinusUIInteractable(false);
        }
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
        // Set actions for Stat points plus and minus
        SetUnitStatPointPlusUIActions();
        SetUnitStatPointMinusUIActions();
        // Set actions for Stat upgrade plus and minus
        SetUnitStatsUpgradePlusUIActions();
        SetUnitStatsUpgradeMinusUIActions();
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

    void SetClassUpgradeInfoUI()
    {
        transform.Find("Panel/ClassUpgrade/ChooseClassUpgradePath/Info").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        string info = "Click [+] to spend 1 class point to upgrade unit's class."
           + "\r\n" + "Click [-] to roll back changes."
           + "\r\n" + "Some classes require specific unit level before unit can change class."
           + "\r\n" + "First change to lower level classes. Once applied, it is not possible to change it aftwards, also other class branches on the same level will be locked."
           + "\r\n" + "Right click on a class name to preview class unit information.";
        transform.Find("Panel/ClassUpgrade/ChooseClassUpgradePath/Info").GetComponent<TextButton>().OnClick.AddListener(delegate { ShowInfo(info); });
    }

    void SetClassUISeparators(Transform classUI, int level)
    {
        // we assume that we have levels from 0 to 3 (4 levels in total with root level), so we have only 3 separators in UI
        switch (level)
        {
            case 0:
                classUI.Find("Separator1").SetAsFirstSibling();
                classUI.Find("Separator2").SetAsFirstSibling();
                classUI.Find("Separator3").SetAsFirstSibling();
                classUI.Find("ClassName").SetAsFirstSibling();
                break;
            case 1:
                classUI.Find("Separator2").SetAsFirstSibling();
                classUI.Find("Separator3").SetAsFirstSibling();
                classUI.Find("ClassName").SetAsFirstSibling();
                classUI.Find("Separator1").SetAsFirstSibling();
                break;
            case 2:
                classUI.Find("Separator3").SetAsFirstSibling();
                classUI.Find("ClassName").SetAsFirstSibling();
                classUI.Find("Separator1").SetAsFirstSibling();
                classUI.Find("Separator2").SetAsFirstSibling();
                break;
            case 3:
                classUI.Find("ClassName").SetAsFirstSibling();
                classUI.Find("Separator1").SetAsFirstSibling();
                classUI.Find("Separator2").SetAsFirstSibling();
                classUI.Find("Separator3").SetAsFirstSibling();
                break;
            default:
                Debug.LogError("Unhandled condition " + level.ToString());
                break;
        }
    }

    void SetClassUIName(Transform classUI, PartyUnit unitClass)
    {
        // set text to unit name
        classUI.Find("ClassName").GetComponent<Text>().text = unitClass.GetUnitName();
    }

    void SetClassUIRequiredLevel(Transform classUI, PartyUnit unitClass, int classLevel)
    {
        // set text to level + 1
        classUI.Find("RequiredLevel").GetComponent<Text>().text = (classLevel + 1).ToString();
    }

    void SetClassUIMinusButtonInteractable(Transform classUI, bool doActivate)
    {
        classUI.Find("Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetClassUIPlusButtonInteractable(Transform classUI, bool doActivate)
    {
        classUI.Find("Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void InitClassUIMinusButton(Transform classUI)
    {
        // disable it by default
        SetClassUIMinusButtonInteractable(classUI, false);
    }

    bool VerifyClassPrerequisites(PartyUnit checkedUnitClass, int classLevel)
    {
        // compare checkedUnitClass with focusedPartyUnit
        // verify if checked unit class is one of the classes which are unlocked by focusedPartyUnit
        foreach (PartyUnit unlockedClass in focusedPartyUnit.UnlocksUnits)
        {
            if (checkedUnitClass.GetUnitType() == unlockedClass.GetUnitType())
            {
                // verify if unit mets level requirements
                if (focusedPartyUnit.GetLevel() >= (classLevel + 1))
                {
                    return true;
                }
            }
        }
        // everything else does not meet requirements
        return false;
    }

    void InitClassUIPlusButton(Transform classUI, PartyUnit unitClass, int classLevel)
    {
        // verify if unit has class points
        if (focusedPartyUnit.UnitClassPoints >= 1)
        {
            // has points
            // verify if prerequsities are met
            //  met prerequisites == can change to this class
            //  does not met prerequisites == cannot change to this class
            SetClassUIPlusButtonInteractable(classUI, VerifyClassPrerequisites(unitClass, classLevel));
        }
        else
        {
            // no class points
            SetClassUIPlusButtonInteractable(classUI, false);
        }
    }

    void HidePreview()
    {
        // get preview transform
        Transform previewTr = transform.root.Find("MiscUI/UnitInfoPanel/Preview");
        // first change color to normal
        previewTr.Find("ExitPreview").GetComponent<TextButton>().SetNormalStatus();
        // show preview text and button
        previewTr.gameObject.SetActive(false);
    }

    void ShowPreview()
    {
        // get preview transform
        Transform previewTr = transform.root.Find("MiscUI/UnitInfoPanel/Preview");
        // verify if it is not active already
        if (!previewTr.gameObject.activeSelf)
        {
            // show preview text and button
            previewTr.gameObject.SetActive(true);
            // Get exit preview button
            TextButton exitPreviewBtn = previewTr.Find("ExitPreview").GetComponent<TextButton>();
            // remove all previously set via script listeners
            exitPreviewBtn.OnClick.RemoveAllListeners();
            // add new listener to activate unit info with focused unit info
            exitPreviewBtn.OnClick.AddListener(delegate { ShowUnitInfo(focusedPartyUnit); });
            // add new listener to exit preview button
            exitPreviewBtn.OnClick.AddListener(delegate { HidePreview(); });
        }
    }

    void ShowUnitInfo(PartyUnit unitClass)
    {
        // Show unit info
        Debug.Log("Show " + unitClass.GetUnitName() + " unit info.");
        transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>().ActivateAdvance(unitClass);
        // Show preview text and button
        ShowPreview();
    }

    void SetClassNameUIShowUnitInfoOnRightClick(Transform classUI, PartyUnit unitClass)
    {
        classUI.Find("ClassName").GetComponent<TextButton>().OnRightMouseButtonDown.RemoveAllListeners();
        classUI.Find("ClassName").GetComponent<TextButton>().OnRightMouseButtonDown.AddListener(delegate { ShowUnitInfo(unitClass); });
    }

    void CleanClassUIClasses()
    {
        foreach(Transform child in transform.Find("Panel/ClassUpgrade/ClassesTree"))
        {
            Destroy(child.gameObject);
        }
    }

    void AddClassPoint()
    {
        // add 1 point to class points
        focusedPartyUnit.UnitClassPoints += 1;
        // update UI
        SetUnitClassPointsValueUI();
        // enable minus on class points, so user can revert changes back
        SetUnitClassPointMinusUIInteractable(true);
        // Update classes
        // Clean previous classes;
        CleanClassUIClasses();
        // Set options for all classes:
        SetClasses();
    }

    void WithdrawClassPoint()
    {
        // remove 1 point to class points
        focusedPartyUnit.UnitClassPoints -= 1;
        // update UI
        SetUnitClassPointsValueUI();
        // verify if we still have points to spend on classs upgrades
        if (focusedPartyUnit.UnitClassPoints == 0)
        {
            // disable minus on class pints, so we do not go to negative values
            SetUnitClassPointMinusUIInteractable(false);
            // Update classes
            // Clean previous classes;
            CleanClassUIClasses();
            // Set options for all classes:
            SetClasses();
        }
    }

    void UnitClassPointPlusUIActions()
    {
        Debug.Log("UnitClassPointPlusUIActions");
        WithdrawUpgradePoint();
        AddClassPoint();
    }

    void SetUnitClassPointPlusUIActions()
    {
        // clean prevoius actions, just in case
        transform.Find("Panel/ClassUpgrade/ClassPoints/Plus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        // set new actions
        transform.Find("Panel/ClassUpgrade/ClassPoints/Plus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitClassPointPlusUIActions(); });
    }

    void UnitClassPointMinusUIActions()
    {
        Debug.Log("UnitClassPointMinusUIActions");
        WithdrawClassPoint();
        AddUpgradePoint();
    }

    void SetUnitClassPointMinusUIActions()
    {
        transform.Find("Panel/ClassUpgrade/ClassPoints/Minus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        transform.Find("Panel/ClassUpgrade/ClassPoints/Minus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitClassPointMinusUIActions(); });
    }

    void SetClassUI(PartyUnit unitClass, int classLevel)
    {
        // create class string in UI
        // Get class UI template
        Transform classUITemplate = transform.Find("Panel/ClassUpgrade/ClassTemplate");
        // Get class UI parent transform
        Transform classUIParent = transform.Find("Panel/ClassUpgrade/ClassesTree");
        // Clone class UI and place it in parent
        Transform newClassUI = Instantiate(classUITemplate, classUIParent);
        // Activate it
        newClassUI.gameObject.SetActive(true);
        // Fill in all required information
        SetClassUISeparators(newClassUI, classLevel);
        SetClassUIName(newClassUI, unitClass);
        SetClassUIRequiredLevel(newClassUI, unitClass, classLevel);
        InitClassUIMinusButton(newClassUI);
        InitClassUIPlusButton(newClassUI, unitClass, classLevel);
        SetClassNameUIShowUnitInfoOnRightClick(newClassUI, unitClass);
        // create class strings recursively for all child classes
        foreach (PartyUnit unitSubClass in unitClass.UnlocksUnits)
        {
            SetClassUI(unitSubClass, classLevel + 1);
        }
    }

    PartyUnit GetRootClass(PartyUnit partyUnit)
    {
        Debug.Log("Get required class for " + partyUnit.GetUnitName());
        if (partyUnit.RequiresUnit != null)
        {
            return GetRootClass(partyUnit.RequiresUnit);
        }
        else
        {
            Debug.Log("Root class is " + partyUnit.GetUnitName());
            return partyUnit;
        }
    }

    void SetClasses()
    {
        // Get root
        PartyUnit rootClass = GetRootClass(focusedPartyUnit);
        // Display all classes starting from root, but do not display root class
        foreach(PartyUnit unitClass in rootClass.UnlocksUnits)
        {
            SetClassUI(unitClass, 1);
        }
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
            // verify if there are points available
            if (focusedPartyUnit.UnitClassPoints >= 1)
            {
                // Enable minus option
                // allow user to convert those points back to upgrade points, if needed
                SetUnitClassPointMinusUIInteractable(true);
            }
            else
            {
                // Disable minus option, it will be enabled if user click on plus
                SetUnitClassPointMinusUIInteractable(false);
            }
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
            SetClassUpgradeInfoUI();
            // Set actions for Class points plus and minus
            SetUnitClassPointPlusUIActions();
            SetUnitClassPointMinusUIActions();
            // Clean previous classes;
            CleanClassUIClasses();
            // Set options for all classes:
            SetClasses();
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
        transform.Find("Panel/SkillsUpgrade/ChooseAndLearnSkills/Info").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        string info = "Click [+] to spend 1 skill point to learn new skill or upgrade already known skill to a higher level, if this is applicable."
           + "\r\n" + "Click [-] to roll back changes."
           + "\r\n" + "Some skills require specific hero level, before hero can learn them."
           + "\r\n" + "Right click on a skill name to get additional information.";
        transform.Find("Panel/SkillsUpgrade/ChooseAndLearnSkills/Info").GetComponent<TextButton>().OnClick.AddListener(delegate { ShowInfo(info); });
    }

    void SetSkillUICurrentLevel(Transform skillUI, PartyUnit.UnitSkill skill)
    {
        // set text to level + 1
        skillUI.Find("SkillLevel").GetComponent<Text>().text = skill.Level.Current.ToString();
    }

    void SetSkillUIName(Transform skillUI, PartyUnit.UnitSkill skill)
    {
        // set text to unit name
        skillUI.Find("SkillName").GetComponent<Text>().text = skill.Name;
    }

    int GetSkillRequiredLevel(PartyUnit.UnitSkill skill)
    {
        // act based on the current skill level
        if (0 == skill.Level.Current)
        {
            return skill.RequiredHeroLevel;
        }
        else if (1 == skill.Level.Current)
        {
            return skill.RequiredHeroLevel + skill.LevelUpIncrementStep;
        }
        else if (1 < skill.Level.Current)
        {
            return skill.RequiredHeroLevel + skill.Level.Current * skill.LevelUpIncrementStep;
        }
        else
        {
            return 0;
        }
    }

    void SetSkillUIRequiredLevel(Transform skillUI, PartyUnit.UnitSkill skill)
    {
        // verify if skill level is already maximum
        if (skill.Level.Current == skill.Level.Max)
        {
            skillUI.Find("RequiredLevel").GetComponent<Text>().text = "max";
        }
        else
        {
            skillUI.Find("RequiredLevel").GetComponent<Text>().text = GetSkillRequiredLevel(skill).ToString();
        }
    }

    void SetSkillUIMinusButtonInteractable(Transform skillUI, bool doActivate)
    {
        skillUI.Find("Minus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void SetSkillUIPlusButtonInteractable(Transform skillUI, bool doActivate)
    {
        skillUI.Find("Plus").GetComponent<TextButton>().SetInteractable(doActivate);
    }

    void InitSkillUIMinusButton(Transform skillUI)
    {
        // disable it by default
        SetSkillUIMinusButtonInteractable(skillUI, false);
    }

    bool VerifySkillPrerequisites(PartyUnit.UnitSkill skill)
    {
        // verify if hero level meets skill level requirements
        if (focusedPartyUnit.GetLevel() >= GetSkillRequiredLevel(skill))
        {
            return true;
        }
        // everything else does not meet requirements
        return false;
    }

    void InitSkillUIPlusButton(Transform skillUI, PartyUnit.UnitSkill skill)
    {
        // Verify if there are free upgrade stats points available
        if (focusedPartyUnit.UnitSkillPoints >= 1)
        {
            // verify if prerequsities are met
            //  met prerequisites == can change to this class
            //  does not met prerequisites == cannot change to this class
            SetSkillUIPlusButtonInteractable(skillUI, VerifySkillPrerequisites(skill));
        }
        else
        {
            // disable plus button
            SetSkillUIPlusButtonInteractable(skillUI, false);
        }
    }

    void SetSkillNameUIShowUnitInfoOnRightClick(Transform skillUI, PartyUnit.UnitSkill skill)
    {
        // Get Text button
        skillUI.Find("SkillName").GetComponent<TextButton>().OnRightMouseButtonDown.RemoveAllListeners();
        skillUI.Find("SkillName").GetComponent<TextButton>().OnRightMouseButtonDown.AddListener(delegate { ShowInfo(skill.Description); });
    }

    void SetSkillUI(PartyUnit.UnitSkill skill)
    {
        // create skill string in UI
        // Get class UI template
        Transform skillUITemplate = transform.Find("Panel/SkillsUpgrade/SkillTemplate");
        // Get class UI parent transform
        Transform skillUIParent = transform.Find("Panel/SkillsUpgrade/Skills");
        // Clone class UI and place it in parent
        Transform newSkillUI = Instantiate(skillUITemplate, skillUIParent);
        // Activate it
        newSkillUI.gameObject.SetActive(true);
        // Fill in all required information
        SetSkillUICurrentLevel(newSkillUI, skill);
        SetSkillUIName(newSkillUI, skill);
        SetSkillUIRequiredLevel(newSkillUI, skill);
        InitSkillUIMinusButton(newSkillUI);
        InitSkillUIPlusButton(newSkillUI, skill);
        SetSkillNameUIShowUnitInfoOnRightClick(newSkillUI, skill);
    }

    void SetUnitSkillsUI()
    {
        foreach (PartyUnit.UnitSkill skill in focusedPartyUnit.skills)
        {
            SetSkillUI(skill);
        }
    }

    void CleanSkillUISkills()
    {
        foreach (Transform child in transform.Find("Panel/SkillsUpgrade/Skills"))
        {
            Destroy(child.gameObject);
        }
    }

    void AddSkillPoint()
    {
        // add 1 point to skill points
        focusedPartyUnit.UnitSkillPoints += 1;
        // update UI
        SetUnitSkillPointsValueUI();
        // enable minus on skill points, so user can revert changes back
        SetUnitSkillPointsMinusUIInteractable(true);
        // Update skills
        // Clean previous skills;
        CleanSkillUISkills();
        // Set options for all skills:
        SetUnitSkillsUI();
    }

    void WithdrawSkillPoint()
    {
        // remove 1 point to skill points
        focusedPartyUnit.UnitSkillPoints -= 1;
        // update UI
        SetUnitSkillPointsValueUI();
        // verify if we still have points to spend on classs upgrades
        if (focusedPartyUnit.UnitSkillPoints == 0)
        {
            // disable minus on skill pints, so we do not go to negative values
            SetUnitSkillPointsMinusUIInteractable(false);
            // Update skills
            // Clean previous skills;
            CleanSkillUISkills();
            // Set options for all skills:
            SetUnitSkillsUI();
        }
    }

    void UnitSkillPointPlusUIActions()
    {
        Debug.Log("UnitSkillPointPlusUIActions");
        WithdrawUpgradePoint();
        AddSkillPoint();
    }

    void SetUnitSkillPointPlusUIActions()
    {
        // clean prevoius actions, just in case
        transform.Find("Panel/SkillsUpgrade/SkillPoints/Plus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        // set new actions
        transform.Find("Panel/SkillsUpgrade/SkillPoints/Plus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitSkillPointPlusUIActions(); });
    }

    void UnitSkillPointMinusUIActions()
    {
        Debug.Log("UnitSkillPointMinusUIActions");
        WithdrawSkillPoint();
        AddUpgradePoint();
    }

    void SetUnitSkillPointMinusUIActions()
    {
        transform.Find("Panel/SkillsUpgrade/SkillPoints/Minus").GetComponent<TextButton>().OnClick.RemoveAllListeners();
        transform.Find("Panel/SkillsUpgrade/SkillPoints/Minus").GetComponent<TextButton>().OnClick.AddListener(delegate { UnitSkillPointMinusUIActions(); });
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
            // verify if unit has skill points
            if (focusedPartyUnit.UnitSkillPoints >= 1)
            {
                // allow user to convert those points back to upgrade points, if needed
                SetUnitSkillPointsMinusUIInteractable(true);
            }
            else
            {
                // Disable minus option, it will be enabled if user click on plus
                SetUnitSkillPointsMinusUIInteractable(false);
            }
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
            // Set Skill Point plus and minus actions
            SetUnitSkillPointMinusUIActions();
            SetUnitSkillPointPlusUIActions();
            // Clean previous skills;
            CleanSkillUISkills();
            // Set options for all skills:
            SetUnitSkillsUI();
        }
        else
        {
            // Disable Skill upgrade menu
            SetSkillsUpgradeUIActive(false);
        }
    }
    #endregion SkillsUpgrade

    //void CopyClassValues(PartyUnit sourceComp, PartyUnit targetComp)
    //{
    //    FieldInfo[] sourceFields = sourceComp.GetType().GetFields(BindingFlags.Public |
    //                                                     BindingFlags.NonPublic |
    //                                                     BindingFlags.Instance);
    //    int i = 0;
    //    for (i = 0; i < sourceFields.Length; i++)
    //    {
    //        var value = sourceFields[i].GetValue(sourceComp);
    //        sourceFields[i].SetValue(targetComp, value);
    //    }
    //}

    void Backup()
    {
        // clone object
        unitBackupGameObject = Instantiate(focusedPartyUnit.gameObject, transform.root.Find("Backup"));
        // copy name of the focused game object to backup game object
        unitBackupGameObject.name = focusedPartyUnit.gameObject.name;

    }

    void RestoreBackup()
    {
        // get parent game object
        Transform parentGameObjectTr = focusedPartyUnit.transform.parent;
        // change parent of backuped object
        unitBackupGameObject.transform.SetParent(parentGameObjectTr);
    }

    void CleanBackup()
    {
        // destroy backup
        Destroy(unitBackupGameObject);
    }

    public void ActivateAdvance(PartyUnit partyUnit)
    {
        // Activate this object
        gameObject.SetActive(true);
        // Save link to Party unit for later use
        focusedPartyUnit = partyUnit;
        // Save backup of party unit component
        Backup();
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
        // Reset counters
        statsUpgradeCount = 0;
    }

    void OnEnable()
    {
        // populate UI with information from unit
    }

    void OnDisable()
    {
        DeactivateUnitInfoPanel();
    }

    void CommonOnExit()
    {
        // close upgrade unit window
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        // revert changes back
        // restore backup of unit
        RestoreBackup();
        // destroy current PartyUnit component
        Destroy(focusedPartyUnit.gameObject);
        // execute common on exit actions
        CommonOnExit();
    }

    public void Apply()
    {
        Debug.Log("Apply");
        // Update party panel UI to represent changes
        focusedPartyUnit.SetUnitCellInfoUI();
        // remove backup
        CleanBackup();
        // execute common on exit actions
        CommonOnExit();
    }
}
