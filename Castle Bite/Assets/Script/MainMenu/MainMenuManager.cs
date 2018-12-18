using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

    public static MainMenuManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void SaveOptions()
    {
        Debug.Log("Save Options");
        // Game options
        PlayerPrefs.SetInt("Autosave", GameOptions.options.gameOpt.autosave); // 0 - disable, 1 - enable
        // Video options
        PlayerPrefs.SetInt("FontSize", GameOptions.options.videoOpt.fontSize);
        // Audio options
        PlayerPrefs.SetInt("MusicVolume", GameOptions.options.audioOpt.musicVolume);
        // Keyboard and Mouse options
        PlayerPrefs.SetInt("KeyboardMoveUp", GameOptions.options.keyboardAndMouseOpt.moveUp);
        PlayerPrefs.SetInt("KeyboardMoveDown", GameOptions.options.keyboardAndMouseOpt.moveDown);
    }

    void LoadOptions()
    {
        Debug.Log("Load Options");
        // Game options
        GameOptions.options.gameOpt.autosave = PlayerPrefs.GetInt("Autosave", 0); // default 1 - enable autosave
        if (GameOptions.options.gameOpt.autosave == 0)
        {
            transform.root.Find("MainMenu/OptionsGameSubmenuL3Panel/Autosave/Value").GetComponent<Text>().text = "Off";
        }
        else
        {
            transform.root.Find("MainMenu/OptionsGameSubmenuL3Panel/Autosave/Value").GetComponent<Text>().text = "On";
        }
        // Video options
        GameOptions.options.videoOpt.fontSize = PlayerPrefs.GetInt("FontSize", 14); // default 14
        transform.root.Find("MainMenu/OptionsVideoSubmenuL3Panel/FontSize/Panel/Text").GetComponent<Text>().text = GameOptions.options.videoOpt.fontSize.ToString();
        transform.root.Find("MainMenu/OptionsVideoSubmenuL3Panel/FontSize/Panel/Slider").GetComponent<Slider>().value = GameOptions.options.videoOpt.fontSize;
        // Audio options
        GameOptions.options.audioOpt.musicVolume = PlayerPrefs.GetInt("MusicVolume", 55); // default 55
        transform.root.Find("MainMenu/OptionsAudioSubmenuL3Panel/MusicVolume/Panel/Text").GetComponent<Text>().text = GameOptions.options.audioOpt.musicVolume.ToString();
        transform.root.Find("MainMenu/OptionsAudioSubmenuL3Panel/MusicVolume/Panel/Slider").GetComponent<Slider>().value = GameOptions.options.audioOpt.musicVolume;
        // Keyboard and Mouse options
        GameOptions.options.keyboardAndMouseOpt.moveUp = PlayerPrefs.GetInt("KeyboardMoveUp", 1); // this is not implemented - just use something as default value
        GameOptions.options.keyboardAndMouseOpt.moveDown = PlayerPrefs.GetInt("KeyboardMoveDown", 2); // this is not implemented - just use something as default value
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
        // activate choose your first hero menu
        chooseYourFirstHero.SetActive(true);
        // disable this menu
        gameObject.SetActive(false);
        // ..
        //// activate main menu in game mode
        //MainMenuInGameModeSetActive(true);
        //// instruct chapter manager to start game
        //ChapterManager.Instance.StartGame();
    }

    public void QuitToTheMainMenu()
    {
        // disable map and map menu
        MapManager.Instance.gameObject.SetActive(false);
        MapMenuManager.Instance.gameObject.SetActive(false);
        // deactivate main menu in game mode
        MainMenuInGameModeSetActive(false);
        // instruct application manager to end current game
        ApplicationManager.Instance.EndCurrentGame();
    }
}
