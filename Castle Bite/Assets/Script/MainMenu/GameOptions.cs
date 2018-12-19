using UnityEngine;

public class GameOptions : MonoBehaviour {

    public static GameOptions Instance;

    public struct GameOpt
    {
        public int autosave;

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
        }
        // verify if game options were instantiated by some other scene, when there is already gameOptions present
        else if (Instance != this)
        {
            // destroy this instance of game options to keep only one instance
            Destroy(gameObject);
            Debug.Log("Destroy current game options");
        }
    }
}
