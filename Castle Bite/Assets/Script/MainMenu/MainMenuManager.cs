using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class MainMenuManager : MonoBehaviour {

    [SerializeField]
    GameObject mStart;
    [SerializeField]
    GameObject mContinue;
    [SerializeField]
    GameObject mSave;
    [SerializeField]
    GameObject mLoad;
    [SerializeField]
    GameObject mOptions;
    [SerializeField]
    GameObject mQuit;
    [SerializeField]
    GameObject mQuitToTheMainMenu;
    [SerializeField]
    ChooseYourFirstHero chooseYourFirstHero;
    [SerializeField]
    ChooseChapter chooseChapter;
    [SerializeField]
    GameObject mainMenuPanel;
    [SerializeField]
    GameObject currentGold;


    public static MainMenuManager Instance { get; private set; }

    public GameObject MainMenuPanel
    {
        get
        {
            return mainMenuPanel;
        }
    }

    public ChooseYourFirstHero ChooseYourFirstHero
    {
        get
        {
            return chooseYourFirstHero;
        }
    }

    public ChooseChapter ChooseChapter
    {
        get
        {
            return chooseChapter;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void SaveOptions()
    {
        Debug.Log("Save Options");
        // Game options
        PlayerPrefs.SetInt("Autosave", GameOptions.Instance.gameOpt.autosave); // 0 - disable, 1 - enable
        // Video options
        PlayerPrefs.SetInt("FontSize", GameOptions.Instance.videoOpt.fontSize);
        // Audio options
        PlayerPrefs.SetInt("MusicVolume", GameOptions.Instance.audioOpt.musicVolume);
        // Keyboard and Mouse options
        PlayerPrefs.SetInt("KeyboardMoveUp", GameOptions.Instance.keyboardAndMouseOpt.moveUp);
        PlayerPrefs.SetInt("KeyboardMoveDown", GameOptions.Instance.keyboardAndMouseOpt.moveDown);
    }

    void LoadOptions()
    {
        Debug.Log("Load Options");
        // Game options
        GameOptions.Instance.gameOpt.autosave = PlayerPrefs.GetInt("Autosave", 0); // default 1 - enable autosave
        if (GameOptions.Instance.gameOpt.autosave == 0)
        {
            transform.root.Find("MainMenu/OptionsGameSubmenuL3Panel/Autosave/Value").GetComponent<Text>().text = "Off";
        }
        else
        {
            transform.root.Find("MainMenu/OptionsGameSubmenuL3Panel/Autosave/Value").GetComponent<Text>().text = "On";
        }
        // Video options
        GameOptions.Instance.videoOpt.fontSize = PlayerPrefs.GetInt("FontSize", 14); // default 14
        transform.root.Find("MainMenu/OptionsVideoSubmenuL3Panel/FontSize/Panel/Text").GetComponent<Text>().text = GameOptions.Instance.videoOpt.fontSize.ToString();
        transform.root.Find("MainMenu/OptionsVideoSubmenuL3Panel/FontSize/Panel/Slider").GetComponent<Slider>().value = GameOptions.Instance.videoOpt.fontSize;
        // Audio options
        GameOptions.Instance.audioOpt.musicVolume = PlayerPrefs.GetInt("MusicVolume", 55); // default 55
        transform.root.Find("MainMenu/OptionsAudioSubmenuL3Panel/MusicVolume/Panel/Text").GetComponent<Text>().text = GameOptions.Instance.audioOpt.musicVolume.ToString();
        transform.root.Find("MainMenu/OptionsAudioSubmenuL3Panel/MusicVolume/Panel/Slider").GetComponent<Slider>().value = GameOptions.Instance.audioOpt.musicVolume;
        // Keyboard and Mouse options
        GameOptions.Instance.keyboardAndMouseOpt.moveUp = PlayerPrefs.GetInt("KeyboardMoveUp", 1); // this is not implemented - just use something as default value
        GameOptions.Instance.keyboardAndMouseOpt.moveDown = PlayerPrefs.GetInt("KeyboardMoveDown", 2); // this is not implemented - just use something as default value
        //// Verify if there are saves available
        //string fileExtension = ".save";
        //FileInfo[] files = new DirectoryInfo(Application.persistentDataPath).GetFiles("*" + fileExtension);
        //if (files.Length >= 1)
        //{
        //    // activate Load game menu
        //    transform.Find("MainMenuPanel/Load").gameObject.SetActive(true);
        //}
        //else
        //{
        //    // deactivate Load game menu (this might be needed if all saves were removed)
        //    transform.Find("MainMenuPanel/Load").gameObject.SetActive(false);
        //}
    }

    void OnEnable()
    {
        LoadOptions();
    }

    void OnDisable()
    {
        SaveOptions();
    }

    public void MainMenuInGameModeSetActive(bool doActivate)
    {
        // disable Start new game menu button
        mStart.gameObject.SetActive(!doActivate);
        // disable Quit menu button
        mQuit.gameObject.SetActive(!doActivate);
        // enable Continue menu button
        mContinue.gameObject.SetActive(doActivate);
        // enable Save menu button
        mSave.gameObject.SetActive(doActivate);
        // enable Quit to the main menu button
        mQuitToTheMainMenu.gameObject.SetActive(doActivate);
    }

    public void StartNewGame()
    {
        // Activate choose chapter menu
        chooseChapter.gameObject.SetActive(true);
        //// create world from template and replace active chapter link
        //ChapterManager.Instance.ActiveChapter = Instantiate(ChapterManager.Instance.ActiveChapter.gameObject, World.Instance.transform).GetComponent<Chapter>();
        //// activate choose your first hero menu
        //chooseYourFirstHero.SetActive(true);
        // disable main menu panels
        mainMenuPanel.SetActive(false);
        // ..
        //// activate main menu in game mode
        //MainMenuInGameModeSetActive(true);
        //// instruct chapter manager to start game
        //ChapterManager.Instance.StartGame();
    }

    void OnQuitToTheMainMenuYesConfirmation()
    {
        Debug.Log("Yes");
        // end current game
        QuitToTheMainMenu();
    }

    void OnQuitToTheMainMenuNoConfirmation()
    {
        Debug.Log("No");
        // nothing to do here
    }

    void QuitToTheMainMenu()
    {
        // disable map and map menu
        MapManager.Instance.gameObject.SetActive(false);
        MapMenuManager.Instance.gameObject.SetActive(false);
        // deactivate main menu in game mode
        MainMenuInGameModeSetActive(false);
        // instruct application manager to end current game
        ChapterManager.Instance.EndCurrentGame();
    }

    public void AskQuitToTheMainMenuConfirmation()
    {
        // ask for confirmation
        // Ask user whether he wants to delete save
        ConfirmationPopUp confirmationPopUp = ConfirmationPopUp.Instance();
        // set actions
        UnityAction YesAction = new UnityAction(OnQuitToTheMainMenuYesConfirmation);
        UnityAction NoAction = new UnityAction(OnQuitToTheMainMenuNoConfirmation);
        // set message
        string confirmationMessage = "Do you want to terminate current game? Not saved progress will be lost.";
        // send actions to Confirmation popup, so he knows how to react on no and yes btn presses
        confirmationPopUp.Choice(confirmationMessage, YesAction, NoAction);
    }

    public void Continue()
    {
        // return map manager to the browse mode
        MapManager.Instance.SetMode(MapManager.Mode.Browse);
        // verify if current gold info should be visible
        if (GameOptions.Instance.mapUIOpt.togglePlayerIncome != 0)
        {
            // enable current gold info
            currentGold.SetActive(true);
        }
        //// enable map and map menu
        //MapManager.Instance.gameObject.SetActive(true);
        //MapMenuManager.Instance.gameObject.SetActive(true);
        // disable main menu
        gameObject.SetActive(false);
    }
}
