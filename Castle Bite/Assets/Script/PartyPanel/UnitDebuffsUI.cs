using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDebuffsUI : MonoBehaviour {
    CoroutineQueue queue;

    // Use this for initialization
    void Start()
    {
        // Create a coroutine queue that can run max two coroutines at once
        queue = new CoroutineQueue(1, StartCoroutine);
    }

    public CoroutineQueue GetQueue()
    {
        return queue;
    }

    //// Update is called once per frame
    //void Update () {

    //}
}
