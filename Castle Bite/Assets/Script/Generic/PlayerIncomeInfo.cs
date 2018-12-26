using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIncomeInfo : MonoBehaviour
{
    [SerializeField]
    Text playerGoldText;
    [SerializeField]
    Text playerManaText;

    public void UpdateInfo()
    {
        // get Active player
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        playerGoldText.text = activePlayer.TotalGold.ToString();
        playerManaText.text = activePlayer.TotalMana.ToString();
    }

    public void UpdateGoldInfo()
    {
        // get Active player
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        playerGoldText.text = activePlayer.TotalGold.ToString();
    }

    public void UpdateManaInfo()
    {
        // get Active player
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        playerManaText.text = activePlayer.TotalMana.ToString();
    }

    public void SetActive(bool doActivate)
    {
        // verify if we need to activate it
        if (doActivate)
        {
            // Update info
            UpdateInfo();
        }
        // Activate/Deactivate this panel
        gameObject.SetActive(doActivate);
    }
}
