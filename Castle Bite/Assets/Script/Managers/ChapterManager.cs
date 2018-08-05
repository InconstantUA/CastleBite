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
    //public static ChapterManager Instance { get; private set; }

    [SerializeField]
    bool lastChapter;
    // define chapter goals
    bool goalTargetCityCaptured = false;
    bool goalTargetHeroDestroyed = true;
    // for end (exit) chapter logic
    bool completed = false;
    bool failed = false;
    string failureReason = "";

    void Awake () {
        //Instance = this;
    }

    public void OnGoalCityCapture(City city)
    {
        GamePlayer player = TurnsManager.Instance.GetActivePlayer();
        // verify if city has been captured by human player
        // .. I assume that game is in single player game mode, where there is only one human player and all others are AI players
        if ( (city.Faction == player.Faction) && (PlayerType.Human == player.PlayerType) )
        {
            goalTargetCityCaptured = true;
            Debug.Log("Target city has been captured by player");
        }
        else
        {
            failed = true;
            failureReason = city.name + " city has been captured by other player.";
            Debug.Log("Target city has been captured by other player.");
        }
    }

    void Update()
    {
        // Verify if player has win game
        if (goalTargetCityCaptured && goalTargetHeroDestroyed)
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
        GameObject map = transform.root.Find("MapScreen").gameObject;
        GameObject transferHeroScreen = transform.root.Find("MiscUI/TransferHeroAndItemsToNextChapter").gameObject;
        // Disable map and enable main menu
        map.SetActive(false);
        transferHeroScreen.SetActive(true);
    }

    void ShowCredits()
    {
        Debug.Log("Show credits");
        // I assume that we were at map before game end
        // Get vars
        GameObject map = transform.root.Find("MapScreen").gameObject;
        GameObject credits = transform.root.Find("MiscUI/Credits").gameObject;
        // Disable map and enable main menu
        map.SetActive(false);
        credits.SetActive(true);
    }

    void EndGame()
    {
        Debug.Log("End game");
        // I assume that we were at map before game end
        // Get vars
        GameObject map = transform.root.Find("MapScreen").gameObject;
        GameObject mainMenu = transform.root.Find("MainMenu").gameObject;
        // Disable map and enable main menu
        map.SetActive(false);
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
        NotificationPopUp notificationPopup = transform.root.Find("MiscUI").Find("NotificationPopUp").GetComponent<NotificationPopUp>();
        string messageText;
        if (failed)
        {
            notificationPopup.GetComponentInChildren<NotificationPopUpOkButton>().SetOnClickFunc(EndGame);
            messageText = "You failed to complete this chapter. " + failureReason;
        }
        else if (completed)
        {
            if (lastChapter)
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
        goalTargetCityCaptured = false;
        goalTargetHeroDestroyed = false;
    }
}
