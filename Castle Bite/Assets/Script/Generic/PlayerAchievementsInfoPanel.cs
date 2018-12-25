using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAchievementsInfoPanel : MonoBehaviour
{
    [SerializeField]
    GameObject playerAchievementInfoTemplate;
    [SerializeField]
    Transform playerAchievementsListTransform;

    public void SetActive(bool doActivate)
    {
        if (doActivate)
        {
            // get active player
            GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
            // loop through all player achievemnts
            foreach (AchievementData achievementData in activePlayer.Achievements)
            {
                // create and activate leader info
                Instantiate(playerAchievementInfoTemplate, playerAchievementsListTransform).GetComponent<PlayerAchievementInfo>().SetActive(achievementData);
            }
        }
        else
        {
            // remove all achievement infos
            foreach (PlayerAchievementInfo playerAchievementInfo in GetComponentsInChildren<PlayerAchievementInfo>())
            {
                Destroy(playerAchievementInfo.gameObject);
            }
        }
    }
}
