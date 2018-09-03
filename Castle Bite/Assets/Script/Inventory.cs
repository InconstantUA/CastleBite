using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public enum HeroEquipmentSlot
{
    Shard,
    Head,
    Neck,
    RightHand,
    LeftHand,
    Chest,
    BeltSlot1,
    BeltSlot2,
    Boots
}

[Serializable]
public class InventoryItem : System.Object
{
    public string itemName;
    public int itemValue;
    public HeroEquipmentSlot[] compatibleEquipmentSlots;
    public bool isConsumableItem;
    public UniquePowerModifier[] uniquePowerModifiers;
}

[Serializable]
public class InventoryData : System.Object
{
    public InventoryItem[] inventoryItems;
}

public class Inventory : MonoBehaviour
{

}

//public class Inventory : MonoBehaviour, IHasChanged {
 //   [SerializeField] Transform slots;
 //   [SerializeField] Text inventoryText;

	//// Use this for initialization
	//void Start () {
 //       // call it once to represent current slots state text
 //       HasChanged();
	//}
	
 //   // This is just an example.
 //   // It may be helful in future
 //   public void HasChanged()
 //   {
 //       System.Text.StringBuilder builder = new System.Text.StringBuilder();
 //       builder.Append(" - ");
 //       foreach (Transform slotTransform in slots)
 //       {
 //           if (slotTransform.childCount > 1)
 //           {
 //               GameObject item = slotTransform.GetChild(1).gameObject;
 //               if (item)
 //               {
 //                   builder.Append(item.name);
 //                   builder.Append(" - ");
 //               }
 //           }
 //       }
 //       inventoryText.text = builder.ToString();
 //   }
//}

//namespace UnityEngine.EventSystems
//{
//    public interface IHasChanged : IEventSystemHandler
//    {
//        void HasChanged();
//    }
//}