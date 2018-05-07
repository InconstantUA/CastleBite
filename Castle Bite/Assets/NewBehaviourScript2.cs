using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class NewBehaviourScript2 : MonoBehaviour
{
    public Font font;
    private TextMesh tm;

    // Use this for initialization
    void Start()
    {
        tm = this.gameObject.AddComponent<TextMesh>();
        tm.font = font;
        // tm.renderer.material = font.material;
        tm.characterSize = 1;
        tm.text = "test";
    }
}