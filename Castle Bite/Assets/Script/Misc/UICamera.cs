using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    public static UICamera Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

}
