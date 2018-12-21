using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for game's chapter objective completion
// Finishes chapter when:
// - player reaches chapter goal
// - other AI player reaches chapter goal
// - player is eliminated

// We subscribe to specific events on specific objects
// which are critcal for game progress

public class ChapterManager : MonoBehaviour {
    public static ChapterManager Instance { get; private set; }

    [SerializeField]
    Chapter activeChapter;
    [SerializeField]
    Chapter[] chapters;
    //[SerializeField]
    //PlayerData[] playersData;
    CoroutineQueue coroutineQueue;
    // for end (exit) chapter logic
    bool completed = false;
    bool failed = false;
    string failureReason = "";

    #region Properties
    public CoroutineQueue CoroutineQueue
    {
        get
        {
            return coroutineQueue;
        }

        set
        {
            coroutineQueue = value;
        }
    }

    //public PlayerData[] PlayersData
    //{
    //    get
    //    {
    //        return playersData;
    //    }
    //}

    public Chapter ActiveChapter
    {
        get
        {
            return activeChapter;
        }

        set
        {
            activeChapter = value;
        }
    }

    public Chapter[] Chapters
    {
        get
        {
            return chapters;
        }

        set
        {
            chapters = value;
        }
    }
    #endregion Properties

    void Awake()
    {
        Instance = this;
        // Create a coroutine queue that can run max 1 coroutine at once
        coroutineQueue = new CoroutineQueue(1, StartCoroutine);
    }

    //public GameObject GetWorldTemplate()
    //{
    //    foreach(Chapter chapter in chapters)
    //    {
    //        // verify if chaper matches
    //        if (chapter.ChapterData.chapterName == activeChapter.ChapterData.chapterName)
    //        {
    //            return chapter.gameObject;
    //        }
    //    }
    //    Debug.LogError("Cannot find world template for " + chapterData.chapterName + " chapter");
    //    return null;
    //}

    public void OnGoalCityCapture(City city)
    {
        if (city.name == activeChapter.ChapterData.targetCityName)
        {
            GamePlayer player = TurnsManager.Instance.GetActivePlayer();
            // verify if city has been captured by human player
            // .. I assume that game is in single player game mode, where there is only one human player and all others are AI players
            if ((city.CityFaction == player.Faction) && (PlayerType.Human == player.PlayerType))
            {
                activeChapter.ChapterData.goalTargetCityCaptured = true;
                Debug.Log("Target city has been captured by player");
            }
            else
            {
                failed = true;
                failureReason = city.name + " city has been captured by other player.";
                Debug.Log("Target city has been captured by other player.");
            }
        }
    }

    void Update()
    {
        // Verify if player has win game
        if (activeChapter.ChapterData.goalTargetCityCaptured && activeChapter.ChapterData.goalTargetHeroDestroyed)
        {
            Debug.Log("Player has completed this chapter");
            completed = true;
            ExitChapter();
        }
        // Verify if player failed to reach goal
        else if (failed)
        {
            Debug.Log("Player has failed to reach goal");
            ExitChapter();
        }
    }

    void GoToNextChapter()
    {
        Debug.Log("Go to next chapter");
        // I assume that we were at map before game end
        // Get vars
        GameObject transferHeroScreen = UIRoot.Instance.transform.Find("MiscUI/TransferHeroAndItemsToNextChapter").gameObject;
        // Disable map and enable main menu
        MapManager.Instance.gameObject.SetActive(false);
        MapMenuManager.Instance.gameObject.SetActive(false);
        transferHeroScreen.SetActive(true);
    }

    void ShowCredits()
    {
        Debug.Log("Show credits");
        // I assume that we were at map before game end
        // Disable city screen if it was active
        EditPartyScreen cityScreen = UIRoot.Instance.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>();
        if (cityScreen)
        {
            cityScreen.gameObject.SetActive(false);
        }
        // Disable map
        MapManager.Instance.gameObject.SetActive(false);
        MapMenuManager.Instance.gameObject.SetActive(false);
        // Enable credits
        GameObject credits = UIRoot.Instance.transform.Find("MiscUI/Credits").gameObject;
        credits.SetActive(true);
    }

    void EndGame()
    {
        Debug.Log("End game");
        // I assume that we were at map before game end
        // Get vars
        GameObject mainMenu = UIRoot.Instance.transform.Find("MainMenu").gameObject;
        // Disable map and enable main menu
        MapManager.Instance.gameObject.SetActive(false);
        MapMenuManager.Instance.gameObject.SetActive(false);
        mainMenu.SetActive(true);
        // Activate and deactivate required menus in main menu so it looks like during game start
        // As long as we are in game mode now, then Start button is not needed any more
        // instead activate Continue button
        GameObject startButton = mainMenu.transform.Find("MainMenuPanel/Start").gameObject;
        GameObject continueButton = mainMenu.transform.Find("MainMenuPanel/Continue").gameObject;
        startButton.SetActive(true);
        continueButton.SetActive(false);
    }

    void ExitChapter()
    {
        // idea: add option to continue playing this chapter and continue to next chapter or end game when player wish
        // ..
        Debug.Log("Exit chapter");
        NotificationPopUp notificationPopup = UIRoot.Instance.transform.Find("MiscUI").Find("NotificationPopUp").GetComponent<NotificationPopUp>();
        string messageText;
        if (failed)
        {
            notificationPopup.GetComponentInChildren<NotificationPopUpOkButton>().SetOnClickFunc(EndGame);
            messageText = "You failed to complete this chapter. " + failureReason;
        }
        else if (completed)
        {
            if (activeChapter.ChapterData.lastChapter)
            {
                notificationPopup.GetComponentInChildren<NotificationPopUpOkButton>().SetOnClickFunc(ShowCredits);
                messageText = "Congratulations! You have completed this game!";
            }
            else
            {
                notificationPopup.GetComponentInChildren<NotificationPopUpOkButton>().SetOnClickFunc(GoToNextChapter);
                messageText = "Congratulations! You have completed this chapter.";
            }
        }
        else
        {
            Debug.LogError("Unhandled condition");
            messageText = "Unhandled condition";
        }
        notificationPopup.SetMessage(messageText);
        notificationPopup.gameObject.SetActive(true);
        // reset variables to remove constant popups on Update() function call:
        failed = false;
        activeChapter.ChapterData.goalTargetCityCaptured = false;
        activeChapter.ChapterData.goalTargetHeroDestroyed = false;
    }

    IEnumerator SetEndingGameScreen()
    {
        // Activate Loading screen
        UIRoot.Instance.GetComponentInChildren<EndingGameScreen>(true).SetActive(true);
        // Set waiting cursor
        CursorController.Instance.SetBlockInputCursor();
        // Activate input blocker
        InputBlocker.Instance.SetActive(true);
        // Replace map with the clear Map
        Debug.LogWarning("Replace map with new map from template");
        // Deactivate loading screen after clean
        yield return new WaitForSeconds(2);
        // Deactivate Loading screen
        UIRoot.Instance.GetComponentInChildren<EndingGameScreen>(true).SetActive(false);
        // Set normal cursor
        CursorController.Instance.SetNormalCursor();
        // Disable input blocker
        InputBlocker.Instance.SetActive(false);
    }

    public void EndCurrentGame()
    {
        coroutineQueue.Run(SetEndingGameScreen());
    }
}
