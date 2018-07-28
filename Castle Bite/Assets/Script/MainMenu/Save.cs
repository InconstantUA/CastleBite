using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Save : MonoBehaviour {
    public string saveName;

    void OnEnable()
    {
        // set Toggle's text
        // pad right will calcualte number of spaces required to keep constant string length so brakets are always located at the edges
        GetComponent<Text>().text = "[ " + saveName.PadRight(28) + "]";
    }
}
