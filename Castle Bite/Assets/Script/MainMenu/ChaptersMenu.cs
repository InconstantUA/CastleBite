using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaptersMenu : MonoBehaviour
{
    GameObject selectedChapter;

    void OnDisable()
    {
        DeselectChapter();
    }

    public void SetChapterDetails(GameObject chapterSelector)
    {
        // init chapter data
        ChapterData chapterData;
        // get chapter data
        chapterData = chapterSelector.GetComponent<ChapterUISelector>().LChapter.ChapterData;
        // update UI
        // set chapter details  transform
        Transform chapterDetails = transform.parent.Find("ChapterDetails");
        // activate info
        chapterDetails.Find("Info").gameObject.SetActive(true);
        // set turn nuber
        chapterDetails.Find("Info/ChapterName/Value").GetComponent<Text>().text = chapterData.chapterDisplayName;
        // set players information
        // get players info root UI
        Transform playersInfoRoot = chapterDetails.Find("Info/Players/List");
        // remove old information
        foreach (Transform child in playersInfoRoot)
        {
            Destroy(child.gameObject);
        }
        // get player info template
        GameObject playerInfoTemplateUI = transform.root.Find("Templates/UI/Menu/PlayerInfoTemplate").gameObject;
        foreach (GamePlayer gamePlayer in chapterSelector.GetComponent<ChapterUISelector>().LChapter.GetComponentInChildren<ObjectsManager>(true).GetGamePlayers())
        {
            // clone template
            GameObject newPlayerInfo = Instantiate(playerInfoTemplateUI, playersInfoRoot);
            // activate it
            newPlayerInfo.SetActive(true);
            // set player name
            newPlayerInfo.transform.Find("Name").GetComponent<Text>().text = gamePlayer.PlayerData.givenName;
            newPlayerInfo.transform.Find("Faction").GetComponent<Text>().text = gamePlayer.PlayerData.faction.ToString();
        }
        // set Description
        chapterDetails.Find("Info/Description/Value").GetComponent<Text>().text = chapterData.description;
    }

    void SelectChapter(GameObject chapter)
    {
        // chapter selected chapter
        selectedChapter = chapter;
        // update chapter details
        SetChapterDetails(chapter);
    }

    public void DeselectChapter()
    {
        // clean up selectedChapter
        selectedChapter = null;
        // clean up chapter details by deactivating info UI
        transform.parent.Find("ChapterDetails/Info").gameObject.SetActive(false);
    }

    public void SetSelectedChapter(GameObject value)
    {
        // verify if we already have any chapter selected
        if (selectedChapter != null)
        {
            // deleselect prevoius chapter
            DeselectChapter();
        }
        // select new chapter
        SelectChapter(value);
    }
}
