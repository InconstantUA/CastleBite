using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

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
    }

    void OnEnable()
    {
        LoadOptions();
    }

    void OnDisable()
    {
        SaveOptions();
    }
}
