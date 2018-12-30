using System;
using System.Collections;
using UnityEngine;

public class GameOptions : MonoBehaviour {

    public static GameOptions Instance;

    [Serializable]
    public struct GameOpt
    {
        private int doAutoSave;    // 0 - disable, 1 - enable
        private int lastAutoSavesToKeep;

        public int DoAutoSave
        {
            get
            {
                return doAutoSave;
            }

            set
            {
                doAutoSave = value;
                // save options to PlayerPrefs
                PlayerPrefs.SetInt("DoAutoSave", value); // 0 - disable, 1 - enable
                //Debug.Log("Save doAutoSave [" + value.ToString() + "] value");
            }
        }

        public int LastAutoSavesToKeep
        {
            get
            {
                return lastAutoSavesToKeep;
            }

            set
            {
                lastAutoSavesToKeep = value;
                // save options to PlayerPrefs
                PlayerPrefs.SetInt("LastAutoSavesToKeep", GameOptions.Instance.gameOpt.LastAutoSavesToKeep);
            }
        }
    }
    public GameOpt gameOpt;

    public struct VideoOpt
    {
        public int fontSize;
    }
    public VideoOpt videoOpt;

    public struct AudioOpt
    {
        public int musicVolume;
    }
    public AudioOpt audioOpt;

    public struct KeyboardAndMouseOpt
    {
        public int moveUp;
        public int moveDown;
    }
    public KeyboardAndMouseOpt keyboardAndMouseOpt;

    [Serializable]
    public struct MapUIOpt
    {
        public int toggleCitiesNames;
        public int toggleHeroesNames;
        public int togglePlayerIncome;
        public int toggleManaSources;
        public int toggleTreasureChests;
    }
    public MapUIOpt mapUIOpt;

	void Awake() {
        // verify if game options already initialized
		if (Instance == null)
        {
            // prevent this game object (game options) to be destroyed on scenes change
            // this only works on root objects in a scene, so disabled for now
            // DontDestroyOnLoad(gameObject);
            // initialize gameOptions with this game object
            Instance = this;
            Debug.Log("Initialize game options");
            // load game options
            LoadAutoSaveOptions();
        }
        // verify if game options were instantiated by some other scene, when there is already gameOptions present
        else if (Instance != this)
        {
            // destroy this instance of game options to keep only one instance
            Destroy(gameObject);
            Debug.Log("Destroy current game options");
        }
    }

    void LoadAutoSaveOptions()
    {
        // load options from PlayerPrefs (Note: addressing ConfigManager via GetComponent<ConfigManager>(), because it may be not instantiated yet, if referenced via static Instance)
        gameOpt.DoAutoSave = PlayerPrefs.GetInt("DoAutoSave", GetComponent<ConfigManager>().GameSaveConfig.doAutoSave); // for default values load options from Config
        gameOpt.LastAutoSavesToKeep = PlayerPrefs.GetInt("LastAutoSavesToKeep", GetComponent<ConfigManager>().GameSaveConfig.lastAutoSavesToKeep); // for default values load options from Config
        //Debug.LogWarning("Do autosave: " + gameOpt.DoAutoSave);
        //Debug.LogWarning("Do LastAutoSavesToKeep: " + gameOpt.LastAutoSavesToKeep);
    }

}
