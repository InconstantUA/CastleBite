using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCell : MonoBehaviour {
    [SerializeField]
    PartyUnit.UnitSize cellSize;

    public PartyUnit.UnitSize GetCellSize()
    {
        return cellSize;
    }
    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
