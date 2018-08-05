using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnsManager : MonoBehaviour {
    public static TurnsManager Instance { get; private set; }

    void Awake()
    {
        // verify if instance already initialized
        if (Instance == null)
        {
            // prevent this game object (game options) to be destroyed on scenes change
            // this only works on root objects in a scene, so disabled for now
            // DontDestroyOnLoad(gameObject);
            // initialize instance with this game object
            Instance = this;
        }
        // verify if instance were instantiated by some other scene, when there is already instance present
        else if (Instance != this)
        {
            // destroy this instance of game options to keep only one instance
            Destroy(gameObject);
        }
    }

    public GamePlayer GetActivePlayer()
    {
        // .. Fix
        return transform.root.Find("GamePlayers").GetComponentInChildren<GamePlayer>();
    }
}
