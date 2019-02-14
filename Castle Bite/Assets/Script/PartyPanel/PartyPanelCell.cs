using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyPanelCell : MonoBehaviour
{
    [SerializeField]
    PartyPanel.Cell cell;
    // .. remove canvasText
    [SerializeField]
    Text canvasText;
    [SerializeField]
    TextMeshProUGUI canvasTMPro;

    Material standardCanvasMaterial;
    Material highlightedCanvasMaterial;

    private Color beforeItemDragColor;
    private PartyUnitUI activePartyUnitUI;

    public PartyPanel.Cell Cell
    {
        get
        {
            return cell;
        }
    }

    //public Text CanvasText
    //{
    //    get
    //    {
    //        return canvasText;
    //    }
    //}

    public TextMeshProUGUI CanvasText
    {
        get
        {
            return canvasTMPro;
        }
    }

    public PartyUnitUI ActivePartyUnitUI
    {
        get
        {
            return activePartyUnitUI;
        }
    }

    public Material StandardCanvasMaterial
    {
        get
        {
            // verify it it has not been assigned yet
            if (standardCanvasMaterial == null)
            {
                // get material from resources
                standardCanvasMaterial = Resources.Load("Fonts and Materials/cour bold Normal SDF") as Material;
            }
            // return material
            return standardCanvasMaterial;
        }
    }

    public Material HighlightedCanvasMaterial
    {
        get
        {
            // verify it it has not been assigned yet
            if (highlightedCanvasMaterial == null)
            {
                // get material from resources
                highlightedCanvasMaterial = Resources.Load("Fonts and Materials/cour bold Hilighted SDF") as Material;
            }
            // return material
            return highlightedCanvasMaterial;
        }
    }

    public bool IsOccupied()
    {
        // verify if cell has party unit UI
        if (GetComponentInChildren<PartyUnitUI>() != null)
        {
            return true;
        }
        return false;
    }

    public void OnBeginItemDrag()
    {
        // Debug.LogWarning("OnBeginItemDrag");
        // save original color
        // Debug.LogWarning("Save original color");
        beforeItemDragColor = CanvasText.color;
        // get UPM config
        UniquePowerModifierConfig uniquePowerModifierConfig = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.PrimaryUniquePowerModifierConfig;
        // verify if item is usable
        if (uniquePowerModifierConfig.AreRequirementsMetInContextOf(InventoryItemDragHandler.itemBeingDragged.LInventoryItem, this))
        {
            Debug.Log("Cell Requirements are met");
            // verify if party unit is not present, because if it is present, then we need to give it a possibility to override "applicability"
            // it maybe already overritten, because PartyUnitUI could possibly react earlier on event
            // get party unit UI
            PartyUnitUI partyUnitUI = GetComponentInChildren<PartyUnitUI>();
            // verify if its not null
            if (partyUnitUI != null)
            {
                // let party unit override highlights and react on begin item drag event
                partyUnitUI.ActOnBeginItemDrag();
            }
            else
            {
                // highlight with applicable color
                // CanvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
                CanvasText.color = uniquePowerModifierConfig.UniquePowerModifierUIConfig.ValidationUIConfig.upmIsApplicableForUnitSlotColor;
            }
        }
        else
        {
            Debug.Log("Cell Requirements are not met");
            // highlight with not applicable color
            // CanvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
            CanvasText.color = uniquePowerModifierConfig.UniquePowerModifierUIConfig.ValidationUIConfig.upmIsNotApplicableForUnitSlotColor;
        }
        //// verify if item has active modifiers or usages
        //if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.HasActiveModifiers())
        //{
        //    Debug.Log("Item has active modifiers");
        //    // get party unit UI
        //    PartyUnitUI partyUnitUI = GetComponentInChildren<PartyUnitUI>();
        //    // verify if its not null
        //    if (partyUnitUI != null)
        //    {
        //        Debug.Log("Found partyUnitUI");
        //        // activate highlight
        //        // get source context 
        //        // try to get party unit (assume that during battle unit can only use items which are located in (childs of) this unit game object)
        //        // if outside of the battle or if item is dragged from inventiry, then this will result in null
        //        System.Object srcContext = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.GetComponentInParent<PartyUnit>();
        //        // verify if srcPartyUnit is null
        //        if (srcContext == null)
        //        {
        //            // context is hero party (item is dragged from inventory)
        //            // get party
        //            HeroParty heroParty = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.GetComponentInParent<HeroParty>();
        //            // verify if party is garnizon type
        //            if (heroParty.PartyMode == PartyMode.Garnizon)
        //            {
        //                // set context to the city
        //                srcContext = heroParty.GetComponentInParent<City>();
        //            }
        //            else
        //            {
        //                // party mode = normal party
        //                // set context to the party leader
        //                srcContext = heroParty.GetPartyLeader();
        //            }
        //        }
        //        // verify if UPM can be applied to destination unit
        //        if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].AreRequirementsMetInContextOf(srcContext, partyUnitUI.LPartyUnit) )
        //        {
        //            Debug.Log("Requirements are met");
        //            // verify if it is advised to use this item in this context
        //            if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].IsItAdvisedToActInContextOf(srcContext, partyUnitUI.LPartyUnit))
        //            {
        //                Debug.Log("Advised");
        //                // advised
        //                // item can be applied to this hero, highlight with applicable color
        //                canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
        //            }
        //            else
        //            {
        //                Debug.Log("Not Advised");
        //                // not advised
        //                // item can be applied to this hero, highlight with applicable color
        //                canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableButNotAdvisedForUnitSlotColor;
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("Requirements are not met");
        //            // item cannot be applied to this hero, highlight with not applicable color
        //            canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //        }
        //        //// try to consume item in preview mode without actually doing anything
        //        //if (partyUnitUI.LPartyUnit.UseItem(InventoryItemDragHandler.itemBeingDragged.LInventoryItem, true))
        //        //{
        //        //    // item can be applied to this hero, highlight with applicable color
        //        //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableForUnitSlotColor;
        //        //}
        //        //else
        //        //{
        //        //    // item cannot be applied to this hero, highlight with not applicable color
        //        //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //        //}
        //    }
        //    else
        //    {
        //        Debug.Log("no party unit UI");
        //        // there is no hero in this slot, highlight with not applicable color
        //        canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //    }
        //}
        //else
        //{
        //    Debug.Log("Item has no active modifiers");
        //    // item is not consumable, highlight with not applicable color
        //    canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableForUnitSlotColor;
        //}
    }

    public void OnEndItemDrag()
    {
        // Debug.LogWarning("Restore original color");
        // verify if edit party screen is active (note: this is not needed during battle screen
        // Debug.LogWarning("(Check if fixed) Fix unit may change its status (die). Reinitialize highlighs on EndItemDrag()");
        UIManager uiManager = transform.root.GetComponentInChildren<UIManager>();
        // verify if EditPartyScreen is active
        if (uiManager.GetComponentInChildren<EditPartyScreen>(false) != null)
        {
            Debug.LogWarning(".. avoid this check");
            // restore original color
            CanvasText.color = beforeItemDragColor;
        }
    }

    bool GetIsTargetableAndHighlightUnitCanvasBasedOnUPMConfigApplicability(UniquePowerModifierConfig uniquePowerModifierConfig)
    {
        // verify if active party unit ability is applicable to this cell (example: summon) and party unit (if it is present)
        if (uniquePowerModifierConfig.AreRequirementsMetInContextOf(activePartyUnitUI.GetComponentInParent<PartyPanelCell>(), this))
        {
            Debug.Log("Cell Requirements are met");
            // verify if party unit is not present, because if it is present, then we need to give it a possibility to override "applicability"
            // it maybe already overritten, because PartyUnitUI could possibly react earlier on event
            // get party unit UI
            PartyUnitUI partyUnitUI = GetComponentInChildren<PartyUnitUI>();
            // verify if its not null
            if (partyUnitUI != null)
            {
                // let party unit override highlights and react on begin item drag event
                return partyUnitUI.ActOnBattleNewUnitHasBeenActivatedEvent(activePartyUnitUI);
            }
            else
            {
                // highlight with applicable color
                CanvasText.color = uniquePowerModifierConfig.UniquePowerModifierUIConfig.ValidationUIConfig.upmIsApplicableForUnitSlotColor;
                // targetable
                return true;
            }
        }
        else
        {
            Debug.Log("Cell Requirements are not met");
            // highlight with not applicable color
            CanvasText.color = uniquePowerModifierConfig.UniquePowerModifierUIConfig.ValidationUIConfig.upmIsNotApplicableForUnitSlotColor;
            // not targetable
            return false;
        }
    }

    public void OnBattleNewUnitHasBeenActivatedEvent(System.Object context)
    {
        // init error message (if cell is not targetable)
        // .. set it dynamically based on limiter triggered
        string errorMessage = "Cannot target this cell";
        // init is targetable
        bool isTargetable = false;
        // verify if context is correct
        if (context is PartyUnitUI)
        {
            // init and cache newly activated party unit UI from context
            activePartyUnitUI = (PartyUnitUI)context;
            // reset battle context values (we don't want to have previously cached targeted party unit slot and upm index)
            BattleContext.Reset();
            // cache active unit in battle context
            BattleContext.ActivePartyUnitUI = activePartyUnitUI;
            // get UPM config
            // UniquePowerModifierConfig uniquePowerModifierConfig = activePartyUnitUI.LPartyUnit.UnitAbilityConfig.primaryUniquePowerModifierConfig;
            UniquePowerModifierConfig uniquePowerModifierConfig = activePartyUnitUI.LPartyUnit.UnitAbilityConfig.PrimaryUniquePowerModifierConfig;
            // Highlight unit canvas based on whether UPM is applicable to this unit or not
            isTargetable = GetIsTargetableAndHighlightUnitCanvasBasedOnUPMConfigApplicability(uniquePowerModifierConfig);
            // Activate/Deactive HighlightUnitCanvas
            // transform.Find("HighlightUnitCanvas").gameObject.SetActive((activePartyUnitUI.GetComponentInParent<PartyPanelCell>().gameObject.GetInstanceID() == this.gameObject.GetInstanceID()));
            bool isThisActiveUnitCell = activePartyUnitUI.GetComponentInParent<PartyPanelCell>().gameObject.GetInstanceID() == this.gameObject.GetInstanceID();
            if (isThisActiveUnitCell)
            {
                CanvasText.fontSharedMaterial = HighlightedCanvasMaterial;
            }
            else
            {
                // CanvasText.fontSharedMaterial.DisableKeyword(ShaderUtilities.Keyword_Glow);
                CanvasText.fontSharedMaterial = StandardCanvasMaterial;
            }
        }
        // set UnitSlot in cell as targetable or not
        GetComponentInChildren<UnitSlot>().SetOnClickAction(isTargetable, errorMessage);
    }

    void OnApplyAbilityFromUnitUIToUnitCell(PartyUnitUI activePartyUnitUI, PartyPanelCell partyPanelCell)
    {
        Debug.LogWarning(".. OnApplyAbilityFromUnitUIToUnitCell");
    }

    void Test(BattleContext battleContext)
    {

    }

    public void OnBattleApplyActiveUnitAbility(System.Object context)
    {
        Debug.LogWarning("OnBattleApplyActiveUnitAbility");
        // verify if context is correct
        if (context is UnitSlot)
        {
            // init unit slot from context
            UnitSlot unitSlot = (UnitSlot)context;
            // cache target unit slot in battle context (unit which has been targeted by ability)
            BattleContext.TargetedUnitSlot = unitSlot;
            // cache this unit slot as destination unit slot in battle context
            BattleContext.DestinationUnitSlot = GetComponentInChildren<UnitSlot>();
            // get active party unit in battle
            PartyUnit activePartyUnit = BattleContext.ActivePartyUnitUI.LPartyUnit;
            // get active party unit ability config
            UnitAbilityConfig activeUnitAbilityConfig = activePartyUnit.UnitAbilityConfig;
            // get active party unit ability upms
            List<UniquePowerModifierConfig> activeUnitUniquePowerModifierConfigsSortedByExecutionOrder = activeUnitAbilityConfig.UniquePowerModifierConfigsSortedByExecutionOrder;
            // loop through all UPM configs in active party unit ability
            for (int i = 0; i < activeUnitUniquePowerModifierConfigsSortedByExecutionOrder.Count; i++)
            {
                Debug.LogWarning("UPM: " + "[" + i + "] " + activeUnitUniquePowerModifierConfigsSortedByExecutionOrder[i].DisplayName);
                // set BattleContext
                BattleContext.ActivatedUPMConfigIndex = i;
                // verify if UPM can be applied to unit in this party unit UI
                if (activeUnitUniquePowerModifierConfigsSortedByExecutionOrder[i].AreRequirementsMetInContextOf(BattleContext.Instance))
                {
                    // set unique power modifier ID
                    BattleContext.UniquePowerModifierID = new UniquePowerModifierID()
                    {
                        unitAbilityID = activeUnitAbilityConfig.unitAbilityID,
                        uniquePowerModifierConfigIndex = i,
                        modifierOrigin = ModifierOrigin.Ability,
                        destinationGameObjectID = this.gameObject.GetInstanceID()
                    };
                    // Apply UPM
                    activeUnitUniquePowerModifierConfigsSortedByExecutionOrder[i].Apply(BattleContext.Instance);
                    // Run UPM animation
                    activeUnitUniquePowerModifierConfigsSortedByExecutionOrder[i].UniquePowerModifierUIConfig.UniquePowerModifierAnimation.Run(BattleContext.Instance);
                    // .. make it more generic and party cell animated too (on summon)
                    //// get party unit UI
                    //PartyUnitUI partyUnitUI = GetComponentInChildren<PartyUnitUI>();
                    //// verify if there is a party unit UI
                    //if (partyUnitUI != null)
                    //{
                    //    // run text animation
                    //    activeUnitUniquePowerModifierConfigsSortedByExecutionOrder[i].UniquePowerModifierUIConfig.OnTriggerUPMTextAnimation.Run(partyUnitUI.UnitInfoPanelText);
                    //}
                }
                else
                {
                    // update highlights (in case if it is single-unit scope upm, then we should remove highlight for others)
                    // highlight with not applicable color
                    CanvasText.color = activeUnitUniquePowerModifierConfigsSortedByExecutionOrder[i].UniquePowerModifierUIConfig.ValidationUIConfig.upmIsNotApplicableForUnitSlotColor;
                }
            }
            // get destination PartyUnitUI
            // PartyUnitUI targetPartyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
            //// get cell PartyUnitUI
            //PartyUnitUI thisCellPartyUnitUI = GetComponentInChildren<PartyUnitUI>();
            //// verify if party unit UI is not null
            //if (thisCellPartyUnitUI != null)
            //{
            //    // thisCellPartyUnitUI.OnApplyAbilityFromUnitUIToUnitUI(activePartyUnitUI, targetPartyUnitUI);
            //    thisCellPartyUnitUI.ActOnApplyAbilityToThisPartyUnitUI();
            //}
            //else
            //{
            //    // this is probably summon
            //    OnApplyAbilityFromUnitUIToUnitCell(activePartyUnitUI, this);
            //}
        }
    }
}
