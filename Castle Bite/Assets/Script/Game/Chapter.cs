using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChapterName
{
    Tutorial,
    ShadowOfDeath
}

[Serializable]
public class ChapterData : System.Object
{
    public ChapterName chapterName;
    public string chapterDisplayName;
    public bool lastChapter;
    // define chapter goals
    public string targetCityName; // we do not use direct link to the city, because it may be destroyed and recreated during save/load process
    public bool goalTargetCityCaptured;
    public string targetHeroName; // we do not use direct link to the hero, because it may be destroyed and recreated during save/load process
    public bool goalTargetHeroDestroyed;
    public string description;
    public string prologHeader;
    public string[] prologBrief;
    public string prologObjective;
}

public class Chapter : MonoBehaviour
{
    [SerializeField]
    ChapterData chapterData;

    internal ChapterData ChapterData
    {
        get
        {
            return chapterData;
        }

        set
        {
            chapterData = value;
        }
    }

    //public ChapterName ChapterName
    //{
    //    get
    //    {
    //        return chapterData.chapterName;
    //    }

    //    set
    //    {
    //        chapterData.chapterName = value;
    //    }
    //}

    //public bool LastChapter
    //{
    //    get
    //    {
    //        return chapterData.lastChapter;
    //    }

    //    set
    //    {
    //        chapterData.lastChapter = value;
    //    }
    //}

    //public string TargetCityName
    //{
    //    get
    //    {
    //        return chapterData.targetCityName;
    //    }

    //    set
    //    {
    //        chapterData.targetCityName = value;
    //    }
    //}

    //public bool GoalTargetCityCaptured
    //{
    //    get
    //    {
    //        return chapterData.goalTargetCityCaptured;
    //    }

    //    set
    //    {
    //        chapterData.goalTargetCityCaptured = value;
    //    }
    //}

    //public bool GoalTargetHeroDestroyed
    //{
    //    get
    //    {
    //        return chapterData.goalTargetHeroDestroyed;
    //    }

    //    set
    //    {
    //        chapterData.goalTargetHeroDestroyed = value;
    //    }
    //}

}
