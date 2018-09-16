using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerraType
{
    Ice,
    Ocean,
    Sea,
    Shore,
    Lake,
    River,
    Snow,
    Mounain,
    Hill,
    Forest,
    Jungle,
    Tundra,
    Plain,
    Sand,
    Lava,
    Volcano,
    Road,
    City,
    Valley
}

[Serializable]
public class TerraData : System.Object
{
    public TerraType terraType;
    public bool terraIsPassable;
    public int terraMoveCost;
}

public class Terra : MonoBehaviour {
    [SerializeField]
    TerraData terraData;

    public TerraType TerraType
    {
        get
        {
            return terraData.terraType;
        }

        set
        {
            terraData.terraType = value;
        }
    }

    public bool TerraIsPassable
    {
        get
        {
            return terraData.terraIsPassable;
        }

        set
        {
            terraData.terraIsPassable = value;
        }
    }

    public int TerraMoveCost
    {
        get
        {
            return terraData.terraMoveCost;
        }

        set
        {
            terraData.terraMoveCost = value;
        }
    }
}
