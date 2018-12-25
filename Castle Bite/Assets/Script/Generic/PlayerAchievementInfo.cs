using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAchievementInfo : MonoBehaviour
{
    [SerializeField]
    Text achievementNameText;
    [SerializeField]
    Text achievementDateText;
    [SerializeField]
    Text achievementDescriptionText;

    public void SetActive(AchievementData achievementData)
    {
        // get achievement config
        Debug.Log("Get config for " + achievementData.achievement + " achievement");
        AchievementConfig achievementConfig = Array.Find(AchievementsManager.Instance.AchievementConfigs, element => element.achievement == achievementData.achievement);
        // set name
        achievementNameText.text = achievementConfig.displayName;
        // convert time to timespan
        DateTime dateTime = new DateTime(achievementData.timeTicksWhenAchieved);
        // set date
        achievementDateText.text = dateTime.ToShortDateString();
        // verify if achivement additional info is not empty
        if ((achievementData.additionalDetails != null) && (achievementData.additionalDetails != ""))
        {
            // set description from data
            achievementDescriptionText.text = achievementData.additionalDetails;
        } else
        {
            // set description from config
            achievementDescriptionText.text = achievementConfig.description;
        }
        // activate it
        gameObject.SetActive(true);
    }
}
