using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    public Color newHighlightColor;
    private void OnMouseOver()
    {
        GetComponent<TextMesh>().color = newHighlightColor;
    }
}
