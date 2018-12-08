using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour {
    public static UIRoot Instance { get; private set; }

	void Awake () {
        Instance = this;
	}
	
}
