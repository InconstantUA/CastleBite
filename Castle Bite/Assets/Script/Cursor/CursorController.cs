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
    [SerializeField]
    private Texture2D invenotryUnitCursor;
    [SerializeField]
    private Texture2D dragUnitCursor;
    [SerializeField]
    private Texture2D blockInputCursor;
    [SerializeField]
    private Texture2D selectionHandCursor;
    [SerializeField]
    private Texture2D grabHandCursor;
    [SerializeField]
    private Texture2D openDoorsCursor;
    [SerializeField]
    private Texture2D editHeroCursor;
    [SerializeField]
    private Texture2D attackCursor;
    [SerializeField]
    private Texture2D moveArrowCursor;
    [SerializeField]
    private Texture2D editTextCursor;

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
        //Debug.Log("SetNormalCursor");
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetBlockInputCursor()
    {
        //Debug.Log("SetBlockInputCursor");
        Cursor.SetCursor(blockInputCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetSelectionHandCursor()
    {
        Cursor.SetCursor(selectionHandCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetGrabHandCursor()
    {
        Cursor.SetCursor(grabHandCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetOpenDoorsCursor()
    {
        //Debug.Log("SetOpenDoorsCursor");
        Cursor.SetCursor(openDoorsCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetEditHeroCursor()
    {
        //Debug.Log("SetEditHeroCursor");
        Cursor.SetCursor(editHeroCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetAttackCursor()
    {
        Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetMoveArrowCursor()
    {
        Cursor.SetCursor(moveArrowCursor, Vector2.zero, CursorMode.Auto);
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

    public void SetInvenotryUnitCursor()
    {
        Cursor.SetCursor(invenotryUnitCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetDragUnitCursor()
    {
        Cursor.SetCursor(dragUnitCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetEditTextCursor()
    {
        Cursor.SetCursor(editTextCursor, Vector2.zero, CursorMode.Auto);
    }

}
