using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {
    public static ApplicationManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
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
