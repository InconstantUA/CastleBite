using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {
    [SerializeField]
    private Texture2D normalCursor;
    [SerializeField]
    private Texture2D dismissUnitCursor;
    [SerializeField]
    private Texture2D healUnitCursor;
    [SerializeField]
    private Texture2D resurectUnitCursor;
    public static CursorController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        SetNormalCursor();
    }

    public void SetNormalCursor()
    {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetDismissUnitCursor()
    {
        Cursor.SetCursor(dismissUnitCursor, new Vector2(16, 16), CursorMode.Auto);
    }

    public void SetHealUnitCursor()
    {
        Cursor.SetCursor(healUnitCursor, new Vector2(16, 16), CursorMode.Auto);
    }

    public void SetResurectUnitCursor()
    {
        Cursor.SetCursor(resurectUnitCursor, new Vector2(16, 16), CursorMode.Auto);
    }


    //// Update is called once per frame
    //void Update () {

    //}
}
