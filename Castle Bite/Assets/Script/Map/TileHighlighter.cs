using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlighter : MonoBehaviour {

    public void EnterBrowseMode()
    {
        // make TileHighligter visible
        // enable txt
        GetComponentInChildren<Text>(true).gameObject.SetActive(true);
    }

    public void EnterDragMode()
    {
        // make TileHighligter invisible
        // disable txt
        GetComponentInChildren<Text>(true).gameObject.SetActive(false);
    }

    public void EnterAnimationMode()
    {
        // make TileHighligter invisible
        // disable txt
        GetComponentInChildren<Text>(true).gameObject.SetActive(false);
    }
}
