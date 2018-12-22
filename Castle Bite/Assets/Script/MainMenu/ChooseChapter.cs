using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseChapter : MonoBehaviour
{
    [SerializeField]
    int numberOfChaptersToLoadAtOnce = 10;
    [SerializeField]
    GameObject continueButton;
    [SerializeField]
    GameObject backButton;


    IEnumerator SelectFirstChapter()
    {
        Debug.Log("Select first chapter");
        yield return new WaitForSeconds(0.01f);
        foreach (TextToggle toggle in transform.Find("Chapters/ChaptersList/Grid").GetComponentsInChildren<TextToggle>())
        {
            Debug.Log("First chapter is " + toggle.name);
            // select first chapter and exit loop
            toggle.ActOnLeftMouseClick();
            break;
        }
    }

    IEnumerator SetListOfChapters()
    {
        // update list of chapters
        // get list of all chapters
        Chapter[] chapters = ChapterManager.Instance.Chapters;
        // verify if all object are loaded
        if (transform.Find("Chapters"))
            if (transform.Find("Chapters/ChaptersList"))
                if (transform.Find("Chapters/ChaptersList/ChapterTemplate"))
                {
                    // Get chapter UI template
                    GameObject chapterUITemplate = transform.Find("Chapters/ChaptersList/ChapterTemplate").gameObject;
                    // Get parent for new chapters UI
                    Transform chaptersParentTr = transform.Find("Chapters/ChaptersList/Grid");
                    // create entry in UI for each chapter
                    GameObject newChapter;
                    for (int i = 0; i < chapters.Length; i++)
                    //foreach (FileInfo file in files)
                    {
                        // create chapter UI
                        newChapter = Instantiate(chapterUITemplate, chaptersParentTr);
                        // link chapter to the selector
                        newChapter.GetComponent<ChapterUISelector>().LChapter = chapters[i];
                        // rename game object 
                        newChapter.name = newChapter.GetComponent<ChapterUISelector>().LChapter.ChapterData.chapterName.ToString();
                        // enable chapter
                        newChapter.gameObject.SetActive(true);
                        // verify if it is time to wait
                        if (i % numberOfChaptersToLoadAtOnce == 0)
                        {
                            // skip to next frame
                            yield return null;
                            //yield return new WaitForSeconds(2);
                        }
                    }
                    // preselect first chapter in the list on the next frame
                    StartCoroutine(SelectFirstChapter());
                }
        yield return null;
    }

    void SetButtonsActive(bool doActivate)
    {
        continueButton.SetActive(doActivate);
        backButton.SetActive(doActivate);
    }

    void OnEnable()
    {
        // update list of chapters on a next frame
        StartCoroutine(SetListOfChapters());
        // enable buttons
        SetButtonsActive(true);
    }

    void OnDisable()
    {
        // check if objects are not destroyed yet
        if (transform.Find("Chapters"))
            if (transform.Find("Chapters/ChaptersList"))
                if (transform.Find("Chapters/ChaptersList/Grid"))
                {
                    // Clean up current list of chapters
                    foreach (ChapterUISelector chapter in transform.Find("Chapters/ChaptersList/Grid").GetComponentsInChildren<ChapterUISelector>())
                    {
                        Destroy(chapter.gameObject);
                    }
                }
        // disable buttons
        SetButtonsActive(false);
    }

    public void Continue()
    {
        // activate choose your first hero menu
        MainMenuManager.Instance.ChooseYourFirstHero.SetActive(true);
        // disable main menu
        MainMenuManager.Instance.gameObject.SetActive(false);
    }

    public void Back()
    {
        // enable main menu panel
        MainMenuManager.Instance.MainMenuPanel.SetActive(true);
        // disable this menu
        gameObject.SetActive(false);
    }
}
