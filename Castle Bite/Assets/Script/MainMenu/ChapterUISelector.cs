using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterUISelector : MonoBehaviour
{
    Chapter lChapter;

    public Chapter LChapter
    {
        get
        {
            return lChapter;
        }

        set
        {
            lChapter = value;
        }
    }

    void OnEnable()
    {
        // set Toggle's text
        // pad right will calcualte number of spaces required to keep constant string length so brakets are always located at the edges
        GetComponent<Text>().text = "[ " + lChapter.ChapterData.chapterDisplayName.PadRight(28) + "]";
    }
}
