﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData : System.Object
{
    public string givenName;
    public int totalGold;
    public Faction faction;
}

public class PlayerObj : MonoBehaviour {
    [SerializeField]
    PlayerData playerData;
    //[SerializeField]
    //int totalGold;
    //[SerializeField]
    //Faction faction;

    public Faction Faction
    {
        get
        {
            return playerData.faction;
        }

        set
        {
            playerData.faction = value;
        }
    }

    public string GivenName
    {
        get
        {
            return playerData.givenName;
        }

        set
        {
            playerData.givenName = value;
        }
    }

    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }

        set
        {
            playerData = value;
        }
    }

    public int PlayerGold
    {
        get
        {
            return playerData.totalGold;
        }

        set
        {
            playerData.totalGold = value;
        }
    }

    //public int GetTotalGold()
    //{
    //    return playerData.totalGold;
    //}

    //public void SetTotalGold(int newTotalGoldValue)
    //{
    //    playerData.totalGold = newTotalGoldValue;
    //    // Trigger gold update event to update gold value in UI
    //}
}
