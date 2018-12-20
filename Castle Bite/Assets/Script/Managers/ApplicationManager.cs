using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {
    public static ApplicationManager Instance { get; private set; }

    CoroutineQueue coroutineQueue;

    void Awake()
    {
        Instance = this;
        // Create a coroutine queue that can run max 1 coroutine at once
        coroutineQueue = new CoroutineQueue(1, StartCoroutine);
    }

    void Start()
    {
        // init game on start
        // enable main menu
        UIRoot.Instance.GetComponentInChildren<MainMenuManager>(true).gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
