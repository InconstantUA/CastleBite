using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour {

    public static GameOptions options;

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

	void Awake() {
        // verify if game options already initialized
		if (options == null)
        {
            // prevent this game object (game options) to be destroyed on scenes change
            // this only works on root objects in a scene, so disabled for now
            // DontDestroyOnLoad(gameObject);
            // initialize gameOptions with this game object
            options = this;
        }
        // verify if game options were instantiated by some other scene, when there is already gameOptions present
        else if (options != this)
        {
            // destroy this instance of game options to keep only one instance
            Destroy(gameObject);
        }
	}
}
