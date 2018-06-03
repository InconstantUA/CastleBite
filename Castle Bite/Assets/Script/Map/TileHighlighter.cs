using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlighter : MonoBehaviour {
    bool isOn;
    [SerializeField]
    float animationDuration = 1f;
    Text txt;

    // Use this for initialization
    void Start() {
        isOn = true;
        txt = GetComponentInChildren<Text>();
        // InvokeRepeating("Blink", 0, animationDuration);
        //Debug.Log("Start repeating");
    }

    void Blink()
    {
        if (isOn)
        {
            // fade away until is off
            txt.gameObject.SetActive(false);
            //Debug.Log("disable");
            isOn = false;
        }
        else
        {
            // appear auntil is on
            txt.gameObject.SetActive(true);
            //Debug.Log("enable");
            isOn = true;
        }
        // yield return new WaitForSeconds(0.2f);
    }

    public void OnChange()
    {
        // act based on the Map mode
        switch (transform.parent.GetComponent<MapManager>().GetMode())
        {
            case MapManager.Mode.Browse:
                EnterBrowseMode();
                break;
            case MapManager.Mode.Drag:
                EnterDragMode();
                break;
            case MapManager.Mode.Selection:
                break;
            case MapManager.Mode.Move:
                break;
            default:
                Debug.LogError("Unknown mode");
                break;
        }
    }

    public void SetActive(bool state)
    {
        txt.gameObject.SetActive(state);
    }

    void EnterBrowseMode()
    {
        // make TileHighligter visible
        // enable txt
        txt.gameObject.SetActive(true);
        // stop blinking (selection) animation
        CancelInvoke();
    }

    void EnterDragMode()
    {
        // make TileHighligter invisible
        // disable txt
        txt.gameObject.SetActive(false);
        // stop blinking (selection) animation
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
