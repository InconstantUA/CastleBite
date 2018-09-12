using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[Serializable]
public struct PositionOnMap
{
    public float offsetMinX;
    public float offsetMinY;
    public float offsetMaxX;
    public float offsetMaxY;
}

public class MapManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum Mode {
        Browse,
        Drag,
        Animation
    };

    public enum Selection
    {
        None,
        PlayerHero,
        PlayerCity
    }

    [SerializeField]
    GameMap lMap;
    [SerializeField]
    Mode mode;
    [SerializeField]
    Selection selection;
    //Vector3 startPosition;
    // Transform startParent;
    [SerializeField]
    int tileSize = 16;
    [SerializeField]
    Color defaultTileHighlighterColor;
    [SerializeField]
    Color defaultTileHighlighterNotEnoughMovePoints;
    [SerializeField]
    Color enemyOnWayColor;
    [SerializeField]
    Color enemyOnWayColorNotEnoughMovePoints;
    [SerializeField]
    Color sameFactionRelationColor;
    [SerializeField]
    Color sameFactionRelationColorNotEnoughMovePoints;
    [SerializeField]
    Color alliedFactionRelationColor;
    [SerializeField]
    Color alliedFactionRelationColorNotEnoughMovePoints;
    [SerializeField]
    Color neutralFactionRelationColor;
    [SerializeField]
    Color neutralFactionRelationColorNotEnoughMovePoints;
    [SerializeField]
    Color atWarFactionRelationColor;
    [SerializeField]
    Color atWarFactionRelationColorNotEnoughMovePoints;
    [SerializeField]
    Color unknownRelationshipsUnitColor;
    [SerializeField]
    Color unknownRelationshipsUnitColorNotEnoughMovePoints;
    Transform tileHighlighterTr;
    Color tileHighlighterColor;
    TileHighlighter tileHighlighter;
    // for pathfinding
    bool[,] tilesmap;
    int tileMapWidth = 60;
    int tileMapHeight = 60;
    MapHero selectedMapHero;
    MapCity selectedCity;
    NesScripts.Controls.PathFind.Point selectedTargetPathPoint;
    NesScripts.Controls.PathFind.Grid grid;
    List<NesScripts.Controls.PathFind.Point> movePath;
    GameObject[,] tileHighlighters;
    // for hero moving
    public float heroMoveSpeed = 10.1f;
    public float heroMoveSpeedDelay = 0.1f;
    //TileState lastTileState = TileState.Terrain;
    NesScripts.Controls.PathFind.Point lastPathTile;

    // Map Sprite size
    float mapWidth;
    float mapHeight;
    // Screen size
    // Screen.width;
    // Screen.height;
    // Those are default and calculated on Start
    float xMinDef;
    float xMaxDef;
    float yMinDef;
    float yMaxDef;
    // those are variables depending on the mouse onDragStart position
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // for debug
    Vector3 mousePosition;
    Vector3 mapPosiiton;

    // Vector3 mouseOnDragStartPosition;
    Vector3 mouseOnDownStartPosition;
    float xCorrectionOnDragStart;
    float yCorrectionOnDragStart;

    // for animation and transition between states
    CoroutineQueue queue;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Map manager: pointer enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Map manager: pointer exit");
        // Hide tile highlighter
        tileHighlighterColor = new Color32(0, 0, 0, 0);
    }

    public CoroutineQueue Queue
    {
        get
        {
            return queue;
        }

        set
        {
            queue = value;
        }
    }

    public Mode GetMode()
    {
        return mode;
    }

    public void SetSelectedHero(MapHero sltdHero)
    {
        selectedMapHero = sltdHero;
    }

    public MapHero GetSelectedHero()
    {
        return selectedMapHero;
    }

    public void SetSelectedCity(MapCity sltCity)
    {
        selectedCity = sltCity;
    }

    void Awake()
    {
        // Create a coroutine queue that can run max 1 coroutine at once
        queue = new CoroutineQueue(1, StartCoroutine);
    }

    void Start()
    {
        // For map drag
        //  get map width and height
        mapWidth = gameObject.GetComponentInChildren<SpriteRenderer>().size.x;
        mapHeight = gameObject.GetComponentInChildren<SpriteRenderer>().size.y;
        //  calculate screen borders
        //  get maximum possible offset
        float xDeltaMax = (mapWidth - Screen.width) / 2;
        float yDeltaMax = (mapHeight - Screen.height) / 2;
        //  border depend on the center position
        //  because canvas is positioned in the lower left corner
        //  mouse's 0:0 coordinates are located at the same position
        //  center is screen width and height divided by 2
        xMinDef = (Screen.width / 2) - xDeltaMax;
        yMinDef = (Screen.height / 2) - yDeltaMax;
        xMaxDef = (Screen.width / 2) + xDeltaMax;
        yMaxDef = (Screen.height / 2) + yDeltaMax;
        // For map tile highligter in selection mode
        tileHighlighterTr = transform.Find("TileHighlighter");
        tileHighlighter = tileHighlighterTr.GetComponent<TileHighlighter>();
        // Init map mode
        SetMode(Mode.Browse);
        // Initialize path finder
        InitTilesMap();
        InitPathFinder();
        InitializePathHighligters();
    }

    // called via Unity Editor
    public void SetCitiesNamesVisible(bool doShow)
    {
        Debug.Log("Show all cities names: " + doShow.ToString());
        //// Init map object
        //MapObject mapObject;
        foreach (MapCity mapCity in transform.GetComponentsInChildren<MapCity>(true))
        {
            // Set always on
            mapCity.GetComponent<MapObject>().SetAlwaysOn(doShow);
            //// get map object
            //mapObject = mapCity.GetComponent<MapObject>();
            //// turn on label always on flag
            //mapObject.LabelAlwaysOn = doShow;
            //// verify if we need to show or hide all labels
            //if (doShow)
            //{
            //    mapObject.GetComponentInChildren<MapObjectLabel>(true).SetAlwaysOnLabelColor();
            //}
            //else
            //{
            //    mapObject.GetComponentInChildren<MapObjectLabel>(true).HideLabel();
            //}
        }
    }

    // called via Unity Editor
    public void SetHeroesNamesVisible(bool doShow)
    {
        Debug.Log("Show all heroes names: " + doShow.ToString());
        //// Init map object
        //MapObject mapObject;
        foreach (MapHero mapHero in transform.GetComponentsInChildren<MapHero>(true))
        {
            // Set always on
            mapHero.GetComponent<MapObject>().SetAlwaysOn(doShow);
            //// get map object
            //mapObject = mapHero.GetComponent<MapObject>();
            //// turn on label always on flag
            //mapObject.LabelAlwaysOn = doShow;
            //// verify if we need to show or hide all labels
            //if (doShow)
            //{
            //    mapObject.GetComponentInChildren<MapObjectLabel>(true).SetAlwaysOnLabelColor();
            //}
            //else
            //{
            //    mapObject.GetComponentInChildren<MapObjectLabel>(true).HideLabel();
            //}
        }
    }

    // called via Unity Editor
    public void SetPlayerIncomeVisible(bool doShow)
    {
        Debug.Log("Show player income: " + doShow.ToString());
        transform.root.Find("MiscUI/TopInfoPanel/Middle/CurrentGold").gameObject.SetActive(doShow);
    }

    // called via Unity Editor
    public void SetManaSourcesVisible(bool doShow)
    {
        Debug.Log("Show All Mana sources names visible: " + doShow.ToString());
    }

    // called via Unity Editor
    public void SetTreasureChestsVisible(bool doShow)
    {
        Debug.Log("Show All Treasure chests names visible: " + doShow.ToString());
        foreach (MapItem mapItem in transform.GetComponentsInChildren<MapItem>(true))
        {
            // Set always on
            mapItem.GetComponent<MapObject>().SetAlwaysOn(doShow);
        }
    }

    bool PositionIsWithinTilesMap(Vector2Int pos)
    {
        if (
            (pos.x > 0) &&
            (pos.y > 0) &&
            (pos.x < tilesmap.GetLength(0) - 1) &&
            (pos.y < tilesmap.GetLength(1) - 1)
           )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void InitTilesMap()
    {
        // create the tiles map
        tilesmap = new bool[tileMapWidth, tileMapHeight];
        // set values here....
        // true = walkable, false = blocking
        for (int x = 0; x < tilesmap.GetLength(0); x += 1)
        {
            for (int y = 0; y < tilesmap.GetLength(1); y += 1)
            {
                // Debug.Log(tilesmap[x, y].ToString());
                tilesmap[x, y] = true;
            }
        }
        // Set all tiles occupied by heroes or cities on map as non-passable
        foreach (MapHero party in transform.GetComponentsInChildren<MapHero>())
        {
            // verify if not null
            if (party)
            {
                Vector2Int pos = GetTilePosition(party.transform);
                if (PositionIsWithinTilesMap(pos))
                {
                    tilesmap[pos.x, pos.y] = false;
                }
            }
        }
        foreach (MapCity city in transform.GetComponentsInChildren<MapCity>())
        {
            // verify if not null
            if (city)
            {
                Vector2Int pos = GetTilePosition(city.transform);
                if (PositionIsWithinTilesMap(pos))
                {
                    tilesmap[pos.x, pos.y] = false;
                }
            }
        }
        foreach (MapItem mapItem in transform.GetComponentsInChildren<MapItem>())
        {
            // verify if not null
            if (mapItem)
            {
                Vector2Int pos = GetTilePosition(mapItem.transform);
                if (PositionIsWithinTilesMap(pos))
                {
                    tilesmap[pos.x, pos.y] = false;
                }
            }
        }
    }

    void InitPathFinder()
    {
        // create a grid
        grid = new NesScripts.Controls.PathFind.Grid(tilesmap);
    }

    void UpdateGridBasedOnHighlightedTile()
    {
        // reinitialize tile map to reset all previous changes
        InitTilesMap();
        // Get highligted tile state
        Vector2Int highlighterPosition = GetTileHighlighterPosition();
        NesScripts.Controls.PathFind.Point highlightedPoint = new NesScripts.Controls.PathFind.Point(highlighterPosition.x, highlighterPosition.y);
        if ((highlightedPoint.x > grid.nodes.GetLength(0) - 1)
            || (highlightedPoint.y > grid.nodes.GetLength(1) - 1)
            || (highlightedPoint.x < 0)
            || (highlightedPoint.y < 0))
        {
            // nothing to do, mouse is over the screen
        }
        else
        {
            if (GetObjectOnTile(highlightedPoint))
            {
                // some object is present
                // adjust grid to make this tile passable (highlightable)
                // this is needed to get move path if user clicked on a occupied tile
                // but this is not needed to be passable if user clicked on other tile
                tilesmap[highlightedPoint.x, highlightedPoint.y] = true;
            }
            else
            {
                // no object on tile, just terrain
            }
            //TileState highlightedTileState = GetObjectOnTile(highlightedPoint);
            //// highlight based on the occupation type
            //switch (highlightedTileState)
            //{
            //    case TileState.Terrain:
            //        // nothing to do
            //        break;
            //    case TileState.Party:
            //    case TileState.City:
            //    case TileState.Treasure:
            //        // adjust grid to make this tile passable
            //        tilesmap[highlightedPoint.x, highlightedPoint.y] = true;
            //        break;
            //    default:
            //        Debug.LogError("Unknown tile state " + highlightedTileState.ToString());
            //        break;
            //}
        }
        // Upgrade grid
        grid.UpdateGrid(tilesmap);
    }


    void SetTileHighlighterToMousePoistion()
    {
        // Update position
        int x = Mathf.FloorToInt(Input.mousePosition.x / tileSize);
        int y = Mathf.FloorToInt(Input.mousePosition.y / tileSize);
        tileHighlighterTr.position = new Vector3(x, y, 0) * tileSize;
        // Debug.Log("Tile: " + x + ";" + y);
    }

    GameObject GetObjectUnderMouse()
    {
        // This requires 2d box collider to be attached to the object
        // also triggers if the object's text or image is not set as raycast target
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawLine(ray.origin, Camera.main.transform.forward * 50000000, Color.red);
        //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, Camera.main.transform.forward);
        if (hit)
        {
            string name = hit.collider.gameObject.name;
            Debug.Log("hit " + name);
            return hit.collider.gameObject;
        }
        return null;
    }

    List<GameObject> GetActiveLabels()
    {
        List<GameObject> labels = new List<GameObject>();
        // loop through all lables and find the one which is active now
        // Set all tiles occupied by heroes or cities on map as non-passable
        foreach (MapHero mapHero in transform.GetComponentsInChildren<MapHero>())
        {
            // verify if not null
            if (mapHero)
            {
                // verify if mouse is over label
                MapObjectLabel label = mapHero.GetComponentInChildren<MapObjectLabel>();
                if (label.IsMouseOver)
                {
                    labels.Add(label.gameObject);
                }
            }
        }
        foreach (MapCity mapCity in transform.GetComponentsInChildren<MapCity>())
        {
            // verify if not null
            if (mapCity)
            {
                // verify if mouse is over label
                MapObjectLabel label = mapCity.GetComponentInChildren<MapObjectLabel>();
                if (label.IsMouseOver)
                {
                    labels.Add(label.gameObject);
                }
            }
        }
        return labels;
    }

    public void UpdateTileHighighterBasedOnMousePosition()
    {
        Vector2Int highlighterPosition = GetTileHighlighterPosition();
        NesScripts.Controls.PathFind.Point highlightedPoint = new NesScripts.Controls.PathFind.Point(highlighterPosition.x, highlighterPosition.y);
        if ((highlightedPoint.x > grid.nodes.GetLength(0) - 1)
            || (highlightedPoint.y > grid.nodes.GetLength(1) - 1)
            || (highlightedPoint.x < 0)
            || (highlightedPoint.y < 0))
        {
            // nothing to do, mouse is over the screen
        }
        else
        {
            switch (mode)
            {
                case Mode.Browse:
                    // predefine colors
                    Color32 darkBlue = new Color32(0, 0, 128, 255);
                    Color32 darkCyan = new Color32(0, 128, 128, 255);
                    Color32 darkYellow = new Color32(128, 122, 0, 255);
                    Color32 darkRed = new Color32(128, 0, 0, 255);
                    // Update tile highlighter position
                    SetTileHighlighterToMousePoistion();
                    // set default tile highlighter color
                    tileHighlighterColor = Color.white;
                    // get game object below the tile highlighter
                    GameObject childGameObject = GetObjectOnTile(highlightedPoint); // return map's objects: hero, city, other..
                    MapHero mapHero = null;
                    MapCity mapCity = null;
                    MapItem mapItem = null;
                    bool label = false;
                    if (GetActiveLabels().Count > 0)
                    {
                        // verify if mouse is over label
                        label = true;
                    }
                    //GameObject getObjectUnderMouse = GetObjectUnderMouse();
                    if (childGameObject)
                    {
                        // predefine variables
                        mapHero = childGameObject.GetComponent<MapHero>();
                        mapCity = childGameObject.GetComponent<MapCity>();
                        mapItem = childGameObject.GetComponent<MapItem>();
                        // act based on current selection and object under mouse cursor
                        switch (selection)
                        {
                            case Selection.None:
                            case Selection.PlayerCity:
                                // act based on the object over which mouse is now
                                if (mapHero)
                                {
                                    // change cursor to different based on the relationships between factions
                                    Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapHero.LHeroParty.Faction);
                                    switch (relationships)
                                    {
                                        case Relationships.State.SameFaction:
                                            tileHighlighterColor = darkBlue;
                                            break;
                                        case Relationships.State.Allies:
                                            tileHighlighterColor = darkCyan;
                                            break;
                                        case Relationships.State.Neutral:
                                            tileHighlighterColor = darkYellow;
                                            break;
                                        case Relationships.State.AtWar:
                                            tileHighlighterColor = darkRed;
                                            break;
                                        default:
                                            Debug.LogError("Unknown relationships " + relationships.ToString());
                                            break;
                                    }
                                }
                                if (mapCity)
                                {
                                    // check relationships with active player
                                    Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapCity.LCity.CityFaction);
                                    switch (relationships)
                                    {
                                        case Relationships.State.SameFaction:
                                            tileHighlighterColor = darkBlue;
                                            break;
                                        case Relationships.State.Allies:
                                            tileHighlighterColor = darkCyan;
                                            break;
                                        case Relationships.State.Neutral:
                                            tileHighlighterColor = darkYellow;
                                            break;
                                        case Relationships.State.AtWar:
                                            tileHighlighterColor = darkRed;
                                            break;
                                        default:
                                            Debug.LogError("Unknown relationships " + relationships.ToString());
                                            break;
                                    }
                                }
                                if (mapItem)
                                {
                                    tileHighlighterColor = darkYellow;
                                }
                                // at this stage we assume that pointer is over map terrain
                                // nothing to do, just keep normal pointer, which was set after exit highlighting other objects
                                break;
                            case Selection.PlayerHero:
                                // act based on the object over which mouse is now
                                if (mapHero)
                                {
                                    // change cursor to different based on the relationships between factions
                                    Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapHero.LHeroParty.Faction);
                                    switch (relationships)
                                    {
                                        case Relationships.State.SameFaction:
                                            // highlighted hero belongs to player
                                            // check if this is the same hero as selected
                                            if (mapHero.GetInstanceID() == selectedMapHero.GetInstanceID())
                                            {
                                                tileHighlighterColor = Color.blue;
                                            }
                                            else
                                            {
                                                tileHighlighterColor = Color.blue;
                                            }
                                            break;
                                        case Relationships.State.Allies:
                                            tileHighlighterColor = Color.cyan;
                                            break;
                                        case Relationships.State.Neutral:
                                            tileHighlighterColor = Color.yellow;
                                            break;
                                        case Relationships.State.AtWar:
                                            tileHighlighterColor = Color.red;
                                            break;
                                        default:
                                            Debug.LogError("Unknown relationships " + relationships.ToString());
                                            break;
                                    }
                                }
                                if (mapCity)
                                {
                                    // check relationships with active player
                                    Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapCity.LCity.CityFaction);
                                    switch (relationships)
                                    {
                                        case Relationships.State.SameFaction:
                                            // change cursor to selection hand
                                            tileHighlighterColor = Color.blue;
                                            break;
                                        case Relationships.State.Allies:
                                            tileHighlighterColor = Color.cyan;
                                            break;
                                        case Relationships.State.Neutral:
                                            tileHighlighterColor = Color.yellow;
                                            break;
                                        case Relationships.State.AtWar:
                                            tileHighlighterColor = Color.red;
                                            break;
                                        default:
                                            Debug.LogError("Unknown relationships " + relationships.ToString());
                                            break;
                                    }
                                }
                                if (mapItem)
                                {
                                    tileHighlighterColor = darkYellow;
                                }
                                // at this stage we assume that pointer is over map terrain
                                // controls here are handled by Update() function
                                break;
                            default:
                                Debug.LogError("Unknown selection " + selection.ToString());
                                break;
                        }
                    }
                    else if (label)
                    {
                        // Hide tile highlighter if mouse if over label
                        // Debug.Log("Hide tile highlighter");
                        tileHighlighterColor = new Color32(0, 0, 0, 0);
                    }
                    else
                    {
                        // no object on tile, just terrain
                        // verify if the tile is protected by enemy hero and we are in hero selection mode
                        if (IsTileProtected(GetTileByPosition(Input.mousePosition)))
                        {
                            // transform.root.Find("CursorController").GetComponent<CursorController>().SetAttackCursor();
                            // act based on current selection
                            tileHighlighterColor = Color.red;
                            switch (selection)
                            {
                                case Selection.None:
                                case Selection.PlayerCity:
                                    tileHighlighterColor = darkRed;
                                    break;
                                case Selection.PlayerHero:
                                    tileHighlighterColor = Color.red;
                                    break;
                                default:
                                    Debug.LogError("Unknown selection " + selection.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            //transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
                            //tileHighlighterColor = Color.white;
                        }
                    }
                    // Update color of tile highlighter
                    tileHighlighterTr.GetComponentInChildren<Text>().color = tileHighlighterColor;
                    break;
                case Mode.Animation:
                    // Hide tile highlighter in this mode
                    tileHighlighterColor = new Color32(0, 0, 0, 0);
                    break;
                case Mode.Drag:
                    // Hide tile highlighter in this mode
                    tileHighlighterColor = new Color32(0, 0, 0, 0);
                    break;
                default:
                    Debug.LogError("Unknown mode " + mode.ToString());
                    break;
            }
        }
    }

    void Update()
    {
        // verify if mouse is moving
        if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
        {
            switch (mode)
            {
                case Mode.Browse:
                    // update tile highliter position
                    UpdateGridBasedOnHighlightedTile();
                    UpdateTileHighighterBasedOnMousePosition();
                    //SetTileHighlghterColorAndShapeBasedOnMousePoistion();
                    //SetTileHighlighterToMousePoistion();
                    //UpdateTileHighlighterColor();
                    //// update tile highliter position
                    //UpdateTileHighlighterToMousePoistion();
                    //FindAndHighlightPath();
                    break;
                case Mode.Drag:
                    // do nothing wait for drag to finish
                    break;
                case Mode.Animation:
                    // do nothing, wait for animation to finish
                    break;
                default:
                    Debug.LogError("Unknown mode " + mode.ToString());
                    break;
            }
        }
    }

    public Vector2Int GetTileByPosition(Vector3 position)
    {
        return new Vector2Int
        {
            x = Mathf.FloorToInt(position.x / tileSize),
            y = Mathf.FloorToInt(position.y / tileSize)
        };
    }

    public Vector3 GetPositionByTile(int tileX, int tileY)
    {
        return new Vector3
        {
            x = tileX * tileSize,
            y = tileY * tileSize,
            z = 0
        };
    }

    Vector2Int GetTilePosition(Transform tr)
    {
        return GetTileByPosition(tr.position);
        //return new Vector2Int
        //{
        //    x = Mathf.FloorToInt(tr.position.x / tileSize),
        //    y = Mathf.FloorToInt(tr.position.y / tileSize)
        //};
    }

    Vector2Int GetHeroTilePosition()
    {
        //Vector2Int result = new Vector2Int
        //{
        //    x = Mathf.FloorToInt(selectedHero.transform.position.x / tileSize),
        //    y = Mathf.FloorToInt(selectedHero.transform.position.y / tileSize)
        //};
        return GetTilePosition(selectedMapHero.transform);
    }

    Vector2Int GetTileHighlighterPosition()
    {
        Vector2Int result = new Vector2Int
        {
            x = Mathf.FloorToInt(Input.mousePosition.x / tileSize),
            y = Mathf.FloorToInt(Input.mousePosition.y / tileSize)
        };
        // Debug.Log(result.x.ToString() + ":" + result.y.ToString());
        return result;
    }

    void InitializePathHighligters()
    {
        // todo: better keep them as string and highlight specific elements
        GameObject movePathHighlighterTemplate = transform.root.Find("Templates/UI/MovePathHighlighter").gameObject;
        Transform movePathHighlighterParent = transform.Find("MovePath");
        tileHighlighters = new GameObject[tileMapWidth, tileMapHeight];
        for (int x = 0; x < tileHighlighters.GetLength(0); x += 1)
        {
            for (int y = 0; y < tileHighlighters.GetLength(1); y += 1)
            {
                // Debug.Log(tilesmap[x, y].ToString());
                // create new highlight object
                tileHighlighters[x, y] = Instantiate(movePathHighlighterTemplate, movePathHighlighterParent);
                tileHighlighters[x, y].transform.position = new Vector3(x, y, 0) * tileSize;
                // tileHighlighters[x, y].gameObject.SetActive(true);
            }
        }
    }

    Vector2Int[] GetAllSurroundingTilesPositions(Vector2Int t)
    {
        // [x-1,y+1]    [y+1]   [x+1,y+1]
        // [x-1]        [x,y]   [x+1]
        // [x-1,y-1]    [y-1]   [x+1,y-1]
        // 
        return new Vector2Int[] {
            new Vector2Int(t.x-1, t.y+1),
            new Vector2Int(t.x,   t.y+1),
            new Vector2Int(t.x+1, t.y+1),
            new Vector2Int(t.x-1, t.y  ),
            new Vector2Int(t.x+1, t.y  ),
            new Vector2Int(t.x-1, t.y-1),
            new Vector2Int(t.x,   t.y-1),
            new Vector2Int(t.x+1, t.y-1)
        };
    }

    bool TileIsProtectedByEnemyParty(Vector2Int enemyTilePosition, Vector2Int checkedTile)
    {
        // verify if there is enemy 
        Vector2Int[] protectedTilesAroundEnemy = GetAllSurroundingTilesPositions(enemyTilePosition);
        foreach (Vector2Int protectedTile in protectedTilesAroundEnemy)
        {
            if (protectedTile == checkedTile)
            {
                return true;
            }
        }
        return false;
    }

    //bool PartyIsEnemy(MapHero party)
    //{
    //    // verify if this is friendly or enemy party
    //    //if (party.LinkedPartyTr.GetComponent<HeroParty>().Faction != selectedHero.LinkedPartyTr.GetComponent<HeroParty>().Faction)
    //    if (party.LinkedPartyTr.GetComponent<HeroParty>().Faction != player.Faction)
    //    {
    //        // different faction -enemy
    //        return true;
    //    }
    //    else
    //    {
    //        // same faction - friendly
    //        return false;
    //    }
    //}

    //bool CityIsEnemy(MapCity city)
    //{
    //    // verify if this is friendly or enemy party
    //    //if (city.LinkedCityTr.GetComponent<City>().Faction != selectedHero.LinkedPartyTr.GetComponent<HeroParty>().Faction)
    //    if (city.LinkedCityTr.GetComponent<City>().Faction != player.Faction)
    //    {
    //        // different faction -enemy
    //        return true;
    //    }
    //    else
    //    {
    //        // same faction - friendly
    //        return false;
    //    }
    //}

    GameObject GetObjectOnTile(Vector2Int tilePosition)
    {
        // go over all objects and verify if they are on the tile
        foreach (MapHero party in transform.GetComponentsInChildren<MapHero>())
        {
            // verify if not null
            if (party)
            {
                Vector2Int partyTilePosition = GetTilePosition(party.transform);
                if (partyTilePosition == tilePosition)
                {
                    return party.gameObject;
                }
                //// verify if this is enemy or friendly party
                //if (PartyIsEnemy(party))
                //{
                //    // verify if enemy party located in tile which we need to examine
                //    if (partyTilePosition == tilePosition)
                //    {
                //        return TileState.EnemyParty;
                //    }
                //    // verify if enemy party located in nearby tiles
                //    else if (TileIsProtectedByEnemyParty(partyTilePosition, tilePosition))
                //    {
                //        // Debug.LogWarning("Protected");
                //        return TileState.Protected;
                //    }
                //}
                //else
                //{
                //    // verify if friendly party located in tile
                //    if (partyTilePosition == tilePosition)
                //    {
                //        return TileState.PlayerParty;
                //    }
                //}
            }
        }
        foreach (MapCity city in transform.GetComponentsInChildren<MapCity>())
        {
            // verify if not null
            if (city)
            {
                Vector2Int cityTilePosition = GetTilePosition(city.transform);
                if (cityTilePosition == tilePosition)
                {
                    return city.gameObject;
                }
                //// verify if this is enemy or friendly city
                //if (CityIsEnemy(city))
                //{
                //    // verify if enemy city located in tile
                //    if (cityTilePosition == tilePosition)
                //    {
                //        return TileState.EnemyCity;
                //    }
                //}
                //else
                //{
                //    // verify if friendly party located in tile
                //    if (cityTilePosition == tilePosition)
                //    {
                //        return TileState.PlayerCity;
                //    }
                //}
            }
        }
        foreach (MapItem mapItem in transform.GetComponentsInChildren<MapItem>())
        {
            // verify if not null
            if (mapItem)
            {
                Vector2Int itemTilePosition = GetTilePosition(mapItem.transform);
                if (itemTilePosition == tilePosition)
                {
                    return mapItem.gameObject;
                }
            }
        }
        // Everything else is just terrain without hero, city or treasure on it.
        return null;
    }

    GameObject GetObjectOnTile(NesScripts.Controls.PathFind.Point pathPoint)
    {
        Vector2Int tilePosition = new Vector2Int
        {
            x = pathPoint.x,
            y = pathPoint.y
        };
        return GetObjectOnTile(tilePosition);
    }

    Color GetTileHighlightColor(GameObject gameObjectOnTile, bool notEnoughMovePoints)
    {
        // highligh the last tile depending on what is under cursor
        // the might be another hero or enemy party or treasure chest or ally party
        // highlight based on the occupation type
        if (gameObjectOnTile)
        {
            MapHero mapHero = gameObjectOnTile.GetComponent<MapHero>();
            MapCity mapCity = gameObjectOnTile.GetComponent<MapCity>();
            MapItem mapItem = gameObjectOnTile.GetComponent<MapItem>();
            Relationships.State relationships;
            if (mapHero)
            {
                // check relationships with active player
                relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapHero.LHeroParty.Faction);
            }
            else if (mapCity)
            {
                // check relationships with active player
                relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapCity.LCity.CityFaction);
            }
            else if (mapItem)
            {
                // check relationships with active player
                relationships = Relationships.State.Neutral;
            }
            else
            {
                Debug.LogError("Unknown object on map " + gameObject.name);
                return Color.magenta;
            }
            switch (relationships)
            {
                case Relationships.State.SameFaction:
                    if (notEnoughMovePoints)
                        return sameFactionRelationColorNotEnoughMovePoints;
                    else
                        return sameFactionRelationColor;
                case Relationships.State.Allies:
                    if (notEnoughMovePoints)
                        return alliedFactionRelationColorNotEnoughMovePoints;
                    else
                        return alliedFactionRelationColor; // Color.cyan;
                case Relationships.State.Neutral:
                    if (notEnoughMovePoints)
                        return neutralFactionRelationColorNotEnoughMovePoints;
                    else
                        return neutralFactionRelationColor;
                case Relationships.State.AtWar:
                    if (notEnoughMovePoints)
                        return atWarFactionRelationColorNotEnoughMovePoints;
                    else
                        return atWarFactionRelationColor;
                default:
                    Debug.LogError("Unknown relationships " + relationships.ToString());
                    return unknownRelationshipsUnitColor; // Color.magenta;
            }
        }
        else
        {
            // no object on tile, just terrain
            if (notEnoughMovePoints)
                return defaultTileHighlighterNotEnoughMovePoints;
            else
                return defaultTileHighlighterColor;
        }
        //switch (tileState)
        //{
        //    case TileState.None:
        //        // nothing to do
        //        return Color.green;
        //    case TileState.SelectedParty:
        //        // this should not be possible, because move path in this case is 0
        //        Debug.LogError("Not possible condition");
        //        return Color.magenta;
        //    case TileState.PlayerParty:
        //        // highlight other party
        //        // highlight blue
        //        return Color.blue;
        //    case TileState.PlayerCity:
        //        // highlight city
        //        // highlight yellow
        //        return Color.yellow;
        //    case TileState.EnemyParty:
        //    case TileState.Protected:
        //        // highlight red
        //        return Color.red;
        //    default:
        //        Debug.LogError("Unknown tile state " + tileState.ToString());
        //        return Color.magenta;
        //}
    }

    public void HighlightMovePath(bool doHighlight)
    {
        Color highlightColor = Color.green;
        // todo: it is better to make them transparant, then instantiate new and destroy each time
        if (movePath != null && movePath.Count > 0)
        {
            // Set lastPathTile variable for later user
            //if (doHighlight)
            //{
            //    lastPathTile = movePath[movePath.Count - 1];
            //    // set lastTileState value it will be used here and in other functions too
            //    lastTileState = GetTileOccupationState(lastPathTile);
            //    // Debug.Log(pathPoint.x.ToString() + ":" + pathPoint.y.ToString() + " " + lastTileState.ToString());
            //}
            // Highlight all tiles
            // Initialize enemy of a way as false, like we do not encounter enemy on our move path
            bool enemyOnWay = false;
            Color tileHighlighColor;
            // init variable for move path highligh
            PartyUnit partyLeader = null;
            int movePointsLeft = 0;
            // verify if selectedMapHero is not null
            if (selectedMapHero != null)
            {
                // Get party leader
                partyLeader = selectedMapHero.LHeroParty.GetPartyLeader();
                // Init move points left counter
                movePointsLeft = partyLeader.MovePointsCurrent;
            }
            // Init variables
            GameObject gameObjectOnTile = null;
            foreach (NesScripts.Controls.PathFind.Point pathPoint in movePath)
            {
                // output path to debug
                // Debug.Log("Path point is [" + pathPoint.x + "]:[" + pathPoint.y + "]");
                tileHighlighters[pathPoint.x, pathPoint.y].SetActive(doHighlight);
                // clear turn info text (because is not reset automatically and may be still present)
                tileHighlighters[pathPoint.x, pathPoint.y].transform.Find("TurnsInfo").GetComponent<Text>().text = "";
                // Verify if enemy on way was not triggered on previous pathPoints
                if (!enemyOnWay)
                {
                    // get tile highlight color based on a tile state (what is in or near tile)
                    // reset collor to correct highlight color
                    // because it might be changed by previous highlight operations to something else
                    // if for example in the past there was enemy standing and we were highliting is with red color
                    // Check if there is enemy on way or tile is protected by enemy
                    gameObjectOnTile = GetObjectOnTile(pathPoint);
                    if (gameObjectOnTile)
                    {
                        MapHero mapHero = gameObjectOnTile.GetComponent<MapHero>();
                        MapCity mapCity = gameObjectOnTile.GetComponent<MapCity>();
                        MapItem mapItem = gameObjectOnTile.GetComponent<MapItem>();
                        if (mapHero)
                        {
                            // check relationships with active player
                            Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapHero.LHeroParty.Faction);
                            switch (relationships)
                            {
                                case Relationships.State.SameFaction:
                                case Relationships.State.Allies:
                                case Relationships.State.Neutral:
                                    break;
                                case Relationships.State.AtWar:
                                    enemyOnWay = true;
                                    break;
                                default:
                                    Debug.LogError("Unknown relationships " + relationships.ToString());
                                    break;
                            }
                        }
                        else if (mapCity)
                        {
                        }
                        else if (mapItem)
                        {

                        }
                        else
                        {
                            Debug.LogError("Unknown object on map " + gameObject.name);
                        }
                    }
                    else
                    {
                        // no object on tile, just terrain
                        // Verify if tile is not protected by enemy
                        enemyOnWay = IsTileProtected(pathPoint);
                    }
                }
                // init not enough move points modifier
                bool notEnoughMovePoints = false;
                // verify if selectedMapHero is not null and if we has reached one day move points limit
                if ((selectedMapHero != null) && (movePointsLeft <= 0))
                {
                    // Activate not enough move points modifier
                    notEnoughMovePoints = true;
                    // get the rest of division
                    float restOfDivision = movePointsLeft % partyLeader.GetEffectiveMaxMovePoints();
                    Debug.Log("Rest of division is " + restOfDivision);
                    // verify if we has reached next day limit
                    if (restOfDivision == 0)
                    {
                        // Get number of days needed to reach this path point
                        int numberOfDaysToReachThisPathPoint = Math.Abs(movePointsLeft / partyLeader.GetEffectiveMaxMovePoints()) + 1;
                        Debug.Log("numberOfDaysToReachThisPathPoint = " + numberOfDaysToReachThisPathPoint);
                        // add day indicator to the move path highter
                        tileHighlighters[pathPoint.x, pathPoint.y].transform.Find("TurnsInfo").GetComponent<Text>().text = numberOfDaysToReachThisPathPoint.ToString();
                    }
                }
                // verify if enemy on way
                if (enemyOnWay)
                {
                    // set tile highlight color as if there was an enemy
                    tileHighlighColor = enemyOnWayColor; // Color.red;
                    if (notEnoughMovePoints)
                        tileHighlighColor = enemyOnWayColorNotEnoughMovePoints;
                }
                else
                {
                    tileHighlighColor = GetTileHighlightColor(gameObjectOnTile, notEnoughMovePoints);
                }
                // Highlight tile
                tileHighlighters[pathPoint.x, pathPoint.y].GetComponentInChildren<Text>().color = tileHighlighColor;
                // reduce number of move points left
                movePointsLeft -= 1;
            }
        }
    }

    bool IsTileProtected(Vector2Int checkedTilePositoin)
    {
        foreach (MapHero mapHero in transform.GetComponentsInChildren<MapHero>())
        {
            // verify if not null
            if (mapHero)
            {
                // check relationships with active player
                Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapHero.LHeroParty.Faction);
                switch (relationships)
                {
                    case Relationships.State.SameFaction:
                    case Relationships.State.Allies:
                    case Relationships.State.Neutral:
                        break;
                    case Relationships.State.AtWar:
                        Vector2Int partyTilePosition = GetTilePosition(mapHero.transform);
                        // verify if enemy party located in nearby tiles
                        if (TileIsProtectedByEnemyParty(partyTilePosition, checkedTilePositoin))
                        {
                            // Debug.Log("Protected");
                            return true;
                            //enemyOnWay = true;
                        }
                        break;
                    default:
                        Debug.LogError("Unknown relationships " + relationships.ToString());
                        break;
                }
            }
            //// Verify if we already found enemyOnWay condition and can exit for loop.
            //if (enemyOnWay)
            //{
            //    break;
            //}
        }
        return false;
    }

    bool IsTileProtected(NesScripts.Controls.PathFind.Point pathPoint)
    {
        Vector2Int checkedTilePositoin = new Vector2Int
        {
            x = pathPoint.x,
            y = pathPoint.y
        };
        return IsTileProtected(checkedTilePositoin);
    }

    void SetMovePath(Vector3 from, Vector3 to)
    {
        SetMovePath(GetTileByPosition(from), GetTileByPosition(to));
    }

    void SetMovePath(Vector2Int from, Vector2Int to)
    {
        Debug.Log("SetMovePath");
        // create source and target points
        NesScripts.Controls.PathFind.Point _from = new NesScripts.Controls.PathFind.Point(from.x, from.y);
        Debug.Log("From [" + _from.x + "]:[" + _from.y + "]");
        NesScripts.Controls.PathFind.Point _to = new NesScripts.Controls.PathFind.Point(to.x, to.y);
        Debug.Log("To [" + _to.x + "]:[" + _to.y + "]");
        // Debug.Log("Length [" + (grid.nodes.GetLength(0) - 1).ToString() + "]:[" + (grid.nodes.GetLength(1) - 1).ToString() + "]");
        // get path
        // path will either be a list of Points (x, y), or an empty list if no path is found.
        // verify if mouse is not over screen
        if ((_to.x > grid.nodes.GetLength(0) - 1)
            || (_to.y > grid.nodes.GetLength(1) - 1)
            || (_to.x < 0)
            || (_to.y < 0))
        {
            // nothing to highlight, mouse is over the screen
        }
        else
        {
            // before setting move path make the last tile passable, if it is tile with city on it
            if (GetObjectOnTile(_to))
            {
                if (GetObjectOnTile(_to).GetComponent<MapCity>())
                {
                    Debug.Log("Make destination tile with city passable");
                    // some object is present
                    // adjust grid to make this tile passable (highlightable)
                    // this is needed if user conquered this city and need to move to it
                    tilesmap[_to.x, _to.y] = true;
                    // Upgrade grid
                    grid.UpdateGrid(tilesmap);
                }
            }
            movePath = NesScripts.Controls.PathFind.Pathfinding.FindPath(grid, _from, _to);
            // for Manhattan distance
            // List<NesScripts.Controls.PathFind.Point> path = NesScripts.Controls.PathFind.Pathfinding.FindPath(grid, _from, _to, NesScripts.Controls.Pathfinding.DistanceType.Manhattan);
        }
    }

    void FindAndHighlightPath()
    {
        // First remove highlight from previous path
        HighlightMovePath(false);
        // Debug.Log("FindAndHighlightPath");
        SetMovePath(new Vector2Int(GetHeroTilePosition().x, GetHeroTilePosition().y), new Vector2Int(GetTileHighlighterPosition().x, GetTileHighlighterPosition().y));
        // highlight new path
        HighlightMovePath(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // save mouse position, it may be required for OnBeginDrag
        mouseOnDownStartPosition = Input.mousePosition;
        // disable tile highliter
        // tileHighlighter.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // verify if user clicked left or right mouse button
        if (Input.GetMouseButton(0))
        {
            // on left mouse down
            // enter Drag mode
            SetMode(Mode.Drag);
            // prepare for drag
            //startPosition = transform.position;
            // startParent = transform.parent;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            // get mouse start possition and calculate which offset to apply to final transform
            // this is because I would like to avoid that map center is also centered under mouse
            // this create strange map jumps
            // mouseOnDragStartPosition = Input.mousePosition;
            // apply corrections depeding on location from the canvas center
            // this actually depends one the current position of the map
            Vector3 mapNewPosiiton = Camera.main.WorldToScreenPoint(transform.position);
            // keep z position otherwise map will disappear
            mapPosiiton = new Vector3(mapNewPosiiton.x, mapNewPosiiton.y, 0);
            // xCorrectionOnDragStart = (Screen.width / 2) - mouseOnDragStartPosition.x;
            // yCorrectionOnDragStart = (Screen.height / 2) - mouseOnDragStartPosition.y;
            xCorrectionOnDragStart = mapPosiiton.x - mouseOnDownStartPosition.x;
            yCorrectionOnDragStart = mapPosiiton.y - mouseOnDownStartPosition.y;
            // this corrections should also be applied to x and y min and max
            xMin = xMinDef - xCorrectionOnDragStart;
            xMax = xMaxDef - xCorrectionOnDragStart;
            yMin = yMinDef - yCorrectionOnDragStart;
            yMax = yMaxDef - yCorrectionOnDragStart;
        }
        else if (Input.GetMouseButton(1))
        {
            // on right mouse down
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // verify if user clicked left or right mouse button
        if (Input.GetMouseButton(0))
        {
            // on left mouse held down
            mousePosition = Input.mousePosition;
            mapPosiiton = Camera.main.WorldToScreenPoint(transform.position);
            // make sure that new position is within the borders of the map
            float newPositionX = mousePosition.x;
            if (mousePosition.x <= xMin)
            {
                newPositionX = xMin;
            }
            else if (mousePosition.x >= xMax)
            {
                newPositionX = xMax;
            }
            float newPositionY = mousePosition.y;
            if (mousePosition.y <= yMin)
            {
                newPositionY = yMin;
            }
            else if (mousePosition.y >= yMax)
            {
                newPositionY = yMax;
            }
            // for unknown reason z is set to -30000 on drag, that is why I use original value
            Vector3 newPosition = new Vector3(newPositionX + xCorrectionOnDragStart, newPositionY + yCorrectionOnDragStart, 0);
            // make drag to allign with tile size
            float x = Mathf.RoundToInt(newPosition.x / tileSize) * tileSize;
            float y = Mathf.RoundToInt(newPosition.y / tileSize) * tileSize;
            newPosition = new Vector3(x, y, newPosition.z);
            Vector3 newPositionTransformed = Camera.main.ScreenToWorldPoint(newPosition);
            transform.position = new Vector3(newPositionTransformed.x, newPositionTransformed.y, 0);
            //    Debug.Log("transform " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
        }
        else if (Input.GetMouseButton(1))
        {
            // on right mouse held down
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // verify if user clicked left or right mouse button
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse up
            // enter back to Browse mode
            SetMode(Mode.Browse);
            // set tile highighter under mouse
            SetTileHighlighterToMousePoistion();
            // allow map to block raycasts
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse up
        }
    }

    void SaveCitiesNamesToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (transform.parent.Find("MapMenu/Options/ToggleCitiesNames").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.options.mapUIOpt.toggleCitiesNames = 1;
        }
        else
        {
            // set option
            GameOptions.options.mapUIOpt.toggleCitiesNames = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowCityNames", GameOptions.options.mapUIOpt.toggleCitiesNames); // 0 - disable, 1 - enable
    }

    void SaveHeroesNamesToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (transform.parent.Find("MapMenu/Options/ToggleHeroesNames").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.options.mapUIOpt.toggleHeroesNames = 1;
        }
        else
        {
            // set option
            GameOptions.options.mapUIOpt.toggleHeroesNames = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowHeroesNames", GameOptions.options.mapUIOpt.toggleHeroesNames); // 0 - disable, 1 - enable
    }

    void SavePlayerIncomeToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (transform.parent.Find("MapMenu/Options/TogglePlayerIncome").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.options.mapUIOpt.togglePlayerIncome = 1;
        }
        else
        {
            // set option
            GameOptions.options.mapUIOpt.togglePlayerIncome = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowPlayerIncome", GameOptions.options.mapUIOpt.togglePlayerIncome); // 0 - disable, 1 - enable
    }

    void SaveMapUIOptions()
    {
        Debug.Log("Save Map UI Options");
        SaveCitiesNamesToggleOptions();
        SaveHeroesNamesToggleOptions();
        SavePlayerIncomeToggleOptions();
    }

    void LoadCitiesNamesToggleOptions()
    {
        // Get map UI options
        GameOptions.options.mapUIOpt.toggleCitiesNames = PlayerPrefs.GetInt("MapUIShowCityNames", 0); // default 0 - disable "always show city names" toggle
        // Get City names toggle
        TextToggle textToggle = transform.parent.Find("MapMenu/Options/ToggleCitiesNames").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.options.mapUIOpt.toggleCitiesNames == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide cities names
            SetCitiesNamesVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // always show city names
            SetCitiesNamesVisible(true);
        }
    }

    void LoadHeroesNamesToggleOptions()
    {
        // Get map UI options
        GameOptions.options.mapUIOpt.toggleHeroesNames = PlayerPrefs.GetInt("MapUIShowHeroesNames", 0); // default 0 - disable "always show heroes names" toggle
        // Get toggle
        TextToggle textToggle = transform.parent.Find("MapMenu/Options/ToggleHeroesNames").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.options.mapUIOpt.toggleHeroesNames == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide cities names
            SetHeroesNamesVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // always show city names
            SetHeroesNamesVisible(true);
        }
    }

    void LoadPlayerIncomeToggleOptions()
    {
        // Get map UI options
        GameOptions.options.mapUIOpt.togglePlayerIncome = PlayerPrefs.GetInt("MapUIShowPlayerIncome", 0); // default 0 - disable "always show heroes names" toggle
        // Get toggle
        TextToggle textToggle = transform.parent.Find("MapMenu/Options/TogglePlayerIncome").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.options.mapUIOpt.togglePlayerIncome == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide cities names
            SetPlayerIncomeVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // always show city names
            SetPlayerIncomeVisible(true);
        }
    }

    void LoadMapUIOptions()
    {
        Debug.Log("Load Map UI Options");
        LoadCitiesNamesToggleOptions();
        LoadHeroesNamesToggleOptions();
        LoadPlayerIncomeToggleOptions();
    }

    void OnEnable()
    {
        LoadMapUIOptions();
        GetComponentInChildren<TileHighlighter>(true).gameObject.SetActive(true);
    }

    void OnDisable()
    {
        SaveMapUIOptions();
        SetMode(MapManager.Mode.Browse);
    }

    float GetRemainingDistance(NesScripts.Controls.PathFind.Point pathPoint)
    {
        float result = 0;
        Vector2 dst = GetDestination(pathPoint);
        Vector2 src = selectedMapHero.transform.position;
        result = Vector2.Distance(src, dst);
        // Debug.Log("Remaining distance is [" + result.ToString() + "]");
        return result;
    }

    Vector2 GetDestination(NesScripts.Controls.PathFind.Point pathPoint)
    {
        // + tileSize/ is to place it in the center of the tile
        return new Vector2
        {
            x = (float)pathPoint.x * (float)tileSize + (float)tileSize / 2f,
            y = (float)pathPoint.y * (float)tileSize + (float)tileSize / 2f
        };
    }

    public void MapHeroMoveToAndEnterCity(MapHero mapHero, MapCity mapCity)
    {
        // set selectedHero - it will be later used by EnterCityAfterMove function
        selectedMapHero = mapHero;
        // set move path
        SetMovePath(mapHero.transform.position, mapCity.transform.position);
        Debug.Log("StartCoroutine(Move())");
        StartCoroutine(Move());
        // Execute default enter city after move sequence
        // . - this is not needed below, because it will be executed automatically
        //EnterCityAfterMove(mapCity);
    }

    void EnterCityAfterMove(MapCity mapCity)
    {
        Debug.Log("MapManager: EnterCityAfterMove");
        //// Get City
        //MapCity mapCity = GetCityByTile(new Vector2Int(lastPathTile.x, lastPathTile.y));
        // Link hero on the map to city on the map
        mapCity.LMapHero = selectedMapHero;
        // And do the opposite 
        // Link city on the map to hero on the map
        selectedMapHero.lMapCity = mapCity;
        // Move hero UI to City
        selectedMapHero.LHeroParty.transform.SetParent(mapCity.LCity.transform);
        // Enter city edit mode
        queue.Run(mapCity.EnterCityEditMode());
        // Trigger on hero entering city
        // ..
        // reset map state and selections, because hero can be removed while in city
        SetSelection(Selection.None);
    }

    //MapCity GetCityByTile(Vector2Int tilePosition)
    //{
    //    MapCity mapCity = null;
    //    foreach (MapCity city in transform.GetComponentsInChildren<MapCity>())
    //    {
    //        // verify if not null
    //        if (city)
    //        {
    //            if (GetTilePosition(city.transform) == tilePosition)
    //            {
    //                return city;
    //            }
    //        }
    //    }
    //    return mapCity;
    //}

    //MapHero GetEnemyByTile(Vector2Int tilePosition)
    //{
    //    foreach (MapHero partyOnMap in transform.GetComponentsInChildren<MapHero>())
    //    {
    //        // verify if not null
    //        if (partyOnMap)
    //        {
    //            if (PartyIsEnemy(partyOnMap))
    //            {
    //                if (GetTilePosition(partyOnMap.transform) == tilePosition)
    //                {
    //                    return partyOnMap.GetComponent<MapHero>();
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}

    //MapHero GetEnemyByProtectedTile(Vector2Int tilePosition)
    //{
    //    Vector2Int[] surroundingTilesPositions = GetAllSurroundingTilesPositions(tilePosition);
    //    foreach (MapHero partyOnMap in transform.GetComponentsInChildren<MapHero>())
    //    {
    //        // verify if not null
    //        if (partyOnMap)
    //        {
    //            // verify if party is enemy
    //            if (PartyIsEnemy(partyOnMap))
    //            {
    //                // verify if enemy is located on one of the surrounding tiles
    //                Vector2Int enemyTilePos = GetTilePosition(partyOnMap.transform);
    //                foreach (Vector2Int tilePos in surroundingTilesPositions)
    //                {
    //                    if (enemyTilePos == tilePos)
    //                    {
    //                        return partyOnMap.GetComponent<MapHero>();
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}

    //void EnterBattleOnMove(NesScripts.Controls.PathFind.Point pathPoint)
    //{
    //    Debug.Log("Enter battle");
    //    MapHero enemyOnMap = null;
    //    TileState tileState = GetObjectOnTile(pathPoint);
    //    switch (tileState)
    //    {
    //        case TileState.EnemyParty:
    //            // Get Enemy party
    //            enemyOnMap = GetEnemyByTile(new Vector2Int(pathPoint.x, pathPoint.y));
    //            break;
    //        case TileState.Protected:
    //            // get random enemy which is protecting this tile
    //            enemyOnMap = GetEnemyByProtectedTile(new Vector2Int(pathPoint.x, pathPoint.y));
    //            break;
    //        default:
    //            Debug.LogError("Unhandled condition " + tileState.ToString());
    //            break;
    //    }
    //    // initialize battle
    //    if (enemyOnMap)
    //    {
    //        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).EnterBattle(selectedHero, enemyOnMap);
    //        // Remove hero Selection
    //        SetSelection(Selection.None);
    //    }
    //    else
    //    {
    //        Debug.LogError("Enemy not found");
    //    }
    //}

    //void EndMoveTransition()
    //{
    //    // transition to required state based on the type of the last occupied cell
    //    switch (lastTileState)
    //    {
    //        case TileState.None:
    //            // Nothing special to do here
    //            break;
    //        case TileState.PlayerCity:
    //            break;
    //        case TileState.EnemyParty:
    //        case TileState.Protected:
    //            Debug.LogError("This state should not be here, but we should enter this state during move, state: " + lastTileState.ToString());
    //            break;
    //        case TileState.SelectedParty:
    //        case TileState.PlayerParty:
    //            // this should not be possible, because move path in this case is 0
    //            Debug.LogError("Not possible condition " + lastTileState.ToString());
    //            break;
    //        default:
    //            Debug.LogError("Unknown tile state " + lastTileState.ToString());
    //            break;
    //    }
    //}

    IEnumerator EnterBattleCommonStart(MapHero mapHero)
    {
        Debug.Log("EnterBattleCommonStart1");
        SetMode(Mode.Animation);
        // Block mouse input
        // this is not needed here because it is still blocked my Move() function
        // show animation before immediately entering battle
        yield return new WaitForSeconds(mapHero.GetComponent<MapObject>().LabelDimTimeout + 0.1f);
        Debug.Log("EnterBattleCommonStart2");
    }

    IEnumerator EnterBattleCommonStart(MapCity mapCity)
    {
        Debug.Log("EnterBattleCommonStart1");
        SetMode(Mode.Animation);
        // Block mouse input
        // this is not needed here because it is still blocked my Move() function
        // show animation before immediately entering battle
        yield return new WaitForSeconds(mapCity.GetComponent<MapObject>().LabelDimTimeout + 0.1f);
        Debug.Log("EnterBattleCommonStart2");
    }

    IEnumerator EnterBattleCommonEnd()
    {
        Debug.Log("EnterBattleCommonEnd");
        SetSelection(Selection.None);
        SetMode(Mode.Browse);
        //// Deactivate map screen
        //transform.root.Find("MapScreen").gameObject.SetActive(false);
        // unblock input
        InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        inputBlocker.SetActive(false);
        yield return null;
    }

    IEnumerator EnterBattleStep(MapHero mapHero)
    {
        Debug.Log("EnterBattleStep");
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).EnterBattle(selectedMapHero, mapHero);
        yield return null;
    }

    IEnumerator EnterBattleStep(MapCity mapCity)
    {
        Debug.Log("EnterBattleStep");
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).EnterBattle(selectedMapHero, mapCity);
        yield return null;
    }

    void EnterBattle(MapHero mapHero)
    {
        queue.Run(EnterBattleCommonStart(mapHero));
        queue.Run(EnterBattleStep(mapHero));
        queue.Run(EnterBattleCommonEnd());
    }

    void EnterBattle(MapCity mapCity)
    {
        Debug.Log("Enter City battle");
        queue.Run(EnterBattleCommonStart(mapCity));
        queue.Run(EnterBattleStep(mapCity));
        queue.Run(EnterBattleCommonEnd());
    }


    IEnumerator PickupItem(MapItem mapItem)
    {
        Debug.Log("Pick up item");
        // init string for the message box
        string message = "<b>You have found</b>:\r\n";
        // Loop through each item in the chest
        foreach (InventoryItem inventoryItem in mapItem.LInventoryItems)
        {
            // change parent to hero party
            inventoryItem.transform.SetParent(selectedMapHero.LHeroParty.transform);
            // get name for the message box
            message += "\r\n" + inventoryItem.ItemName;
        }
        // Destroy chest
        Destroy(mapItem.gameObject);
        // exit animation mode and enter browse mode
        SetMode(Mode.Browse);
        yield return new WaitForSeconds(0.5f);
        // Unblock mouse input
        transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>().SetActive(false);
        // Display message about what item(s) were found
        transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(message);
        // Exit coroutine
        yield return null;
    }

    IEnumerator Move()
    {
        Debug.Log("MapManager: Move");
        // Verify if hero was in city
        if (selectedMapHero.lMapCity)
        {
            // Unlink city from hero and hero from city if they were linked before
            MapCity linkedCity = selectedMapHero.lMapCity;
            linkedCity.LMapHero = null;
            selectedMapHero.lMapCity = null;
            // Get current party city
            HeroParty heroParty = selectedMapHero.LHeroParty;
            // Trigger on hero leaving city
            // ..
            // Move party from city to map
            // Transform partiesOnMap = transform.root.Find("PartiesOnMap");
            heroParty.transform.SetParent(lMap.transform);
            // Update hero party place
            //heroParty.SetPlace(HeroParty.PartyPlace.Map);
        }
        // Verify if hero party was on hold
        if (selectedMapHero.LHeroParty.HoldPosition == true)
        {
            // trigger break hold event
            transform.root.Find("MapScreen/MapMenu").GetComponentInChildren<MapFocusPanel>().BreakHoldPosition(selectedMapHero);
        }
        // Move
        float deltaTime;
        float previousTime = Time.time;
        // Get party leader
        PartyUnit partyLeader = selectedMapHero.LHeroParty.GetPartyLeader();
        // verify if move path is not null and has at least one move point and hero has move points
        if ((movePath != null) && (movePath.Count >= 1) && (partyLeader.MovePointsCurrent >= 1))
        {
            Debug.Log("Move path is not null");
            // exit exit browse mode and enter animation mode
            SetMode(Mode.Animation);
            // Block mouse input
            InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
            inputBlocker.SetActive(true);
            // initialize break move condition
            bool breakMove = false;
            MapCity enterCity = null;
            MapHero protectedTileEnemy = null;
            Faction selectedHeroFaction = selectedMapHero.LHeroParty.Faction;
            // Get map focus panel
            MapFocusPanel mapFocusPanel = transform.root.Find("MapScreen/MapMenu").GetComponentInChildren<MapFocusPanel>();
            // loop through path points
            Debug.Log("Move path count: " + movePath.Count.ToString());
            for (int i = 0; i < (movePath.Count) && (partyLeader.MovePointsCurrent >= 1) ; i++)
            {
                // consume move points from hero
                partyLeader.MovePointsCurrent -= 1;
                // update focus panel info
                mapFocusPanel.UpdateMovePointsInfo();
                // .. change highlight for move points where user do not have enough move points
                // get path point
                var pathPoint = movePath[i];
                // act based on what is located on this (next) tile before we actually move into it
                GameObject nextGameObjectOnPath = GetObjectOnTile(pathPoint);
                if (nextGameObjectOnPath)
                {
                    //Debug.Log("04");
                    MapHero mapHero = nextGameObjectOnPath.GetComponent<MapHero>();
                    MapCity mapCity = nextGameObjectOnPath.GetComponent<MapCity>();
                    MapItem mapItem = nextGameObjectOnPath.GetComponent<MapItem>();
                    if (mapHero)
                    {
                        // check relationships with moving party faction
                        //Relationships.State relationships = Relationships.Instance.GetRelationships(player.Faction, mapHero.LinkedPartyTr.GetComponent<HeroParty>().Faction);
                        Relationships.State relationships = Relationships.Instance.GetRelationships(selectedHeroFaction, mapHero.LHeroParty.Faction);
                        switch (relationships)
                        {
                            case Relationships.State.SameFaction:
                            case Relationships.State.Allies:
                                breakMove = true;
                                break;
                            case Relationships.State.Neutral:
                            case Relationships.State.AtWar:
                                // enter battle
                                EnterBattle(mapHero);
                                breakMove = true;
                                break;
                            default:
                                Debug.LogError("Unknown relationships " + relationships.ToString());
                                break;
                        }
                    }
                    else if (mapCity)
                    {
                        Debug.Log("Move(): it is map city");
                        // check relationships with moving party faction
                        //Relationships.State relationships = Relationships.Instance.GetRelationships(player.Faction, mapCity.LinkedCityTr.GetComponent<City>().Faction);
                        Relationships.State relationships = Relationships.Instance.GetRelationships(selectedHeroFaction, mapCity.LCity.CityFaction);
                        switch (relationships)
                        {
                            case Relationships.State.SameFaction:
                                // continue move untill we are on the same tile
                                enterCity = mapCity;
                                break;
                            case Relationships.State.Allies:
                                breakMove = true;
                                break;
                            case Relationships.State.Neutral:
                            case Relationships.State.AtWar:
                                // enter battle
                                EnterBattle(mapCity);
                                breakMove = true;
                                break;
                            default:
                                Debug.LogError("Unknown relationships " + relationships.ToString());
                                break;
                        }
                    }
                    else if (mapItem)
                    {
                        queue.Run(PickupItem(mapItem));
                        breakMove = true;
                    }
                    else
                    {
                        Debug.LogError("Unknown object on map " + gameObject.name);
                    }
                }
                else
                {
                    // no object on tile, just terrain
                    // Verify if tile is not protected by enemy
                    Vector2Int checkedTilePositoin = new Vector2Int
                    {
                        x = pathPoint.x,
                        y = pathPoint.y
                    };
                    foreach (MapHero mapHero in transform.GetComponentsInChildren<MapHero>())
                    {
                        // verify if not null
                        if (mapHero)
                        {
                            // check relationships with moving party faction
                            //Relationships.State relationships = Relationships.Instance.GetRelationships(player.Faction, mapHero.LinkedPartyTr.GetComponent<HeroParty>().Faction);
                            Relationships.State relationships = Relationships.Instance.GetRelationships(selectedHeroFaction, mapHero.LHeroParty.Faction);
                            switch (relationships)
                            {
                                case Relationships.State.SameFaction:
                                case Relationships.State.Allies:
                                case Relationships.State.Neutral:
                                    break;
                                case Relationships.State.AtWar:
                                    Vector2Int partyTilePosition = GetTilePosition(mapHero.transform);
                                    // verify if enemy party located in nearby tiles
                                    if (TileIsProtectedByEnemyParty(partyTilePosition, checkedTilePositoin))
                                    {
                                        // Debug.Log("Protected");
                                        // enter battle
                                        protectedTileEnemy = mapHero;
                                    }
                                    break;
                                default:
                                    Debug.LogError("Unknown relationships " + relationships.ToString());
                                    break;
                            }
                        }
                        // Verify if we already found protected tile and can exit inner for loop.
                        if (breakMove)
                        {
                            break;
                        }
                    }
                }
                // Verify if we can break move
                // this might happen if next tile is city, other party or treasure
                if (breakMove)
                {
                    break;
                }
                // execute move animation
                Vector2 dst = GetDestination(pathPoint);
                while (GetRemainingDistance(pathPoint) > 0.5f)
                {
                    // Debug.Log("Path point is [" + pathPoint.x + "]:[" + pathPoint.y + "]");
                    // move hero
                    deltaTime = Time.time - previousTime;
                    selectedMapHero.transform.position = Vector2.MoveTowards(selectedMapHero.transform.position, dst, deltaTime * heroMoveSpeed);
                    // wait until next move
                    previousTime = Time.time;
                    yield return new WaitForSeconds(heroMoveSpeedDelay);
                }
                if (enterCity)
                {
                    EnterCityAfterMove(enterCity);
                }
                if (protectedTileEnemy)
                {
                    EnterBattle(protectedTileEnemy);
                    break;
                }
                //// get tile state on current path point
                //TileState tileState = GetObjectOnTile(pathPoint);
                //// verify if state requires hero to enter battle
                //if ((TileState.EnemyParty == tileState) || (TileState.Protected == tileState))
                //{
                //    // enter battle
                //    //EnterBattleOnMove(pathPoint);
                //    transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).EnterBattle(selectedHero, enemyOnMap);
                //    // Remove hero Selection
                //    SetSelection(Selection.None);
                //    // break move
                //    break;
                //}
                //// verify if state requires hero to stop
                //// - 2 heroes cannot be on the same tile
                //// - hero and treasure chest cannot be on the same tile
                //if (TileState.PlayerParty)
                //{

                //}
                //// Verify if we have reached last tile in path here
                //if (movePath.Count - 1 == i)
                //{
                //    // We finished our move
                //    EndMoveTransition();
                //}
            }
            // verify if we reached this code without breaking move or entering city or protected enemy tile
            if (!(breakMove || enterCity || protectedTileEnemy))
            {
                // this was just move on the map without any additional challanges on the way
                // exit animation mode and enter browse mode
                SetMode(Mode.Browse);
                // Unblock mouse input
                inputBlocker.SetActive(false);
            }
        }
        // Remove path highlight
        HighlightMovePath(false);
    }

    public void SetMode(Mode value)
    {
        mode = value;
        switch (mode)
        {
            case Mode.Browse:
                tileHighlighter.EnterBrowseMode();
                break;
            case Mode.Animation:
                // change tile highlighter mode
                tileHighlighter.EnterAnimationMode();
                break;
            case Mode.Drag:
                // update tileHighlighter
                tileHighlighter.EnterDragMode();
                break;
            default:
                Debug.LogError("Unknown mode " + mode.ToString());
                break;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
            ActOnClick(gameObject, pointerEventData);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            //Debug.Log("Right mouse click on map");
            // Remove selection
            SetSelection(Selection.None);
        }
    }

    //public void OnPointerEnterChildObject(GameObject childGameObject, PointerEventData eventData)
    //{

    //}
    public void OnPointerEnterChildObject(GameObject childGameObject, PointerEventData eventData)
    {
        //// set default tile highlighter color
        //tileHighlighterColor = Color.yellow;
        // predefine variables
        MapHero mapHero = childGameObject.GetComponent<MapHero>();
        MapCity mapCity = childGameObject.GetComponent<MapCity>();
        MapItem mapItem = childGameObject.GetComponent<MapItem>();
        MapObjectLabel mapHeroViaLabel = null;
        MapObjectLabel mapCityViaLabel = null;
        MapObjectLabel label = childGameObject.GetComponent<MapObjectLabel>();
        if (label)
        {
            // set hero and city variables
            mapHero = label.transform.parent.GetComponent<MapHero>();
            mapCity = label.transform.parent.GetComponent<MapCity>();
            // find out on which label we clicked
            if (label.transform.parent.GetComponent<MapHero>())
            {
                mapHeroViaLabel = label;
            }
            if (label.transform.parent.GetComponent<MapCity>())
            {
                mapCityViaLabel = label;
            }
        }

        switch (mode)
        {
            case Mode.Browse:
                // act based on current selection
                switch (selection)
                {
                    case Selection.None:
                        // act based on the object over which mouse is now
                        if (mapHero)
                        {
                            // verify if this is player's hero
                            HeroParty heroParty = mapHero.LHeroParty;
                            if (TurnsManager.Instance.GetActivePlayer().Faction == heroParty.Faction)
                            {
                                // highlighted hero belongs to player
                                // change cursor to selection hand
                                transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                                //tileHighlighterColor = Color.blue;
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                // do nothing
                                // user cannot select and control units or cities which he does not own
                            }
                        }
                        if (mapCity)
                        {
                            // verify if this is player's city
                            City city = mapCity.LCity;
                            if (TurnsManager.Instance.GetActivePlayer().Faction == city.CityFaction)
                            {
                                // highlighted city belongs to player
                                // change cursor to selection hand
                                transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                                //tileHighlighterColor = Color.blue;
                            }
                            else
                            {
                                // highlighted city is from other faction
                                // do nothing
                                // user cannot select and control units or cities which he does not own
                            }
                        }
                        if (mapItem)
                        {
                            // nothing to do here
                        }
                        // at this stage we assume that pointer is over map terrain
                        // nothing to do, just keep normal pointer, which was set after exit highlighting other objects
                        break;
                    case Selection.PlayerHero:
                        // act based on the object over which mouse is now
                        if (mapHero && !label)
                        {
                            // change cursor to different based on the relationships between factions
                            Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapHero.LHeroParty.Faction);
                            switch (relationships)
                            {
                                case Relationships.State.SameFaction:
                                    // highlighted hero belongs to player
                                    // check if this is the same hero as selected
                                    if (mapHero.GetInstanceID() == selectedMapHero.GetInstanceID())
                                    {
                                        // verify if hero is in city
                                        if (mapHero.lMapCity)
                                        {
                                            //Debug.Log("hero in city");
                                            // hero is in city
                                            // change cursor to open doors cursor -> enter city edit mode indicator
                                            transform.root.Find("CursorController").GetComponent<CursorController>().SetOpenDoorsCursor();
                                        }
                                        else
                                        {
                                            //Debug.Log("hero on map");
                                            // her os on map
                                            // change cursor to edit hero cursor
                                            transform.root.Find("CursorController").GetComponent<CursorController>().SetEditHeroCursor();
                                            //tileHighlighterColor = Color.blue;
                                        }
                                    }
                                    else
                                    {
                                        // change cursor to selection hand
                                        //transform.root.Find("CursorController").GetComponent<CursorController>().SetMoveArrowCursor();
                                        transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                                    }
                                    break;
                                case Relationships.State.Allies:
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetAttackCursor();
                                    //tileHighlighterColor = Color.red;
                                    break;
                                default:
                                    Debug.LogError("Unknown relationships " + relationships.ToString());
                                    break;
                            }
                        }
                        if (mapHeroViaLabel)
                        {
                            // change cursor to different based on the relationships between factions
                            Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapHero.LHeroParty.Faction);
                            switch (relationships)
                            {
                                case Relationships.State.SameFaction:
                                    // highlighted hero belongs to player
                                    // check if this is the same hero as selected
                                    if (mapHero.GetInstanceID() == selectedMapHero.GetInstanceID())
                                    {
                                        Debug.Log("Same hero");
                                        // verify if hero is in city
                                        if (mapHero.lMapCity)
                                        {
                                            Debug.Log("hero in city");
                                            // hero is in city
                                            // change cursor to open doors cursor -> enter city edit mode indicator
                                            transform.root.Find("CursorController").GetComponent<CursorController>().SetOpenDoorsCursor();
                                        }
                                        else
                                        {
                                            Debug.Log("hero on map");
                                            // her os on map
                                            // change cursor to edit hero cursor
                                            transform.root.Find("CursorController").GetComponent<CursorController>().SetEditHeroCursor();
                                            //tileHighlighterColor = Color.blue;
                                        }
                                    }
                                    else
                                    {
                                        // change cursor to selection hand
                                        transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                                        //tileHighlighterColor = Color.blue;
                                    }
                                    break;
                                case Relationships.State.Allies:
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
                                    //tileHighlighterColor = Color.red;
                                    break;
                                default:
                                    Debug.LogError("Unknown relationships " + relationships.ToString());
                                    break;
                            }
                        }
                        if (mapCity && !label)
                        {
                            //Debug.Log("Enter city box " + mapCity.name);
                            // check relationships with active player
                            Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapCity.LCity.CityFaction);
                            switch (relationships)
                            {
                                case Relationships.State.SameFaction:
                                case Relationships.State.Allies:
                                    // change cursor to selection hand
                                    // Todo: change to move to city cursor
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetMoveArrowCursor();
                                    //tileHighlighterColor = Color.blue;
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetAttackCursor();
                                    //tileHighlighterColor = Color.red;
                                    break;
                                default:
                                    Debug.LogError("Unknown relationships " + relationships.ToString());
                                    break;
                            }
                        }
                        if (mapCityViaLabel)
                        {
                            Debug.Log("Enter city label box " + mapCity.name);
                            // check relationships with active player
                            Relationships.State relationships = Relationships.Instance.GetRelationships(TurnsManager.Instance.GetActivePlayer().Faction, mapCity.LCity.CityFaction);
                            switch (relationships)
                            {
                                case Relationships.State.SameFaction:
                                    // change cursor to selection hand
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                                    //tileHighlighterColor = Color.blue;
                                    break;
                                case Relationships.State.Allies:
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
                                    //tileHighlighterColor = Color.red;
                                    break;
                                default:
                                    Debug.LogError("Unknown relationships " + relationships.ToString());
                                    break;
                            }
                        }
                        if (mapItem)
                        {
                            // change cursor to grab hand
                            transform.root.Find("CursorController").GetComponent<CursorController>().SetGrabHandCursor();
                        }
                        // at this stage we assume that pointer is over map terrain
                        // controls here are handled by Update() function
                        break;
                    case Selection.PlayerCity:
                        // act based on the object over which mouse is now
                        if (mapHero)
                        {
                            // verify if this is player's hero
                            HeroParty heroParty = mapHero.LHeroParty;
                            if (TurnsManager.Instance.GetActivePlayer().Faction == heroParty.Faction)
                            {
                                // highlighted hero belongs to player
                                // change cursor to selection hand
                                transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                // do nothing
                                // user cannot select and control units or cities which he does not own
                            }
                        }
                        if (mapCity)
                        {
                            // verify if this is player's city
                            City city = mapCity.LCity;
                            if (TurnsManager.Instance.GetActivePlayer().Faction == city.CityFaction)
                            {
                                // highlighted city belongs to player
                                // Verify if it is the same city as already selected
                                if (mapCity.GetInstanceID() == selectedCity.GetInstanceID())
                                {
                                    // change cursor to open doors cursor
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetOpenDoorsCursor();
                                }
                                else
                                {
                                    // change cursor to selection hand
                                    transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                                }
                            }
                            else
                            {
                                // highlighted city is from other faction
                                // do nothing
                                // user cannot select and control units or cities which he does not own
                            }
                        }
                        if (mapItem)
                        {
                            // nothing to do here
                        }
                        // at this stage we assume that pointer is over map terrain
                        // nothing to do, just keep normal pointer, which was set after exit highlighting other objects
                        break;
                    default:
                        Debug.LogError("Unknown selection " + selection.ToString());
                        break;
                }
                break;
            case Mode.Animation:
                // Ignore mouse enter in this mode
                break;
            case Mode.Drag:
                // Ignore mouse enter in this mode
                break;
            default:
                Debug.LogError("Unknown mode " + mode.ToString());
                break;
        }
        //// Update color of tile highlighter
        //if (label)
        //{
        //    // Hide tile highlighter if mouse if over label
        //    Debug.Log("Hide tile highlighter");
        //    tileHighlighterColor = new Color32(0, 0, 0, 0);
        //}
        //UpdateTileHighlighterColor();
    }

    //public void OnPointerExitChildObject(GameObject childGameObject, PointerEventData eventData)
    //{

    //}
    public void OnPointerExitChildObject(GameObject childGameObject, PointerEventData eventData)
    {
        switch (mode)
        {
            case Mode.Browse:
                // change cursor
                transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
                //tileHighlighterColor = Color.white;
                break;
            case Mode.Animation:
                // Ignore mouse exit in this mode
                break;
            case Mode.Drag:
                // Ignore mouse exit in this mode
                break;
            default:
                Debug.LogError("Unknown mode " + mode.ToString());
                break;
        }
    }

    void DeselectPreviouslySelectedObjectsOnMap()
    {
        // deselect previous hero if it was present
        if (selectedMapHero)
        {
            selectedMapHero.SetSelectedState(false);
            selectedMapHero = null;
        }
        // deselect previous city if it was selected
        if (selectedCity)
        {
            selectedCity.SetSelectedState(false);
            selectedCity = null;
        }
        // remove move path highlight
        if (selectedTargetPathPoint != null)
        {
            HighlightMovePath(false);
            selectedTargetPathPoint = null;
        }
    }

    void VerifyLastPathPoint(NesScripts.Controls.PathFind.Point clickedTargetPathPoint)
    {
        // this is may be overhead
        int lastPathPoint = movePath.Count - 1;
        if (lastPathPoint >= 0)
        {
            Debug.Log("lastPathPoint " + lastPathPoint.ToString());
            if (clickedTargetPathPoint != movePath[lastPathPoint])
            {
                Debug.LogError("Logical error. They should be equal");
            }
        }
    }

    void SaveLastPathPoint()
    {
        // verify that we have 1 or more move path points
        if (movePath.Count >= 1)
        {
            // Save target path point for later checks
            selectedTargetPathPoint = movePath[movePath.Count - 1];
        }
        else
        {
            // no path
            selectedTargetPathPoint = null;
        }
    }

    void VerifyMovePathAndMoveIfNeeded(PointerEventData eventData)
    {
        Vector2Int clickedTargetTile = GetTileByPosition(eventData.position);
        NesScripts.Controls.PathFind.Point clickedTargetPathPoint = new NesScripts.Controls.PathFind.Point(clickedTargetTile.x, clickedTargetTile.y);
        // verify if we already highlighted path
        if (selectedTargetPathPoint != null)
        {
            // We already have path highlighted
            // verify if it is path to the same target
            if (selectedTargetPathPoint == clickedTargetPathPoint)
            {
                // we target the same path point
                // Move to the point
                StartCoroutine(Move());
                // Reset path point
                selectedTargetPathPoint = null;
            }
            else
            {
                // we target new path point
                // find and highlight new path
                FindAndHighlightPath();
                // Save target path point for later checks
                SaveLastPathPoint();
                // just in case do verifications
                VerifyLastPathPoint(clickedTargetPathPoint);
            }
        }
        else
        {
            // there is no yet path highlighted
            // find and highlight new path
            FindAndHighlightPath();
            // Save target path point for later checks
            SaveLastPathPoint();
            // just in case do verifications
            VerifyLastPathPoint(clickedTargetPathPoint);
        }
    }

    public void SetSelection(Selection s)
    {
        // set selection
        selection = s;
        // set additional settings according to selection
        switch (s)
        {
            case Selection.None:
                // deselect hero party
                DeselectPreviouslySelectedObjectsOnMap();
                // change cursor to normal
                transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
                // update focus panel
                transform.root.Find("MapScreen/MapMenu").GetComponentInChildren<MapFocusPanel>().ReleaseFocus();
                break;
            default:
                Debug.LogError("Unknown or incompatible selection " + selection);
                break;
        }
    }

    public void SetSelection(Selection s, MapHero mH)
    {
        // set selection
        selection = s;
        // set additional settings according to selection
        switch (s)
        {
            case Selection.PlayerHero:
                DeselectPreviouslySelectedObjectsOnMap();
                SetSelectedHero(mH);
                // Update map focus panel
                transform.root.Find("MapScreen/MapMenu").GetComponentInChildren<MapFocusPanel>().SetActive(mH);
                mH.SetSelectedState(true);
                // verify if hero is in city
                if (mH.lMapCity)
                {
                    //Debug.Log("SetSelection hero in city");
                    // hero is in city
                    // change cursor to open doors cursor -> enter city edit mode indicator
                    transform.root.Find("CursorController").GetComponent<CursorController>().SetOpenDoorsCursor();
                }
                else
                {
                    //Debug.Log("SetSelection hero on map");
                    // her os on map
                    // change cursor to edit hero cursor
                    transform.root.Find("CursorController").GetComponent<CursorController>().SetEditHeroCursor();
                    //tileHighlighterColor = Color.blue;
                }
                break;
            default:
                Debug.LogError("Unknown or incompatible selection " + selection);
                break;
        }
    }

    public void SetSelection(Selection s, MapCity mC)
    {
        // set selection
        selection = s;
        // set additional settings according to selection
        switch (s)
        {
            case Selection.PlayerCity:
                DeselectPreviouslySelectedObjectsOnMap();
                SetSelectedCity(mC);
                // Update map focus panel
                transform.root.Find("MapScreen/MapMenu").GetComponentInChildren<MapFocusPanel>().SetActive(mC);
                mC.SetSelectedState(true);
                // change cursor to open doors cursor
                transform.root.Find("CursorController").GetComponent<CursorController>().SetOpenDoorsCursor();
                break;
            default:
                Debug.LogError("Unknown or incompatible selection " + selection);
                break;
        }
    }

    public void EnterCityOnMap(MapCity mapCity)
    {
        // remove selection
        SetSelection(Selection.None);
        // Enter city edit mode
        SetMode(Mode.Animation);
        queue.Run(mapCity.EnterCityEditMode());
    }

    public void ActOnClick(GameObject childGameObject, PointerEventData pointerEventData)
    {
        // predefine variables
        MapHero mapHero = childGameObject.GetComponent<MapHero>();
        MapCity mapCity = childGameObject.GetComponent<MapCity>();
        MapItem mapItem = childGameObject.GetComponent<MapItem>();
        MapObjectLabel mapHeroViaLabel = null;
        MapObjectLabel mapCityViaLabel = null;
        MapObjectLabel label = childGameObject.GetComponent<MapObjectLabel>();
        if (label)
        {
            // find out on which label we clicked
            if (label.transform.parent.GetComponent<MapHero>())
            {
                mapHeroViaLabel = label;
            }
            if (label.transform.parent.GetComponent<MapCity>())
            {
                mapCityViaLabel = label;
            }
        }
        MapManager mapMgr = childGameObject.GetComponent<MapManager>();
        //Debug.Log("Mode " + mode.ToString());
        // act based on current mode
        switch (mode)
        {
            // city click
            //        case MapManager.Mode.Browse:
            //            EnterCityEditMode();
            //            break;
            //        case MapManager.Mode.HighlightMovePath:
            //            // Move hero to the city
            //            transform.parent.GetComponent<MapManager>().EnterMoveMode();
            //            break;
            case Mode.Browse:
                // act based on current selection
                //Debug.Log("Selection " + selection.ToString());
                switch (selection)
                {
                    case Selection.None:
                        // act based on the object over which mouse is now
                        if (mapHero || mapHeroViaLabel)
                        {
                            //Debug.Log("Clicked on hero party on map");
                            // get Hero Party depending on wheter user clicked on mapHero or its label
                            if (mapHeroViaLabel)
                            {
                                mapHero = mapHeroViaLabel.transform.parent.GetComponent<MapHero>();
                            }
                            HeroParty heroParty = mapHero.LHeroParty;
                            // verify if this is player's hero
                            if (TurnsManager.Instance.GetActivePlayer().Faction == heroParty.Faction)
                            {
                                // highlighted hero belongs to player
                                // select this hero
                                SetSelection(Selection.PlayerHero, mapHero);
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                // do nothing, because we did not select our hero which can attack it
                            }
                        }
                        if (mapCity || mapCityViaLabel)
                        {
                            //Debug.Log("Clicked on city on map");
                            // get Hero City depending on wheter user clicked on mapCity or its label
                            if (mapCityViaLabel)
                            {
                                mapCity = mapCityViaLabel.transform.parent.GetComponent<MapCity>();
                            }
                            City city = mapCity.LCity;
                            // verify if this is player's city
                            if (TurnsManager.Instance.GetActivePlayer().Faction == city.CityFaction)
                            {
                                // highlighted city belongs to player
                                // select this city
                                SetSelection(Selection.PlayerCity, mapCity);
                            }
                            else
                            {
                                // highlighted city is from other faction
                                // do nothing
                            }
                        }
                        if (mapItem)
                        {
                            // nothing to do here
                        }
                        break;
                    case Selection.PlayerHero:
                        if (mapCity)
                        {
                            //Debug.Log("Clicked on city on map");
                            // verify if hero is already in the same city
                            if (selectedMapHero.lMapCity)
                            {
                                if (selectedMapHero.lMapCity.GetInstanceID() == mapCity.GetInstanceID())
                                {
                                    // Hero is in the same city
                                    // Enter city edit mode
                                    // remove selection
                                    SetSelection(Selection.None);
                                    // Enter city edit mode
                                    SetMode(Mode.Animation);
                                    queue.Run(mapCity.EnterCityEditMode());
                                }
                                else
                                {
                                    // Move to the city
                                    VerifyMovePathAndMoveIfNeeded(pointerEventData);
                                }
                            }
                            else
                            {
                                // Move to the city
                                VerifyMovePathAndMoveIfNeeded(pointerEventData);
                            }
                        }
                        if (mapCityViaLabel)
                        {
                            //Debug.Log("Clicked on city label on map");
                            // get Hero City
                            mapCity = mapCityViaLabel.transform.parent.GetComponent<MapCity>();
                            City city = mapCity.LCity;
                            // verify if this is player's city
                            if (TurnsManager.Instance.GetActivePlayer().Faction == city.CityFaction)
                            {
                                // highlighted city belongs to player
                                // select this city instead of previously selected city
                                SetSelection(Selection.PlayerCity, mapCity);
                            }
                            else
                            {
                                // highlighted city is from other faction
                                SetSelection(Selection.None);
                            }
                        }
                        if (mapHero)
                        {
                            //Debug.Log("Clicked on hero's partie's marker on map");
                            HeroParty heroParty = mapHero.LHeroParty;
                            // verify if this is player's hero
                            if (TurnsManager.Instance.GetActivePlayer().Faction == heroParty.Faction)
                            {
                                // Debug.LogWarning("1");
                                // highlighted hero belongs to player
                                // verify it it is the same hero as already selected
                                if (selectedMapHero.GetInstanceID() == mapHero.GetInstanceID())
                                {
                                    // Debug.LogWarning("2");
                                    // same hero as selected
                                    // remove selection
                                    SetSelection(Selection.None);
                                    // Enter hero edit mode
                                    SetMode(Mode.Animation);
                                    queue.Run(mapHero.EnterHeroEditMode());
                                }
                                else
                                {
                                    // Debug.LogWarning("3");
                                    // other hero from the same faction
                                    // select other hero
                                    //VerifyMovePathAndMoveIfNeeded(pointerEventData);
                                    SetSelection(Selection.PlayerHero, mapHero);
                                }
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                VerifyMovePathAndMoveIfNeeded(pointerEventData);
                            }
                        }
                        if (mapHeroViaLabel)
                        {
                            //Debug.Log("Clicked on hero's partie's lable on map");
                            // get Hero Party depending on wheter user clicked on mapHero or its label
                            mapHero = mapHeroViaLabel.transform.parent.GetComponent<MapHero>();
                            HeroParty heroParty = mapHero.LHeroParty;
                            // verify if this is player's hero
                            if (TurnsManager.Instance.GetActivePlayer().Faction == heroParty.Faction)
                            {
                                // Debug.LogWarning("1");
                                // highlighted hero belongs to player
                                // verify it it is the same hero as already selected
                                if (selectedMapHero.GetInstanceID() == mapHero.GetInstanceID())
                                {
                                    // Debug.LogWarning("2");
                                    // same hero as selected
                                    // remove selection
                                    SetSelection(Selection.None);
                                    // enter animation mode to disable all input
                                    SetMode(Mode.Animation);
                                    // verify if hero is in city
                                    if (mapHero.lMapCity)
                                    {
                                        // hero is in city
                                        // enter city edit mode
                                        MapCity _mapCity = mapHero.lMapCity;
                                        queue.Run(_mapCity.EnterCityEditMode());
                                    }
                                    else
                                    {
                                        // her os on map
                                        // enter hero edit mode
                                        queue.Run(mapHero.EnterHeroEditMode());
                                    }
                                }
                                else
                                {
                                    // Debug.LogWarning("3");
                                    // other hero
                                    // select new hero
                                    SetSelection(Selection.PlayerHero, mapHero);
                                }
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                SetSelection(Selection.None);
                            }
                        }
                        if (mapItem)
                        {
                            // Move to the item on map
                            VerifyMovePathAndMoveIfNeeded(pointerEventData);
                        }
                        if (mapMgr)
                        {
                            VerifyMovePathAndMoveIfNeeded(pointerEventData);
                        }
                        break;
                    case Selection.PlayerCity:
                        if (mapHero || mapHeroViaLabel)
                        {
                            //Debug.Log("Clicked on hero party on map");
                            // get Hero Party depending on wheter user clicked on mapHero or its label
                            if (mapHeroViaLabel)
                            {
                                mapHero = mapHeroViaLabel.transform.parent.GetComponent<MapHero>();
                            }
                            HeroParty heroParty = mapHero.LHeroParty;
                            // verify if this is player's hero
                            if (TurnsManager.Instance.GetActivePlayer().Faction == heroParty.Faction)
                            {
                                // highlighted hero belongs to player
                                // select this hero instead of previously selected city
                                SetSelection(Selection.PlayerHero, mapHero);
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                // remove selection
                                SetSelection(Selection.None);
                            }
                        }
                        if (mapCity || mapCityViaLabel)
                        {
                            //Debug.Log("Clicked on city on map");
                            // get Hero Party depending on wheter user clicked on mapCity or its label
                            if (mapCityViaLabel)
                            {
                                mapCity = mapCityViaLabel.transform.parent.GetComponent<MapCity>();
                            }
                            City city = mapCity.LCity;
                            // verify if this is player's city
                            if (TurnsManager.Instance.GetActivePlayer().Faction == city.CityFaction)
                            {
                                // Debug.LogWarning("1");
                                // highlighted city belongs to player
                                // verify it it is the same city as already selected
                                if (selectedCity.GetInstanceID() == mapCity.GetInstanceID())
                                {
                                    // Debug.LogWarning("2");
                                    // same city as selected
                                    EnterCityOnMap(mapCity);
                                }
                                else
                                {
                                    // Debug.LogWarning("3");
                                    // other city
                                    // select new city
                                    SetSelection(Selection.PlayerCity, mapCity);
                                }
                            }
                            else
                            {
                                // highlighted city is from other faction
                                // remove selection
                                SetSelection(Selection.None);
                            }
                        }
                        if (mapItem)
                        {
                            // nothing to do here
                        }
                        if (mapMgr)
                        {
                            // remove selection
                            SetSelection(Selection.None);
                        }
                        break;
                    default:
                        Debug.LogError("Unknown selection " + selection.ToString());
                        break;
                }
                break;
            case Mode.Animation:
                // Ignore mouse clicks in this mode
                break;
            case Mode.Drag:
                // Ignore mouse clicks in this mode
                break;
            default:
                Debug.LogError("Unknown mode " + mode.ToString());
                break;
        }
    }

    Vector2Int GetEscapeDestination(Vector2Int fleeingPartyPositionOnMap, Vector2Int direction)
    {
        Vector2Int destination = direction + fleeingPartyPositionOnMap;
        // verify if destination is within map borders
        if (
            (destination.x >= 1)
            && (destination.y >= 1)
            && (destination.x <= tilesmap.GetLength(0))
            && (destination.y <= tilesmap.GetLength(1))
            )
        {
            // verify if destination tile is not occupied
            if (!GetObjectOnTile(destination))
            {
                return destination;
            }
        }
        // by default return the same position where party is now, if there is no place to escape
        return fleeingPartyPositionOnMap;
    }

    public void EscapeBattle(Transform fleeingPartyTransform, Transform oppositeObjectTransform)
    {
        // fleeing party position on map
        Vector2Int fleeingPartyPositionOnMap = GetTileByPosition(fleeingPartyTransform.position);
        Vector2Int oppositeObjectPositionOnMap = GetTileByPosition(oppositeObjectTransform.position);
        // get flee vector based on current position of the objects
        Vector2Int direction = fleeingPartyPositionOnMap - oppositeObjectPositionOnMap;
        Debug.Log(direction);
        // get destination
        Vector2Int destination = GetEscapeDestination(fleeingPartyPositionOnMap, direction);
        // set selectedHero - it will be later used by EnterCityAfterMove function
        selectedMapHero = fleeingPartyTransform.GetComponent<MapHero>();
        // set move path
        SetMovePath(fleeingPartyPositionOnMap, destination);
        // start move 
        Debug.Log("StartCoroutine(Move())");
        StartCoroutine(Move());
    }
}
