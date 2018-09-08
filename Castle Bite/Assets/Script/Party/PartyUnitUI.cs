using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyUnitUI : MonoBehaviour {
    [SerializeField]
    PartyUnit lPartyUnit;
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
    }

    void OnDisable()
    {
        // Remove required PartyUnit handlers
        SetPartyUnitHandlersActive(false);
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

    public Transform GetUnitRow()
    {
        // structure: 3[Top/Middle/Bottom]Row-2UnitCell[Front/Back/Wide]-1UnitSlot-UnitCanvas(this with link to Unit)
        return transform.parent.parent.parent;
    }

    #region Unit Info Panel

    public Text GetUnitInfoPanelText()
    {
        return transform.Find("InfoPanel").GetComponent<Text>();
    }

    // Note: animation should be identical to the function with the same name in PartyPanel
    public void FadeUnitCellInfo(float alpha)
    {
        Text infoPanel = GetUnitInfoPanelText();
        Color c = infoPanel.color;
        c.a = alpha;
        infoPanel.color = c;
    }

    public void ClearUnitInfoPanel()
    {
        //Color32 defaultColor = new Color32(180, 180, 180, 255);
        Text infoPanelTxt = GetUnitInfoPanelText();
        infoPanelTxt.text = "";
        infoPanelTxt.color = defaultUnitInfoColor;
    }

    #endregion Unit Info Panel

    #region Unit Status and (De)Buffs

    public Text GetUnitStatusText()
    {
        return transform.Find("UnitStatus").GetComponent<Text>();
    }

    public Transform GetUnitBuffsPanel()
    {
        return transform.Find("UnitStatus/Buffs");
    }

    public Transform GetUnitDebuffsPanel()
    {
        return transform.Find("UnitStatus/Debuffs");
    }

    public string GetUnitCellAddress()
    {
        // structure: PartyPanel-3[Top/Middle/Bottom]-2[Front/Back/Wide]-1UnitSlot-PartyUnitUI(this)
        return transform.parent.parent.parent.name + "/" + transform.parent.parent.name;
    }

    public void ClearPartyUnitStatusUI()
    {
        //Color32 defaultColor = new Color32(180, 180, 180, 255);
        Text infoPanelTxt = GetUnitStatusText();
        infoPanelTxt.text = "";
        infoPanelTxt.color = defaultUnitStatusColor;
    }

    public void SetUnitStatus(UnitStatus value)
    {
        Debug.Log("Set unit " + LPartyUnit.UnitName + " status " + value.ToString());
        LPartyUnit.UnitStatus = value;
        // get new UI color according ot unit status
        Color32 newUIColor;
        // set dead in status
        string statusString;
        switch (value)
        {
            case UnitStatus.Active:
                newUIColor = new Color32(160, 160, 160, 255);
                statusString = "";
                break;
            case UnitStatus.Waiting:
                newUIColor = new Color32(96, 96, 96, 96);
                statusString = "Waiting";
                break;
            case UnitStatus.Escaping:
                newUIColor = new Color32(96, 96, 96, 255);
                statusString = "Escaping";
                break;
            case UnitStatus.Escaped:
                newUIColor = new Color32(64, 64, 64, 255);
                statusString = "Escaped";
                // clear unit buffs and debuffs
                RemoveAllBuffsAndDebuffs();
                break;
            case UnitStatus.Dead:
                newUIColor = new Color32(64, 64, 64, 255);
                statusString = "Dead";
                // clear unit buffs and debuffs
                RemoveAllBuffsAndDebuffs();
                break;
            default:
                Debug.LogError("Unknown status " + value.ToString());
                newUIColor = Color.red;
                statusString = "Error";
                break;
        }
        GetUnitCurrentHealthText().color = newUIColor;
        GetUnitMaxHealthText().color = newUIColor;
        GetUnitCanvasText().color = newUIColor;
        GetUnitStatusText().color = newUIColor;
        GetUnitStatusText().text = statusString;
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

    void SetUnitHealthUI()
    {
        // set Health
        transform.Find("HPPanel/HPcurr").GetComponent<Text>().text = LPartyUnit.UnitHealthCurr.ToString();
        transform.Find("HPPanel/HPmax").GetComponent<Text>().text = LPartyUnit.GetUnitEffectiveMaxHealth().ToString();
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
        SetUnitHealthUI();
    }

    #endregion Unit Health

    public void SetUnitCellInfoUI()
    {
        // Set unit name
        SetUnitNameUI();
        // Set unit health
        SetUnitHealthUI();
        // Set unit status
        SetUnitStatus(LPartyUnit.UnitStatus);
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
        int healthAfterDamage = LPartyUnit.UnitHealthCurr - damageDealt;
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
            SetUnitStatus(UnitStatus.Dead);
        }
        // display damage dealt in info panel
        Text infoPanel = GetUnitInfoPanelText();
        infoPanel.text = "-" + damageDealt + " health";
        infoPanel.color = Color.red;
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
            CoroutineQueue queue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).GetQueue();
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
            queue.Run(coroutine);
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
            // verify if unit is leader
            if (LPartyUnit.IsLeader)
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
        // verify if there is unit in slot
        if (LPartyUnit != null)
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

    public void TryToConsumeItem(InventoryItemDragHandler inventoryItemDragHandler)
    {
        Debug.Log("Try to consume item");
        // get item
        InventoryItem inventoryItem = inventoryItemDragHandler.LInventoryItem;
        // verify if item is consumable
        if (inventoryItem.MaxUsagesCount >= 1)
        {
            Debug.Log("Apply item's UniquePowerModifier(s) and UnitStatModifier(s) to the party unit and its UI");
            // consume item and verify if it was successfull
            if (lPartyUnit.ConsumeItem(inventoryItem))
            {
                // successfully consumed item
                // update UI based on changed party unit data
                UpdateUnitCellInfo();
                // Get source item slot transform
                InventorySlotDropHandler srcItemSlot = inventoryItemDragHandler.ItemBeindDraggedSlot;
                // verify if source slot is in party inventory mode
                if (srcItemSlot.SlotMode == InventorySlotDropHandler.Mode.PartyInventory)
                {
                    // Get PartyInventoryUI (before slot is destroyed)
                    PartyInventoryUI partyInventoryUI = srcItemSlot.GetComponentInParent<PartyInventoryUI>();
                    //// remove source item slot
                    //Destroy(srcItemSlot.gameObject);
                    // remove all empty slots in inventory to fill in possible gaps after item consumption
                    partyInventoryUI.RemoveAllEmptySlots();
                    // fill in empty slots in inventory;
                    partyInventoryUI.FillInEmptySlots();
                }
            }
            else
            {
                // item cannot be consumed
                // nothing to do here
                // item will return to its original position
            }
        }
        else
        {
            // item is not consumable
            // nothing to do here
            // item will return to its original position
        }
    }
}
