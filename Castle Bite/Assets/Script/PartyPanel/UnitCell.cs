using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCell : MonoBehaviour {
    [SerializeField]
    UnitSize cellSize;

    public UnitSize GetCellSize()
    {
        return cellSize;
    }

}
