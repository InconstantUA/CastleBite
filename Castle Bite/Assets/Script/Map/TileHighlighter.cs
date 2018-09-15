using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlighter : MonoBehaviour {
    //bool isOn;
    //float animationDuration = 1f;
    Text tileHighliter;

    // Use this for initialization
    void Awake() {
        //isOn = true;
        tileHighliter = gameObject.GetComponentInChildren<Text>(true) as Text;
        // txt = transform.Find("Text").GetComponent<Text>();
    }

    public void EnterBrowseMode()
    {
        // make TileHighligter visible
        // enable txt
        tileHighliter.gameObject.SetActive(true);
        //// stop blinking (selection) animation
        //CancelInvoke();
    }

    public void EnterDragMode()
    {
        // make TileHighligter invisible
        // disable txt
        tileHighliter.gameObject.SetActive(false);
    }

    public void EnterAnimationMode()
    {
        // make TileHighligter invisible
        // disable txt
        tileHighliter.gameObject.SetActive(false);
    }
}
