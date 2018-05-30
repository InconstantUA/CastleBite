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

    public void SetCityActiveViewStateCursor(City.CityViewActiveState requiredState, bool doActivate)
    {
        if(doActivate)
        {
            switch (requiredState)
            {
                case City.CityViewActiveState.ActiveDismiss:
                    Cursor.SetCursor(dismissUnitCursor, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case City.CityViewActiveState.ActiveHeal:
                    Cursor.SetCursor(healUnitCursor, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case City.CityViewActiveState.ActiveResurect:
                    Cursor.SetCursor(resurectUnitCursor, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case City.CityViewActiveState.ActiveHeroEquipment:
                    Cursor.SetCursor(invenotryUnitCursor, Vector2.zero, CursorMode.Auto);
                    break;
                case City.CityViewActiveState.ActiveUnitDrag:
                    Cursor.SetCursor(dragUnitCursor, Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    Debug.LogError("Unknown condition");
                    break;
            }
        }
        else
        {
            SetNormalCursor();
        }
    }

    //// Update is called once per frame
    //void Update () {

    //}
}
