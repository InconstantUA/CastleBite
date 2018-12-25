using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Achievement
{
    Capturer,
    Conqueror,
    Tyrant
}

[Serializable]
public class AchievementData: System.Object
{
    public Achievement achievement;
    public long timeTicksWhenAchieved;
    public string additionalDetails;       // [Optional] Additional details, for example: which city has been captured
}

[Serializable]
public class AchievementTriggerCityCapture
{
    public Achievement achievement;
    public int citiesCaptured;
    public bool isCapital;
}

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager Instance { get; private set; }

    [SerializeField]
    AchievementConfig[] achievementConfigs;
    [SerializeField]
    AchievementTriggerCityCapture[] achievementTriggerCityCapture;

    void Awake()
    {
        Instance = this;
    }

    public AchievementConfig[] AchievementConfigs
    {
        get
        {
            return achievementConfigs;
        }

        set
        {
            achievementConfigs = value;
        }
    }

}
