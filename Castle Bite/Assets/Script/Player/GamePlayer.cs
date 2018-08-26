using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum PlayerType
{
    Human,
    AI
}

[Serializable]
public enum PlayerUniqueAbility {
    Offence,
    Defense,
    Sorcery,
    Hardcore
};

[Serializable]
public class PlayerData : System.Object
{
    public PlayerUniqueAbility playerUniqueAbility;
    public PlayerType playerType;
    public string givenName;
    public int totalGold;
    public Faction faction;
}

[Serializable]
public class Gold : System.Object
{

}

public class GamePlayer : MonoBehaviour {
    [SerializeField]
    PlayerData playerData;
    //[SerializeField]
    //int totalGold;
    //[SerializeField]
    //Faction faction;
    public PlayerUniqueAbility PlayerUniqueAbility
    {
        get
        {
            return playerData.playerUniqueAbility;
        }

        set
        {
            playerData.playerUniqueAbility = value;
        }
    }

    public PlayerType PlayerType
    {
        get
        {
            return playerData.playerType;
        }

        set
        {
            playerData.playerType = value;
        }
    }

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
            // trigger event

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
