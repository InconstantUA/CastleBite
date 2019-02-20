using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPropagationContext : PropagationContext
{
    InventoryItem sourceItem;

    public ItemPropagationContext(InventoryItem sourceItem, PartyUnit destinationPartyUnit)
    {
        this.sourceItem = sourceItem;
        this.destinationPartyUnit = destinationPartyUnit;
    }

    public InventoryItem SourceItem
    {
        get
        {
            return sourceItem;
        }

        set
        {
            sourceItem = value;
        }
    }
}
