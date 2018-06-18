using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalInfo : MonoBehaviour {

    [SerializeField]
    string header;
    //[SerializeField]
    //string line1;
    //[SerializeField]
    //string line2;
    //[SerializeField]
    //string line3;
    //[SerializeField]
    //string line4;
    //[SerializeField]
    //string line5;
    //[SerializeField]
    //string line6;
    //[SerializeField]
    //string line7;
    //[SerializeField]
    //string line8;
    [SerializeField]
    string[] lines = new string[8];

    public string GetHeader()
    {
        return header;
    }

    public string[] GetLines()
    {
        return lines;
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
