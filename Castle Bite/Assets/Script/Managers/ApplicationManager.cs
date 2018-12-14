using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {
    public static ApplicationManager Instance { get; private set; }

    [SerializeField]
    LoadGame loadGame;

    CoroutineQueue coroutineQueue;

    void Awake()
    {
        Instance = this;
        // Create a coroutine queue that can run max 1 coroutine at once
        coroutineQueue = new CoroutineQueue(1, StartCoroutine);
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator SetEndingGameScreen()
    {
        // Activate Loading screen
        UIRoot.Instance.GetComponentInChildren<EndingGameScreen>(true).SetActive(true);
        // Set waiting cursor
        CursorController.Instance.SetBlockInputCursor();
        // Activate input blocker
        InputBlocker.Instance.SetActive(true);
        // Replace map with the clear Map
        Debug.LogWarning("Replace map with new map from template");
        // Deactivate loading screen after clean
        yield return new WaitForSeconds(2);
        // Deactivate Loading screen
        UIRoot.Instance.GetComponentInChildren<EndingGameScreen>(true).SetActive(false);
        // Set normal cursor
        CursorController.Instance.SetNormalCursor();
        // Disable input blocker
        InputBlocker.Instance.SetActive(false);
    }

    public void EndCurrentGame()
    {
        coroutineQueue.Run(SetEndingGameScreen());
    }

}
