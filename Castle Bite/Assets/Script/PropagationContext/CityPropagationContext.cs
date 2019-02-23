using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPropagationContext : PropagationContext
{
    City sourceCity;

    public CityPropagationContext(City sourceCity, PartyUnit destinationPartyUnit)
    {
        this.sourceCity = sourceCity;
        this.destinationPartyUnit = destinationPartyUnit;
    }

    public City SourceCity
    {
        get
        {
            return sourceCity;
        }

        set
        {
            sourceCity = value;
        }
    }
}
