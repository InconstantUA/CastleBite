using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUnitPropagationContext : PropagationContext
{
    //InventoryItem sourceItem;
    PartyUnit sourcePartyUnit;

    //public PartyUnitPropagationContext(InventoryItem sourceItem, PartyUnit destinationPartyUnit)
    //{
    //    this.sourceItem = sourceItem;
    //    this.destinationPartyUnit = destinationPartyUnit;
    //}

    public PartyUnitPropagationContext(PartyUnit sourcePartyUnit, PartyUnit destinationPartyUnit)
    {
        this.sourcePartyUnit = sourcePartyUnit;
        this.destinationPartyUnit = destinationPartyUnit;
    }

    //public InventoryItem SourceItem
    //{
    //    get
    //    {
    //        return sourceItem;
    //    }

    //    set
    //    {
    //        sourceItem = value;
    //    }
    //}

    public PartyUnit SourcePartyUnit
    {
        get
        {
            return sourcePartyUnit;
        }

        set
        {
            sourcePartyUnit = value;
        }
    }
}
