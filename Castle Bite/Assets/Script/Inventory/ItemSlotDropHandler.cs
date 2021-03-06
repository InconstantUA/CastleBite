﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlotDropHandler : MonoBehaviour, IDropHandler {
    //public enum Mode
    //{
    //    PartyInventory,
    //    HeroEquipment
    //}
    //[SerializeField]
    //Mode slotMode;
    [SerializeField]
    HeroEquipmentSlots equipmentSlot;
    [SerializeField]
    Text canvasText;
    [SerializeField]
    Text canvasMessageText;
    [SerializeField]
    protected GameEvent itemHasBeenDroppedIntoTheItemSlotEvent;
    //[SerializeField]
    //GameEvent itemHasBeenDroppedIntoEquipmentSlotEvent;
    //[SerializeField]
    //GameEvent itemHasBeenDroppedIntoInventorySlotEvent;


    bool isDroppable = true;
    private Color beforeBeginItemDragColor;

    //public Mode SlotMode
    //{
    //    get
    //    {
    //        return slotMode;
    //    }
    //}

    public HeroEquipmentSlots EquipmentSlot
    {
        get
        {
            return equipmentSlot;
        }

        set
        {
            equipmentSlot = value;
        }
    }

    public bool IsDroppable
    {
        set
        {
            isDroppable = value;
        }
    }

    //GameObject Item
    //{
    //    get
    //    {
    //        if (transform.childCount > 1)
    //        {
    //            return transform.GetChild(1).gameObject;
    //        }
    //        return null;
    //    }
    //}

    public virtual Transform GetParentObjectTransform()
    {
        //// verify if this hero Equipment or Party inventory slot
        //if (slotMode == Mode.HeroEquipment)
        //{
        //    // item being dragged to the party leader (hero)
        //    // get hero (party leader)
        //    // structure: 2UnitCanvas[PartyUnitUI->LPartyUnit]-1UnitEquipmentControl-EquipmentButton
        //    return GetComponentInParent<HeroEquipment>().LUnitEquipmentButton.transform.parent.parent.GetComponent<PartyUnitUI>().LPartyUnit.transform;
        //}
        //else if (slotMode == Mode.PartyInventory)
        //{
        //    // get HeroParty
        //    return GetComponentInParent<HeroPartyUI>().LHeroParty.transform;
        //} else
        //{
        //    Debug.LogError("Unknown slot mode: " + slotMode.ToString());
        //    return null;
        //}
        Debug.LogError("This function should not be called. Instead call should be made to EquipmentSlotDropHandler or InventorySlotDropHandler");
        return null;
    }

    public void PutItemIntoSlot(InventoryItemDragHandler itemBeingDragged)
    {
        // move item being dragged UI into this slot
        itemBeingDragged.SetParent(this);
        //// reset UI position to 0/0/0/0
        //itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // [ left - bottom ]
        //itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // [ right - top ]
        //// verify if we are in edit party screen
        //if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
        //{
        //    // move InventoryItem to the new parent object
        //    itemBeingDragged.LInventoryItem.transform.SetParent(GetParentObjectTransform());
        //}
        //// verify if we are in battle screen
        //else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
        //{
        //    // do not move InventoryItem to the new parent object, because this is item equipped on a hero
        //}
        //else
        //{
        //    Debug.LogError("Unknonw active screen");
        //}
        //Debug.LogError("remove below");
        // verify if this is hero eqiupment slot
        //if (Mode.HeroEquipment == slotMode)
        //{
        //    //itemHasBeenDroppedIntoEquipmentSlotEvent.Raise(this);
        //}
        //else
        //{
        //    // Mode.PartyInventory
        //    itemHasBeenDroppedIntoInventorySlotEvent.Raise(this);
        //}
        // raise event
        itemHasBeenDroppedIntoTheItemSlotEvent.Raise(this);
    }

    public void ExchangeWithSlotOnDrop(ItemSlotDropHandler destinationSlot)
    {
        // this is an item being dragged slot
        // get item drag handler in destination slot
        InventoryItemDragHandler destinationItemDragHandler = destinationSlot.GetComponentInChildren<InventoryItemDragHandler>();
        // move item being dragged (source) into destination slot
        InventoryItemDragHandler.itemBeingDragged.SetParent(destinationSlot);
        // move destination drag handler into this slot
        destinationItemDragHandler.SetParent(this);
        // raise events
        itemHasBeenDroppedIntoTheItemSlotEvent.Raise(this);
        itemHasBeenDroppedIntoTheItemSlotEvent.Raise(destinationSlot);
    }

    public virtual void MoveItemIntoThisSlot()
    {
        //// Get source item slot transform
        //ItemSlotDropHandler srcItemSlot = InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot;
        //// init exchange flag
        //bool thisIsExachnge = false;
        //// init destination slot variable with this slot
        //ItemSlotDropHandler dstItemSlot = this;
        //// Get item in this slot
        //InventoryItemDragHandler itemInThisSlot = GetComponentInChildren<InventoryItemDragHandler>();
        //// verify if there is no item already in this slot
        //if (itemInThisSlot != null)
        //{
        //    // verify if source is equipment slot and destination is party inventory slot
        //    if ((srcItemSlot.SlotMode == Mode.HeroEquipment) && (slotMode == Mode.PartyInventory))
        //    {
        //        // do not do exchange, just move item into inventory into the new slot
        //        // create new slot and set it as destination
        //        dstItemSlot = GetComponentInParent<PartyInventoryUI>().AddSlot();
        //    }
        //    else
        //    {
        //        thisIsExachnge = true;
        //        // Put item from this slot to the slot of the item beind dragged
        //        srcItemSlot.PutItemIntoSlot(itemInThisSlot);
        //        //// verify if srcItemSlot is equipment slot
        //        //if (srcItemSlot.SlotMode == Mode.HeroEquipment)
        //        //{
        //        //    // change item equipment slot address to the destination slot
        //        //    itemInThisSlot.LInventoryItem.CurrentHeroEquipmentSlot = srcItemSlot.EquipmentSlot;
        //        //}
        //    }
        //    // trigger event
        //    // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
        //}
        //// Put dragged item into slot
        //dstItemSlot.PutItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
        //// verify if it was not just simple exchange
        //if (!thisIsExachnge)
        //{
        //    // verify if source slot is in party inventory mode
        //    if (srcItemSlot.SlotMode == Mode.PartyInventory)
        //    {
        //        // .. Optimize
        //        // remove all empty slots in inventory
        //        srcItemSlot.GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
        //        // fill in empty slots in inventory
        //        srcItemSlot.GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
        //    }
        //    // verify if this is equipment slot
        //    else if ((srcItemSlot.SlotMode == Mode.HeroEquipment)
        //    // and that battle screen is active
        //        && (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null))
        //    {
        //        // remove all empty slots in inventory
        //        srcItemSlot.GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
        //        // fill in empty slots in inventory
        //        srcItemSlot.GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
        //    }
        //    // verify if destination slot was changed to the other from this slot
        //    if (dstItemSlot.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        //    {
        //        // trigger this party inventory reorganisation
        //        // remove all empty slots in this inventory
        //        GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
        //        // fill in empty slots in inventory
        //        GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
        //    }
        //}
        Debug.LogError("This function should not be called. Instead call should be made to EquipmentSlotDropHandler or InventorySlotDropHandler");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // verify if it is item being dragged and not other object
        if (InventoryItemDragHandler.itemBeingDragged != null)
        {
            // verify if slot is droppable for this item (this is being set during drag initiation)
            if (isDroppable)
            {
                // move item into slot
                MoveItemIntoThisSlot();
                //// verify if this is hero eqiupment slot
                //if (Mode.HeroEquipment == slotMode)
                //{
                //    itemHasBeenDroppedIntoEquipmentSlotEvent.Raise(this);
                //}
                //else
                //{
                //    // Mode.PartyInventory
                //    itemHasBeenDroppedIntoInventorySlotEvent.Raise(this);
                //}
                //// verify if we are in edit screen mode
                //if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
                //{
                //    // verify if this is hero eqiupment slot
                //    if (Mode.HeroEquipment == slotMode)
                //    {
                //        // change item equipment slot address
                //        InventoryItemDragHandler.itemBeingDragged.LInventoryItem.CurrentHeroEquipmentSlot = equipmentSlot;
                //        // update unit info UI
                //        transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>().ActivateAdvance(GetComponentInParent<HeroEquipment>().LPartyUnit, UnitInfoPanel.Align.Right, false, UnitInfoPanel.ContentMode.Short);
                //    }
                //    else
                //    {
                //        // change item position parameter
                //        InventoryItemDragHandler.itemBeingDragged.LInventoryItem.CurrentHeroEquipmentSlot = HeroEquipmentSlots.None;
                //    }
                //}
                //// verify if we are in battle screen mode
                //else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
                //{
                //    // take equipment slot parameter from item (in case if new slot has been created or empty slot has been reused)
                //    equipmentSlot = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.CurrentHeroEquipmentSlot;
                //}
                //else
                //{
                //    Debug.LogError("Unknown active screen");
                //}
            }
        }
    }

    public void OnBeginItemDrag()
    {
        // save original slot color
        beforeBeginItemDragColor = canvasText.color;
        // init isCompatible with this slot item flag
        bool isCompatible = false;
        // reset is droppable flag
        // verify if equipment slot is compatible
        if ((InventoryItemDragHandler.itemBeingDragged.LInventoryItem.CompatibleEquipmentSlots & EquipmentSlot) == EquipmentSlot)
        {
            // set context
            EquipmentScreenContext.DestinationItemSlotDropHandler = this;
            // verify item is compatible with hero (all required prerequisites are net)
            ModifierLimiter.ValidationResult validationResult = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.PrimaryUniquePowerModifierConfig.AreRequirementsMetInContextOf(GameContext.Context);
            // if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.uniquePowerModifierConfigs[0].AreRequirementsMetInContextOf(null, GetComponentInParent<HeroEquipment>().LPartyUnit))
            // if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.PrimaryUniquePowerModifierConfig.AreRequirementsMetInContextOf(null, this))
            if (!validationResult.doDiscardModifier)
            {
                // set compatible flag
                isCompatible = true;
            }
            else
            {
                // set cavast message
                // Debug.LogError(".. uncomment below and fix");
                //canvasMessageText.text = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.PrimaryUniquePowerModifierConfig.GetLimiterMessageInContextOf(null, GetComponentInParent<HeroEquipment>().LPartyUnit);
                canvasMessageText.text = validationResult.message;
                // set highlight color to not the color which indicates that prerequisites are not met
                canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemPrerequsitesAreNotMetForThisUnitColor;
            }
            //// verify if this is shard slot
            //if (EquipmentSlot == HeroEquipmentSlots.Shard)
            //{
            //    // for shard slot we need to verify if party leader has skill at least 1st level
            //    if (Array.Find(GetComponentInParent<HeroEquipment>().LPartyUnit.UnitSkillsData, element => element.unitSkill == UnitSkillID.ShardAura).currentSkillLevel >= 1)
            //    {
            //        // set compatible flag
            //        isCompatible = true;
            //    }
            //    else
            //    {
            //        // set highlight color to not the color which indicates that prerequisites are not met
            //        canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemPrerequsitesAreNotMetForThisUnitColor;
            //        // set cavast message
            //        canvasMessageText.text = "Requires " + ConfigManager.Instance[UnitSkillID.ShardAura].skillDisplayName + " skill";
            //    }
            //}
            //else
            //{
            //    // set compatible flag
            //    isCompatible = true;
            //}
        }
        else
        {
            // set highlight color to not applicable for this equipment slot
            canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotCompatibleWithEquipmentSlotColor;
        }
        // verify if slot is compatible
        if (isCompatible)
        {
            // set droppable flag
            IsDroppable = true;
            // set highlight color to applicable
            canvasText.color = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemCanBeEquippedColor;
        }
        else
        {
            // unset droppable flag
            IsDroppable = false;
        }
        // reset normal button color
        canvasText.GetComponent<TextButton>().NormalColor = canvasText.color;
    }

    public void OnEndItemDrag()
    {
        // set color in UI
        canvasText.color = beforeBeginItemDragColor;
        // reset normal button color
        canvasText.GetComponent<TextButton>().NormalColor = canvasText.color;
        // reset slot is droppable status
        IsDroppable = false;
        // reset canvas message text
        canvasMessageText.text = "";
    }

    //void OnTransformChildrenChanged()
    //{
    //    Debug.Log("On change");
    //}

    //public void OnDrop(PointerEventData eventData)
    //{
    //    RectTransform invPanel = transform as RectTransform;
    //    if(!RectTransformUtility.RectangleContainsScreenPoint(invPanel,Input.mousePosition))
    //    {
    //        Debug.Log("Drop item");
    //    }
    //}
}
