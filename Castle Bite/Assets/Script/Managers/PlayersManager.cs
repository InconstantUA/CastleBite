using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour {
    [SerializeField]
    Transform playersRootTransform;

    public GamePlayer GetPlayerByType(PlayerType playerType)
    {
        foreach (GamePlayer gamePlayer in playersRootTransform.GetComponentsInChildren<GamePlayer>())
        {
            if (playerType == gamePlayer.PlayerType)
            {
                return gamePlayer;
            }
        }
        Debug.LogWarning("Failed to find player of " + playerType.ToString() + " type");
        return null;
    }
}
