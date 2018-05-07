using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickStart : MonoBehaviour {
    public Color newHighlightColor;
    public string newSceneName;

    //private void SetCollider()
    //{
    //    Text text = GetComponent<Text>();
    //    if(text)
    //    {
    //        Rect box = text.GetPixelAdjustedRect();
    //        BoxCollider boxCollider = (BoxCollider)gameObject.AddComponent(typeof(BoxCollider));
    //        boxCollider.isTrigger = true;
    //        Debug.Log("boxX = " + box.width + " boxY = " + box.height);
    //        Debug.Log("text " + text.text);
    //        boxCollider.size = new Vector3(box.width, box.height, 1);
    //    }
    //}

    //void Start()
    //{
    //    SetCollider();
    //}

    private void OnMouseOver()
    {
        GetComponent<Text>().color = newHighlightColor;
    }

    private void OnMouseUp()
    {
        SceneManager.LoadScene(newSceneName);
    }


}
