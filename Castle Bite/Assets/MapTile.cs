using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTile : MonoBehaviour {
    [SerializeField]
    Terra terra;
    Text tileHighliterText;

    public Terra Terra
    {
        get
        {
            return terra;
        }
    }

    // Use this for initialization
    void Awake()
    {
        tileHighliterText = GetComponentInChildren<Text>(true);
    }
    
    public void EnterBrowseMode()
    {
        // enable TileHighligter
        tileHighliterText.gameObject.SetActive(true);
    }

    public void EnterDragMode()
    {
        // disable TileHighligter
        tileHighliterText.gameObject.SetActive(false);
    }

    public void EnterAnimationMode()
    {
        // disable TileHighligter
        tileHighliterText.gameObject.SetActive(false);
    }
}
