using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext : Singleton<GameContext>
{
    public enum GameContextID
    {
        None,
        Map,
        EditPartyScreen,
        EquipmentScreen,
        Battle
    }

    static GameContextID activeGameContextID;

    public void OnBattleScreenHasBeenActivated()
    {
        activeGameContextID = GameContextID.Battle;
    }

    public void OnEditPartyScreenHasBeenActivated()
    {
        activeGameContextID = GameContextID.EditPartyScreen;
    }

    public void OnMapScreenHasBeenActivated()
    {
        activeGameContextID = GameContextID.Map;
    }

    public void OnBattleScreenHasBeenDeactivated()
    {
        activeGameContextID = GameContextID.None;
    }

    public void OnEditPartyScreenHasBeenDeactivated()
    {
        activeGameContextID = GameContextID.None;
    }

    public void OnMapScreenHasBeenDeactivated()
    {
        activeGameContextID = GameContextID.None;
    }

    public void OnEquipmentScreenHasBeenActivated()
    {
        activeGameContextID = GameContextID.EquipmentScreen;
    }

    public void OnEquipmentScreenHasBeenDeactivated()
    {
        // return context to EditPartyScreen
        activeGameContextID = GameContextID.EditPartyScreen;
    }

    public static System.Object Context
    {
        get
        {
            switch (activeGameContextID)
            {
                case GameContextID.None:
                    Debug.LogWarning("Return Null context");
                    return null;
                case GameContextID.Map:
                    Debug.Log("Return Map context");
                    return MapContext.Instance;
                case GameContextID.Battle:
                    Debug.Log("Return Battle context");
                    return BattleContext.Instance;
                case GameContextID.EditPartyScreen:
                    Debug.Log("Return EditPartyScreen context");
                    return EditPartyScreenContext.Instance;
                case GameContextID.EquipmentScreen:
                    Debug.Log("Return EquipmentScreen context");
                    return EquipmentScreenContext.Instance;
                default:
                    Debug.LogError("Unknown game context");
                    return null;
            }
        }
    }

    public static InventoryItem GetItemBeingUsed()
    {
        switch (activeGameContextID)
        {
            case GameContextID.None:
                Debug.LogWarning("Return Null");
                return null;
            case GameContextID.Map:
                Debug.Log("Return ItemBeingUsed from Map context");
                return MapContext.ItemBeingUsed;
            case GameContextID.Battle:
                Debug.Log("Return ItemBeingUsed from Battle context");
                return BattleContext.ItemBeingUsed;
            case GameContextID.EditPartyScreen:
                Debug.Log("Return ItemBeingUsed from EditPartyScreen context");
                return EditPartyScreenContext.ItemBeingUsed;
            case GameContextID.EquipmentScreen:
                Debug.Log("Return ItemBeingUsed from EquipmentScreen context");
                return EquipmentScreenContext.ItemBeingUsed;
            default:
                Debug.LogError("Unknown game context");
                return null;
        }
    }

    public static void SetDestinationUnitSlot(UnitSlot destinationUnitSlot)
    {
        switch (activeGameContextID)
        {
            case GameContextID.None:
                Debug.LogWarning("None: Return");
                return;
            case GameContextID.Map:
                Debug.LogWarning("Map: Return");
                return;
            case GameContextID.Battle:
                Debug.Log("Set DestinationUnitSlot for Battle context");
                BattleContext.DestinationUnitSlot = destinationUnitSlot;
                return;
            case GameContextID.EditPartyScreen:
                Debug.Log("Set DestinationUnitSlot for EditPartyScreen context");
                EditPartyScreenContext.DestinationUnitSlot = destinationUnitSlot;
                return;
            case GameContextID.EquipmentScreen:
                Debug.LogWarning("EquipmentScreen: Return");
                return;
            default:
                Debug.LogError("Unknown game context");
                return;
        }
    }

    public static void SetUniquePowerModifierID(UniquePowerModifierID uniquePowerModifierID)
    {
        switch (activeGameContextID)
        {
            case GameContextID.None:
                Debug.LogWarning("None: Return");
                return;
            case GameContextID.Map:
                Debug.LogWarning("Map: Return");
                return;
            case GameContextID.Battle:
                Debug.Log("Set UniquePowerModifierID for Battle context");
                BattleContext.UniquePowerModifierID = uniquePowerModifierID;
                return;
            case GameContextID.EditPartyScreen:
                Debug.Log("Set UniquePowerModifierID for EditPartyScreen context");
                EditPartyScreenContext.UniquePowerModifierID = uniquePowerModifierID;
                return;
            case GameContextID.EquipmentScreen:
                Debug.LogWarning("EquipmentScreen: Return");
                return;
            default:
                Debug.LogError("Unknown game context");
                return;
        }
    }

    public static void SetActivatedUPMConfigIndex(int activatedUPMConfigIndex)
    {
        switch (activeGameContextID)
        {
            case GameContextID.None:
                Debug.LogWarning("None: Return");
                return;
            case GameContextID.Map:
                Debug.LogWarning("Map: Return");
                return;
            case GameContextID.Battle:
                Debug.Log("Set ActivatedUPMConfigIndex for Battle context");
                BattleContext.ActivatedUPMConfigIndex = activatedUPMConfigIndex;
                return;
            case GameContextID.EditPartyScreen:
                Debug.Log("Set ActivatedUPMConfigIndex for EditPartyScreen context");
                EditPartyScreenContext.ActivatedUPMConfigIndex = activatedUPMConfigIndex;
                return;
            case GameContextID.EquipmentScreen:
                Debug.LogWarning("EquipmentScreen: Return");
                return;
            default:
                Debug.LogError("Unknown game context");
                return;
        }
    }
}