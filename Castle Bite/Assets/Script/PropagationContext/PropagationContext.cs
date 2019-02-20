using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagationContext
{
    protected PartyUnit destinationPartyUnit;

    public PartyUnit DestinationPartyUnit
    {
        get
        {
            return destinationPartyUnit;
        }

        set
        {
            destinationPartyUnit = value;
        }
    }
}
