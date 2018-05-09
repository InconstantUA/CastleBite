using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuKeyboardControl : MonoBehaviour {
    Button[] menuObjList;
    int currentlySelectedMenuID;
    int previouslySelectedMenuID;
    Color tmpColor;

    // Use this for initialization
    void Start () {
        // Init list of all buttons in the menu
        menuObjList = GetComponentsInChildren<Button>();
        // pre-select first element in the menu
        currentlySelectedMenuID = 0;
        previouslySelectedMenuID = 0;
        HighlightSelectedMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) == true)
        {
            // audio.PlayOneShot(blip);
            // Disable cursor, to not conflict with keyboard input
            // If cursor was enabled, then we should verify if highlighted menu has not changed
            // and we cannot rely on previoiusly set previouslySelectedMenuID, 
            // that is why we each time get it from scratch using GetCurrentlySelectedMenuID();
            Cursor.visible = false;
            currentlySelectedMenuID = GetCurrentlySelectedMenuID();
            previouslySelectedMenuID = currentlySelectedMenuID;
            if (currentlySelectedMenuID < menuObjList.Length - 1)
            {
                currentlySelectedMenuID += 1;
            }
            else
            {
                currentlySelectedMenuID = 0;
            }
            DimmPreviouslySelectedMenu();
            HighlightSelectedMenu();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) == true)
        {
            // audio.PlayOneShot(blip);
            Cursor.visible = false;
            currentlySelectedMenuID = GetCurrentlySelectedMenuID();
            previouslySelectedMenuID = currentlySelectedMenuID;
            if (currentlySelectedMenuID > 0)
            {
                currentlySelectedMenuID -= 1;
            }
            else
            {
                currentlySelectedMenuID = menuObjList.Length - 1;
            }
            DimmPreviouslySelectedMenu();
            HighlightSelectedMenu();
        }
    }

    int GetCurrentlySelectedMenuID()
    {
        // it is possible that id has changed, because there was mouse activity
        // that is why I need to get active menu id set by mouse and verify if it is the same as for keyboard
        int id;
        // to get currently highlighted menu we compare its text color with highlighted color
        for (id=0; id < menuObjList.Length; id++)
        {
            // do not compare alpha (transparancy) because it is different, because we set it to 1(255)
            Debug.Log("Comparing " + menuObjList[id] + " menu");
            Color btnHClr = menuObjList[id].colors.highlightedColor;
            Color txtClr = menuObjList[id].GetComponentInChildren<Text>().color;
            if (((int)(txtClr.r * 1000) == (int)(btnHClr.r * 1000)) || ((int)(txtClr.g * 1000) == (int)(btnHClr.g * 1000)) || ((int)(txtClr.b * 1000) == (int)(btnHClr.b * 1000)))
            {
                // found id
                break;
            }
        }
        Debug.Log("Currently highlighted menu object is " + menuObjList[id].name);
        return id;
    }

    void DimmPreviouslySelectedMenu()
    {
        // Remove highlight for previously selected menu
        if (menuObjList[previouslySelectedMenuID].interactable)
        {
            tmpColor = menuObjList[previouslySelectedMenuID].colors.normalColor;
        }
        else
        {
            tmpColor = menuObjList[previouslySelectedMenuID].colors.disabledColor;
        }
        tmpColor.a = 1;
        menuObjList[previouslySelectedMenuID].GetComponentInChildren<Text>().color = tmpColor;
        Debug.Log("Dimm menu object " + menuObjList[previouslySelectedMenuID].name);
    }

    void HighlightSelectedMenu()
    {
        // Highlight newly selected menu
        if (menuObjList[currentlySelectedMenuID].interactable)
        {
            tmpColor = menuObjList[currentlySelectedMenuID].colors.highlightedColor;
        }
        else
        {
            tmpColor = menuObjList[currentlySelectedMenuID].colors.disabledColor;
        }
        tmpColor.a = 1;
        menuObjList[currentlySelectedMenuID].GetComponentInChildren<Text>().color = tmpColor;
        Debug.Log("Highlight menu object " + menuObjList[currentlySelectedMenuID].name);
    }
}
