using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyUnitUI : MonoBehaviour {
    [SerializeField]
    PartyUnit lPartyUnit;
    [SerializeField]
    Text unitStatusText;
    [SerializeField]
    Text unitInfoPanelText;
    [SerializeField]
    Color defaultUnitStatusColor;
    [SerializeField]
    Color defaultUnitInfoColor;

    public PartyUnit LPartyUnit
    {
        get
        {
            return lPartyUnit;
        }

        set
        {
            lPartyUnit = value;
        }
    }

    void OnEnable()
    {
        // verify if party unit is set
        UpdateUnitCellInfo();
        // Verify and enable Unit Equipment button
        UpdateUnitEquipmentControl();
        // Update Unit Upgrade button
        UpdateUpgradeUnitControl();
        // Set required PartyUnit handlers
        SetPartyUnitHandlersActive(true);
        // register for unit health events changes
        //EventsAdmin.Instance.OnPartyUnitHealthCurrentChanged.AddListener(OnCurrentHealthChanged);
        //EventsAdmin.Instance.OnPartyUnitHealthMaxChanged.AddListener(OnMaxHealthChanged);
    }

    void OnDisable()
    {
        // Remove required PartyUnit handlers
        SetPartyUnitHandlersActive(false);
        // unregister from unit health events changes
        //EventsAdmin.Instance.OnPartyUnitHealthCurrentChanged.RemoveListener(OnCurrentHealthChanged);
        //EventsAdmin.Instance.OnPartyUnitHealthMaxChanged.RemoveListener(OnMaxHealthChanged);
    }

    void OnTransformChildrenChanged()
    {
        Debug.Log("PartyUnitUI OnTransformChildrenChanged");
        UpdateUnitCellInfo();
    }

    void SetPartyUnitCityScreenHandlersActive(bool doActivate)
    {
        if (doActivate)
        {
            // add UnitOnBattleMouseHandler Component
            gameObject.AddComponent<UnitDragHandler>();
        }
        else
        {
            // remove UnitOnBattleMouseHandler Component
            Destroy(GetComponent<UnitDragHandler>());
        }
    }

    void SetPartyUnitBattleScreenHandlersActive(bool doActivate)
    {
        if (doActivate)
        {
            // add UnitOnBattleMouseHandler Component
            gameObject.AddComponent<UnitOnBattleMouseHandler>();
        }
        else
        {
            // remove UnitOnBattleMouseHandler Component
            Destroy(GetComponent<UnitOnBattleMouseHandler>());
        }
    }

    void SetPartyUnitHandlersActive(bool doActivate)
    {
        if (GetComponentInParent<UIManager>())
        {
            // verify if we are in city screen view
            if (GetComponentInParent<UIManager>().GetComponentInChildren<EditPartyScreen>())
            {
                SetPartyUnitCityScreenHandlersActive(doActivate);
            }
            else if (GetComponentInParent<UIManager>().GetComponentInChildren<BattleScreen>())
            {
                SetPartyUnitBattleScreenHandlersActive(doActivate);
            }
        }
    }

    public Transform GetUnitCell()
    {
        // structure: 2UnitCell[Front/Back/Wide]-1UnitSlot-UnitCanvas(this with link to Unit)
        return transform.parent.parent;
    }

    public PartyPanelRow GetUnitRow()
    {
        // structure: 3[Top/Middle/Bottom]Row-2UnitCell[Front/Back/Wide]-1UnitSlot-UnitCanvas(this with link to Unit)
        return transform.parent.parent.parent.GetComponent<PartyPanelRow>();
    }

    #region Unit Info Panel
    public Text UnitInfoPanelText
    {
        get
        {
            // return transform.Find("InfoPanel").GetComponent<Text>();
            return unitInfoPanelText;
        }
    }

    // Note: animation should be identical to the function with the same name in PartyPanel
    public void FadeUnitCellInfo(float alpha)
    {
        Color c = UnitInfoPanelText.color;
        c.a = alpha;
        UnitInfoPanelText.color = c;
    }

    public void ClearUnitInfoPanel()
    {
        //Color32 defaultColor = new Color32(180, 180, 180, 255);
        UnitInfoPanelText.text = "";
        UnitInfoPanelText.color = defaultUnitInfoColor;
    }

    #endregion Unit Info Panel

    #region Unit Status and (De)Buffs
    public Text UnitStatusText
    {
        get
        {
            return unitStatusText;
            //return transform.Find("UnitStatus").GetComponent<Text>();
        }
    }

    public Transform GetUnitBuffsPanel()
    {
        return transform.Find("UnitStatus/Buffs");
    }

    public Transform GetUnitDebuffsPanel()
    {
        return transform.Find("UnitStatus/Debuffs");
    }

    //public string GetUnitCellAddress()
    //{
    //    // structure: PartyPanel-3[Top/Middle/Bottom]-2[Front/Back/Wide]-1UnitSlot-PartyUnitUI(this)
    //    return transform.parent.parent.parent.name + "/" + transform.parent.parent.name;
    //}

    public void ClearPartyUnitStatusUI()
    {
        //Color32 defaultColor = new Color32(180, 180, 180, 255);
        UnitStatusText.text = "";
        UnitStatusText.color = defaultUnitStatusColor;
    }


    //public void SetUnitStatus(UnitStatus unitStatus)
    //{
    //    Debug.Log("Set unit " + LPartyUnit.UnitName + " status " + unitStatus.ToString());
    //    LPartyUnit.UnitStatus = unitStatus;
    //    //// get new UI color according ot unit status
    //    //Color32 newUIColor;
    //    //// set dead in status
    //    //string statusString;
    //    //switch (value)
    //    //{
    //    //    case UnitStatus.Active:
    //    //        newUIColor = new Color32(160, 160, 160, 255);
    //    //        statusString = "";
    //    //        break;
    //    //    case UnitStatus.Waiting:
    //    //        newUIColor = new Color32(96, 96, 96, 96);
    //    //        statusString = "Waiting";
    //    //        break;
    //    //    case UnitStatus.Escaping:
    //    //        newUIColor = new Color32(96, 96, 96, 255);
    //    //        statusString = "Escaping";
    //    //        break;
    //    //    case UnitStatus.Escaped:
    //    //        newUIColor = new Color32(64, 64, 64, 255);
    //    //        statusString = "Escaped";
    //    //        // clear unit buffs and debuffs
    //    //        RemoveAllBuffsAndDebuffs();
    //    //        break;
    //    //    case UnitStatus.Dead:
    //    //        newUIColor = new Color32(64, 64, 64, 255);
    //    //        statusString = "Dead";
    //    //        // clear unit buffs and debuffs
    //    //        RemoveAllBuffsAndDebuffs();
    //    //        break;
    //    //    default:
    //    //        Debug.LogError("Unknown status " + value.ToString());
    //    //        newUIColor = Color.red;
    //    //        statusString = "Error";
    //    //        break;
    //    //}
    //    // Get Unit status config
    //    UnitStatusConfig unitStatusConfig = Array.Find(ConfigManager.Instance.UnitStatusConfigs, e => e.unitStatus == LPartyUnit.UnitStatus);
    //    // Set UI colors and text according to the configu
    //    // Set status text color
    //    GetUnitStatusText().color = unitStatusConfig.statusTextColor;
    //    // Set status text
    //    GetUnitStatusText().text = unitStatusConfig.statusDisplayName;
    //    // Set current health color
    //    GetUnitCurrentHealthText().color = unitStatusConfig.currentHealthTextColor;
    //    // Set max health color
    //    GetUnitMaxHealthText().color = unitStatusConfig.maxHealthTextColor;
    //    // Set canvas color
    //    GetUnitCanvasText().color = unitStatusConfig.unitCanvasTextColor;
    //}

    public void SetUnitStatus()
    {
        Debug.Log("Set unit " + LPartyUnit.UnitName + " status " + LPartyUnit.UnitStatus.ToString() + " in UI");
        // Reset previously fetched unit status config, because it has changed
        // After reset on next get UnitStatusUIConfig call new config will be fetched according to new status
        UnitStatusUIConfig = null;
        // Set UI colors and text according to the configu
        // Set status text color
        UnitStatusText.color = UnitStatusUIConfig.statusTextColor;
        // Set status text
        UnitStatusText.text = UnitStatusUIConfig.statusDisplayName;
        // Set current health color
        GetUnitCurrentHealthText().color = UnitStatusUIConfig.currentHealthTextColor;
        // Set max health color
        GetUnitMaxHealthText().color = UnitStatusUIConfig.maxHealthTextColor;
        // Set canvas color
        GetUnitCanvasText().color = UnitStatusUIConfig.unitCanvasTextColor;
    }

    #endregion Unit Status and (De)Buffs

    public PartyPanel GetUnitPartyPanel()
    {
        // structure: 4PartyPanel-3Row-2UnitCell[Front/Back/Wide]-1UnitSlot-UnitCanvas(Unit)
        // verify if unit is member of party
        if (transform.parent.parent)
            if (transform.parent.parent.parent)
                return transform.parent.parent.parent.parent.GetComponent<PartyPanel>();
        return null;
    }

    void SetUnitNameUI()
    {
        // set Name
        transform.Find("Name").GetComponent<Text>().text = LPartyUnit.GetUnitDisplayName();
    }

    #region Unit Health

    void UpdateUnitCurrentHealthInfo()
    {
        Debug.Log("Updating " + LPartyUnit.UnitName + " unit current health");
        transform.Find("HPPanel/HPcurr").GetComponent<Text>().text = LPartyUnit.UnitHealthCurr.ToString();
    }

    void UpdateUnitMaxHealthInfo()
    {
        Debug.Log("Updating " + LPartyUnit.UnitName + " unit max health");
        transform.Find("HPPanel/HPmax").GetComponent<Text>().text = LPartyUnit.GetUnitEffectiveMaxHealth().ToString();
    }

    void UpdateUnitHealthInfo()
    {
        UpdateUnitCurrentHealthInfo();
        UpdateUnitMaxHealthInfo();
    }

    public Text GetUnitCurrentHealthText()
    {
        return transform.Find("HPPanel/HPcurr").GetComponent<Text>();
    }

    public Text GetUnitMaxHealthText()
    {
        return transform.Find("HPPanel/HPmax").GetComponent<Text>();
    }

    public void ResetUnitHealthToMax()
    {
        // this is done on lvl up
        LPartyUnit.UnitHealthCurr = LPartyUnit.GetUnitEffectiveMaxHealth();
        // update Health panel
        UpdateUnitHealthInfo();
    }

    #endregion Unit Health

    public void SetUnitCellInfoUI()
    {
        // Set unit name
        SetUnitNameUI();
        // Set unit health
        UpdateUnitHealthInfo();
        // Set unit status
        SetUnitStatus();
    }

    #region Unit Br Canvas
    public Text GetUnitCanvasText()
    {
        return GetUnitCell().Find("Br").GetComponent<Text>();
    }

    public void HighlightActiveUnitInBattle(bool doHighlight)
    {
        // Highlight
        Color highlightColor;
        if (doHighlight)
        {
            //Debug.Log(" HighlightActiveUnitInBattle");
            highlightColor = Color.white;
        }
        else
        {
            //Debug.Log(" Remove highlight from active unit in battle");
            highlightColor = Color.grey;
        }
        // highlight unit canvas with required color
        Text canvasText = GetUnitCell().Find("Br").GetComponent<Text>();
        canvasText.color = highlightColor;
    }
    #endregion Unit Br Canvas


    public void ApplyDestructiveAbility(int damageDealt)
    {
        int healthAfterDamage = LPartyUnit.UnitHealthCurr + damageDealt; // Note: damageDealt is negative normally
        // make sure that we do not set health less then 0
        if (healthAfterDamage <= 0)
        {
            healthAfterDamage = 0;
        }
        LPartyUnit.UnitHealthCurr = healthAfterDamage;
        // update current health in UI
        Text currentHealth = GetUnitCurrentHealthText();
        currentHealth.text = healthAfterDamage.ToString();
        // verify if unit is dead
        if (0 == healthAfterDamage)
        {
            // set unit is dead attribute
            LPartyUnit.UnitStatus = UnitStatus.Dead;
        }
        // display damage dealt in info panel
        UnitInfoPanelText.text = damageDealt.ToString() + " health";
        UnitInfoPanelText.color = Color.red;
    }

    public void RemoveAllBuffs()
    {
        //Debug.Log("RemoveAllBuffs");
        // in unit properties
        for (int i = 0; i < LPartyUnit.UnitBuffs.Length; i++)
        {
            LPartyUnit.UnitBuffs[i] = UnitBuff.None;
        }
        // in UI
        UnitBuffIndicator[] allBuffs = GetUnitBuffsPanel().GetComponentsInChildren<UnitBuffIndicator>();
        foreach (UnitBuffIndicator buff in allBuffs)
        {
            Destroy(buff.gameObject);
        }
    }

    public void RemoveAllDebuffs()
    {
        // in unit properties
        for (int i = 0; i < LPartyUnit.UnitDebuffs.Length; i++)
        {
            LPartyUnit.UnitDebuffs[i] = UnitDebuff.None;
        }
        // in UI
        UnitDebuffIndicator[] allDebuffs = GetUnitDebuffsPanel().GetComponentsInChildren<UnitDebuffIndicator>();
        foreach (UnitDebuffIndicator buff in allDebuffs)
        {
            Destroy(buff.gameObject);
        }
    }

    public void RemoveAllBuffsAndDebuffs()
    {
        RemoveAllBuffs();
        RemoveAllDebuffs();
    }

    public void DeactivateExpiredBuffs()
    {
        // Deactivate expired buffs in UI
        // PartyUnit unit = GetComponent<PartyUnit>();
        UnitBuffIndicator[] buffsUI = GetUnitBuffsPanel().GetComponentsInChildren<UnitBuffIndicator>();
        foreach (UnitBuffIndicator buffUI in buffsUI)
        {
            // First decrement buff current duration
            buffUI.DecrementCurrentDuration();
            // Verify if it has timed out;
            if (buffUI.GetCurrentDuration() == 0)
            {
                // buff has timed out
                // deactivate it (it will be destroyed at the end of animation)
                buffUI.SetActiveAdvance(false);
                // deactivate it in unit properties too
                LPartyUnit.UnitBuffs[(int)buffUI.GetUnitBuff()] = UnitBuff.None;
            }
        }
    }

    public void TriggerAppliedDebuffs()
    {
        //Debug.Log("TriggerAppliedDebuffs");
        UnitDebuffIndicator[] debuffsIndicators = GetUnitDebuffsPanel().GetComponentsInChildren<UnitDebuffIndicator>();
        //UnitDebuffsUI unitDebuffsUI = unit.GetUnitDebuffsPanel().GetComponent<UnitDebuffsUI>();
        foreach (UnitDebuffIndicator debuffIndicator in debuffsIndicators)
        {
            Debug.Log(name);
            // as long as we cannot initiate all debuffs at the same time
            // we add debuffs to the queue and they will be triggered one after another
            // CoroutineQueue queue = unitDebuffsUI.GetQueue();
            // CoroutineQueue queue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).Queue;
            //if (queue == null)
            //{
            //    Debug.LogError("No queue");
            //}
            //if (debuffIndicator == null)
            //{
            //    Debug.LogError("No debuffIndicator");
            //}
            IEnumerator coroutine = debuffIndicator.TriggerDebuff(this);
            //if (coroutine == null)
            //{
            //    Debug.LogError("No coroutine");
            //}
            CoroutineQueueManager.Run(coroutine);
            // Trigger debuff against player
            // Decrement buff current duration
            debuffIndicator.DecrementCurrentDuration();
        }
    }

    void UpdateUnitCellInfo()
    {
        if (LPartyUnit != null)
        {
            // set unit info in UI
            SetUnitCellInfoUI();
        }
    }

    Transform GetUnitEquipmentControl()
    {
        return transform.Find("UnitEquipmentControl");
    }

    void UpdateUnitEquipmentControl()
    {
        if (lPartyUnit != null)
        {
            // verify if unit is leader and edit party screen is active
            if (LPartyUnit.IsLeader && (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null))
            {
                // activate equipment button
                //Debug.LogWarning("Enable equipment button");
                GetUnitEquipmentControl().gameObject.SetActive(true);
            }
            else
            {
                // deactivate equipment button
                //Debug.LogWarning("Disable equipment button");
                GetUnitEquipmentControl().gameObject.SetActive(false);
            }
        }
        else
        {
            // deactivate equipment button
            //Debug.LogWarning("Disable equipment button");
            GetUnitEquipmentControl().gameObject.SetActive(false);
        }
    }

    void UpdateUpgradeUnitControl()
    {
        // verify if there is unit in slot and that edit party screen is active
        if ((LPartyUnit != null)  && (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null))
        {
            //Debug.LogWarning("Enable upgrade unit + button");
            transform.Find("UpgradeUnitControl").gameObject.SetActive(true);
        }
        else
        {
            //Debug.LogWarning("Disable upgrade unit + button");
            transform.Find("UpgradeUnitControl").gameObject.SetActive(false);
        }
    }

    public void ActOnItemDrop(InventoryItemDragHandler inventoryItemDragHandler)
    {
        Debug.Log("Act on Item Drop");
        // get item
        InventoryItem inventoryItem = inventoryItemDragHandler.LInventoryItem;
        // verify if item has active modifiers or usages
        if (inventoryItem.HasActiveModifiers())
        {
            Debug.Log("Apply item's UniquePowerModifier(s) and UnitStatModifier(s) to the party unit and its UI");
            // consume item and verify if it was successfull
            if (lPartyUnit.UseItem(inventoryItem))
            {
                // successfully consumed item
                // update UI based on changed party unit data
                UpdateUnitCellInfo();
                // item has run out of usages
                if (inventoryItem.LeftUsagesCount == 0)
                {
                    // there are no usages left
                    Debug.Log("Move item to the unit");
                    // move item to the unit, there are still might be non-instant upms and usms
                    inventoryItem.transform.SetParent(LPartyUnit.transform);
                    // Get source item slot transform
                    InventorySlotDropHandler srcItemSlot = inventoryItemDragHandler.ItemBeindDraggedSlot;
                    // verify if source slot is in party inventory mode
                    if (srcItemSlot.SlotMode == InventorySlotDropHandler.Mode.PartyInventory)
                    {
                        // Get PartyInventoryUI (before slot is destroyed)
                        PartyInventoryUI partyInventoryUI = srcItemSlot.GetComponentInParent<PartyInventoryUI>();
                        // remove all empty slots in inventory to fill in possible gaps after item consumption
                        partyInventoryUI.RemoveAllEmptySlots();
                        // fill in empty slots in inventory;
                        partyInventoryUI.FillInEmptySlots();
                    }
                }
                else
                {
                    // there are still usages left
                    Debug.Log("Clone item to the unit");
                    // clone item and place it into the unit
                    // so non-instant modifiers can be applied
                    // and assign inventoryItem varible with new inventory item
                    inventoryItem = Instantiate(inventoryItem.gameObject, this.transform).GetComponent<InventoryItem>();
                    // old inventory item, which was clonned, will return back to the inventory
                }
                // remove expired modifiers from triggered item
                inventoryItem.RemoveExpiredModifiers();
                // self-destory item on expire
                inventoryItem.SelfDestroyIfExpired();
            }
            else
            {
                // item cannot be consumed
                // item will return to its original position
            }
        }
        else
        {
            // item is not consumable
            // nothing to do here
            // item will return to its original position
            // this type of items should be placed into hero equipment
        }

    }

    #region Events Actions
    // called in UI in EventsListener
    public void OnPartyUnitStatusChange(PartyUnit changedPartyUnit)
    {
        // verify if changed party unit is the same as current
        if (changedPartyUnit == LPartyUnit)
        {
            SetUnitStatus();
        }
    }

    public void OnCurrentHealthChanged(PartyUnit partyUnit, int difference)
    {
        // verify if party unit is this
        if (partyUnit == LPartyUnit)
        {
            // Update unit current health info
            UpdateUnitCurrentHealthInfo();
            // verify if difference is > 0
            if (difference > 0)
            {
                // set "+" symbol
                UnitInfoPanelText.text = "+";
            }
            else
            {
                // reset current text
                UnitInfoPanelText.text = "";
            }
            // Display difference in info panel
            UnitInfoPanelText.text += difference.ToString();
            // set text color transparent, so that animation, if it is available, will take care of the rest
            UnitInfoPanelText.color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0);
        }
    }

    public void OnMaxHealthChanged(PartyUnit partyUnit)
    {
        // verify if party unit is this
        if (partyUnit == LPartyUnit)
        {
            UpdateUnitMaxHealthInfo();
        }
    }

    public void OnAbilityApply(PartyUnit appliedToPartyUnit, UnitAbility unitAbility)
    {
        // verify if party unit to which modifier has been applied is the same as current
        if (appliedToPartyUnit == LPartyUnit)
        {
            unitAbility.textAnimation.Run(UnitInfoPanelText);
        }
    }

    public void OnUnitPowerModifierApply(PartyUnit appliedToPartyUnit, UnitPowerModifier unitPowerModifier)
    {
        // verify if party unit to which modifier has been applied is the same as current
        if (appliedToPartyUnit == LPartyUnit)
        {
            // get battle screen queue
            //CoroutineQueue coroutineQueue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).Queue;
            // display unit power modifier animation
            //unitPowerModifier.textAnimation.Run(UnitInfoPanelText, coroutineQueue);
            unitPowerModifier.textAnimation.Run(UnitInfoPanelText);
        }
    }
    #endregion Events Actions

    #region Properties
    UnitStatusUIConfig unitStatusUIConfig;
    public UnitStatusUIConfig UnitStatusUIConfig
    {
        get
        {
            // verify if unit status UI config is not initialized yet
            if (unitStatusUIConfig == null)
            {
                // Get and set Unit status UI config
                unitStatusUIConfig = Array.Find(ConfigManager.Instance.UnitStatusUIConfigs, e => e.unitStatus == LPartyUnit.UnitStatus);
            }
            return unitStatusUIConfig;
            //// Get fresh unit status config, because Linked Party Unit may change and its status may change too
            //return Array.Find(ConfigManager.Instance.UnitStatusUIConfigs, e => e.unitStatus == LPartyUnit.UnitStatus);
        }

        set
        {
            unitStatusUIConfig = value;
        }
    }

    #endregion Properties
}
