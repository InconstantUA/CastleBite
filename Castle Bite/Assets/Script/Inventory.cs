using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IHasChanged {
    [SerializeField] Transform slots;
    [SerializeField] Text inventoryText;

	// Use this for initialization
	void Start () {
        // call it once to represent current slots state text
        HasChanged();
	}
	
    // This is just an example.
    // It may be helful in future
    public void HasChanged()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.Append(" - ");
        foreach (Transform slotTransform in slots)
        {
            if (slotTransform.childCount > 1)
            {
                GameObject item = slotTransform.GetChild(1).gameObject;
                if (item)
                {
                    builder.Append(item.name);
                    builder.Append(" - ");
                }
            }
        }
        inventoryText.text = builder.ToString();
    }
}

namespace UnityEngine.EventSystems
{
    public interface IHasChanged : IEventSystemHandler
    {
        void HasChanged();
    }
}