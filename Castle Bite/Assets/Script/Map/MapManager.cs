using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
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

    // for path highlighting
    public enum TileState
    {
        None,
        ImpassableTerrain,
        SelectedParty,
        PlayerParty,
        EnemyParty,
        AlliedParty,
        Treasure,
        PlayerCity,
        EnemyCity,
        Protected
    }

    [SerializeField]
    Mode mode;
    [SerializeField]
    Selection selection;
    Vector3 startPosition;
    // Transform startParent;
    [SerializeField]
    int tileSize = 16;
    Transform tileHighlighterTr;
    TileHighlighter tileHighlighter;
    // for pathfinding
    bool[,] tilesmap;
    int tileMapWidth = 60;
    int tileMapHeight = 60;
    MapHero selectedHero;
    MapCity selectedCity;
    NesScripts.Controls.PathFind.Grid grid;
    List<NesScripts.Controls.PathFind.Point> movePath;
    GameObject[,] tileHighlighters;
    // for hero moving
    public float heroMoveSpeed = 10.1f;
    public float heroMoveSpeedDelay = 0.1f;
    TileState lastTileState = TileState.None;
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

    // for logic
    PlayerObj player;

    public Mode GetMode()
    {
        return mode;
    }

    public void SetSelectedHero(MapHero sltdHero)
    {
        selectedHero = sltdHero;
    }

    public MapHero GetSelectedHero()
    {
        return selectedHero;
    }

    public void SetSelectedCity(MapCity sltCity)
    {
        selectedCity = sltCity;
    }

    void Start()
    {
        // preapre commonly used variables
        player = transform.root.Find("PlayerObj").GetComponent<PlayerObj>();
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
        foreach (EnemyPartyOnMap enemyParty in transform.GetComponentsInChildren<EnemyPartyOnMap>())
        {
            // verify if not null
            if (enemyParty)
            {
                Vector2Int pos = GetTilePosition(enemyParty.transform);
                tilesmap[pos.x, pos.y] = false;
            }
        }
        foreach (PlayerPartyOnMap playerParty in transform.GetComponentsInChildren<PlayerPartyOnMap>())
        {
            // verify if not null
            if (playerParty)
            {
                Vector2Int pos = GetTilePosition(playerParty.transform);
                tilesmap[pos.x, pos.y] = false;
            }
        }
        foreach (PlayerCityOnMap playerCity in transform.GetComponentsInChildren<PlayerCityOnMap>())
        {
            // verify if not null
            if (playerCity)
            {
                Vector2Int pos = GetTilePosition(playerCity.transform);
                tilesmap[pos.x, pos.y] = false;
            }
        }
    }

    void InitPathFinder()
    {
        // create a grid
        grid = new NesScripts.Controls.PathFind.Grid(tilesmap);
    }

    void UpdateTileHighlighterToMousePoistion()
    {
        int x = Mathf.FloorToInt(Input.mousePosition.x / tileSize);
        int y = Mathf.FloorToInt(Input.mousePosition.y / tileSize);
        tileHighlighterTr.position = new Vector3(x, y, 0) * tileSize;
        // Debug.Log("Tile: " + x + ";" + y);
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
            TileState highlightedTileState = GetTileOccupationState(highlightedPoint);
            // highlight based on the occupation type
            switch (highlightedTileState)
            {
                case TileState.None:
                case TileState.SelectedParty:
                case TileState.Protected:
                    // nothing to do
                    break;
                case TileState.PlayerParty:
                case TileState.PlayerCity:
                case TileState.EnemyParty:
                    // adjust grid to make this tile passable
                    tilesmap[highlightedPoint.x, highlightedPoint.y] = true;
                    break;
                default:
                    Debug.LogError("Unknown tile state " + highlightedTileState.ToString());
                    break;
            }
        }
        // Upgrade grid
        grid.UpdateGrid(tilesmap);
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
                    UpdateTileHighlighterToMousePoistion();
                    //// update tile highliter position
                    //UpdateGridBasedOnHighlightedTile();
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

    Vector2Int GetTilePosition(Transform tr)
    {
        return new Vector2Int
        {
            x = Mathf.FloorToInt(tr.position.x / tileSize),
            y = Mathf.FloorToInt(tr.position.y / tileSize)
        };
    }

    Vector2Int GetHeroTilePosition()
    {
        //Vector2Int result = new Vector2Int
        //{
        //    x = Mathf.FloorToInt(selectedHero.transform.position.x / tileSize),
        //    y = Mathf.FloorToInt(selectedHero.transform.position.y / tileSize)
        //};
        return GetTilePosition(selectedHero.transform);
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

    bool TileIsProtectedByEnemyParty(Vector2Int enemyTilePosition, Vector2Int tilePosition)
    {
        foreach (Vector2Int tilePos in GetAllSurroundingTilesPositions(enemyTilePosition))
        {
            if (tilePos == tilePosition)
            {
                return true;
            }
        }
        return false;
    }

    TileState GetTileOccupationState(NesScripts.Controls.PathFind.Point pathPoint)
    {
        Vector2Int tilePosition = new Vector2Int
        {
            x = pathPoint.x,
            y = pathPoint.y
        };
        // go over all objects and verify if they are on the tile
        foreach (EnemyPartyOnMap enemyParty in transform.GetComponentsInChildren<EnemyPartyOnMap>())
        {
            // verify if not null
            if (enemyParty)
            {
                Vector2Int enemyTilePosition = GetTilePosition(enemyParty.transform);
                if (enemyTilePosition == tilePosition)
                {
                    return TileState.EnemyParty;
                }
                else if (TileIsProtectedByEnemyParty(enemyTilePosition, tilePosition))
                {
                    // Debug.LogWarning("Protected");
                    return TileState.Protected;
                }
            }
        }
        foreach (PlayerPartyOnMap playerParty in transform.GetComponentsInChildren<PlayerPartyOnMap>())
        {
            // verify if not null
            if (playerParty)
            {
                if (GetTilePosition(playerParty.transform) == tilePosition)
                {
                    return TileState.PlayerParty;
                }
            }
        }
        foreach (PlayerCityOnMap playerCity in transform.GetComponentsInChildren<PlayerCityOnMap>())
        {
            // verify if not null
            if (playerCity)
            {
                if (GetTilePosition(playerCity.transform) == tilePosition)
                {
                    return TileState.PlayerCity;
                }
            }
        }
        return TileState.None;
    }

    Color GetTileHighlightColor(TileState tileState)
    {
        // highligh the last tile depending on what is under cursor
        // the might be another hero or enemy party or treasure chest or ally party
        // highlight based on the occupation type
        switch (tileState)
        {
            case TileState.None:
                // nothing to do
                return Color.green;
            case TileState.SelectedParty:
                // this should not be possible, because move path in this case is 0
                Debug.LogError("Not possible condition");
                return Color.magenta;
            case TileState.PlayerParty:
                // highlight other party
                // highlight blue
                return Color.blue;
            case TileState.PlayerCity:
                // highlight city
                // highlight yellow
                return Color.yellow;
            case TileState.EnemyParty:
            case TileState.Protected:
                // highlight red
                return Color.red;
            default:
                Debug.LogError("Unknown tile state " + tileState.ToString());
                return Color.magenta;
        }
    }

    public void HighlightMovePath(bool doHighlight)
    {
        Color highlightColor = Color.green;
        // todo: it is better to make them transparant, then instantiate new and destroy each time
        if (movePath != null && movePath.Count > 0)
        {
            // Set lastPathTile variable for later user
            if (doHighlight)
            {
                lastPathTile = movePath[movePath.Count - 1];
                // set lastTileState value it will be used here and in other functions too
                lastTileState = GetTileOccupationState(lastPathTile);
                // Debug.Log(pathPoint.x.ToString() + ":" + pathPoint.y.ToString() + " " + lastTileState.ToString());
            }
            // Highlight all tiles
            // Initialize enemy of a way as false, like we do not encounter enemy on our move path
            bool enemyOnWay = false;
            Color tileHighlighColor;
            foreach (NesScripts.Controls.PathFind.Point pathPoint in movePath)
            {
                // output path to debug
                // Debug.Log("Path point is [" + pathPoint.x + "]:[" + pathPoint.y + "]");
                tileHighlighters[pathPoint.x, pathPoint.y].SetActive(doHighlight);
                // reset collor to correct highlight color
                // because it might be changed by previous highlight operations to something else
                // if for example in the past there was enemy standing and we were highliting is with red color
                TileState tileState = GetTileOccupationState(pathPoint);
                // verify if at least once we have an enemy on our path
                if ( (TileState.EnemyParty == tileState) || (TileState.Protected == tileState))
                {
                    // Set enemy on way to true
                    // This and all tiles after this should be highlighted red
                    enemyOnWay = true;
                }
                // Verify if enemy on way was triggered
                if (enemyOnWay)
                {
                    // set tile highlight color as if there was an enemy
                    tileHighlighColor = GetTileHighlightColor(TileState.Protected);
                }
                else
                {
                    // get tile highlight color based on a tile state (what is in or near tile)
                    tileHighlighColor = GetTileHighlightColor(tileState);
                }
                // Highlight tile
                tileHighlighters[pathPoint.x, pathPoint.y].GetComponentInChildren<Text>().color = tileHighlighColor;
            }
        }
    }

    void FindAndHighlightPath()
    {
        // First remove highlight from previous path
        HighlightMovePath(false);
        // create source and target points
        NesScripts.Controls.PathFind.Point _from = new NesScripts.Controls.PathFind.Point(GetHeroTilePosition().x, GetHeroTilePosition().y);
        // Debug.Log("From [" + _from.x + "]:[" + _from.y + "]");
        Vector2Int highlighterPosition = GetTileHighlighterPosition();
        NesScripts.Controls.PathFind.Point _to = new NesScripts.Controls.PathFind.Point(highlighterPosition.x, highlighterPosition.y);
        // Debug.Log("To [" + _to.x + "]:[" + _to.y + "]");
        // Debug.Log("Length [" + (grid.nodes.GetLength(0) - 1).ToString() + "]:[" + (grid.nodes.GetLength(1) - 1).ToString() + "]");
        // get path
        // path will either be a list of Points (x, y), or an empty list if no path is found.
        // verify if mouse is not over screen
        if (   (_to.x > grid.nodes.GetLength(0) - 1) 
            || (_to.y > grid.nodes.GetLength(1) - 1)
            || (_to.x < 0)
            || (_to.y < 0) )
        {
            // nothing to highlight, mouse is over the screen
        }
        else
        {
            movePath = NesScripts.Controls.PathFind.Pathfinding.FindPath(grid, _from, _to);
            // for Manhattan distance
            // List<NesScripts.Controls.PathFind.Point> path = NesScripts.Controls.PathFind.Pathfinding.FindPath(grid, _from, _to, NesScripts.Controls.Pathfinding.DistanceType.Manhattan);
            // highlight new path
            HighlightMovePath(true);
        }
        // Debug.Log("FindAndHighlightPath");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // save mouse position, it may be required for OnBeginDrag
        mouseOnDownStartPosition = Input.mousePosition;
        // disable tile highliter
        tileHighlighter.SetActive(false);
    }

    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        // enter Drag mode
        SetMode(Mode.Drag);
        // prepare for drag
        startPosition = transform.position;
        // startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        // get mouse start possition and calculate which offset to apply to final transform
        // this is because I would like to avoid that map center is also centered under mouse
        // this create strange map jumps
        // mouseOnDragStartPosition = Input.mousePosition;
        // apply corrections depeding on location from the canvas center
        // this actually depends one the current position of the map
        mapPosiiton = Camera.main.WorldToScreenPoint(transform.position);
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
    #endregion

    #region IDragHandler implementation
    public void OnDrag(PointerEventData eventData)
    {
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
        Vector3 newPosition = new Vector3(newPositionX + xCorrectionOnDragStart, newPositionY + yCorrectionOnDragStart, startPosition.z);
        // make drag to allign with tile size
        float x = Mathf.RoundToInt(newPosition.x / tileSize) * tileSize;
        float y = Mathf.RoundToInt(newPosition.y / tileSize) * tileSize;
        newPosition = new Vector3(x, y, newPosition.z);
        transform.position = Camera.main.ScreenToWorldPoint(newPosition);
        //    Debug.Log("transform " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
    }
    #endregion

    #region IEndDragHandler implementation
    public void OnEndDrag(PointerEventData eventData)
    {
        // enter back to Browse mode
        SetMode(Mode.Browse);
        // set tile highighter under mouse
        UpdateTileHighlighterToMousePoistion();
        // allow map to block raycasts
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    #endregion

    float GetRemainingDistance(NesScripts.Controls.PathFind.Point pathPoint)
    {
        float result = 0;
        Vector2 dst = GetDestination(pathPoint);
        Vector2 src = selectedHero.transform.position;
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

    void EnterCityAfterMove()
    {
        Debug.Log("Enter city");
        // Get City
        MapCity mapCity = GetCityByTile(new Vector2Int(lastPathTile.x, lastPathTile.y));
        // Link hero on the map to city on the map
        mapCity.linkedPartyTr = selectedHero.transform;
        // And do the opposite 
        // Link city on the map to hero on the map
        selectedHero.linkedCityOnMapTr = mapCity.transform;
        // Move hero UI to City
        selectedHero.linkedPartyTr.SetParent(mapCity.linkedCityTr);
        // Enter city edit mode
        mapCity.EnterCityEditMode();
        // Trigger on hero entering city
        mapCity.linkedCityTr.GetComponent<City>().ActOnHeroEnteringCity();
    }

    MapCity GetCityByTile(Vector2Int tilePosition)
    {
        MapCity mapCity = null;
        foreach (MapCity city in transform.GetComponentsInChildren<MapCity>())
        {
            // verify if not null
            if (city)
            {
                if (GetTilePosition(city.transform) == tilePosition)
                {
                    return city;
                }
            }
        }
        return mapCity;
    }

    MapHero GetEnemyByTile(Vector2Int tilePosition)
    {
        foreach (EnemyPartyOnMap enemyOnMap in transform.GetComponentsInChildren<EnemyPartyOnMap>())
        {
            // verify if not null
            if (enemyOnMap)
            {
                if (GetTilePosition(enemyOnMap.transform) == tilePosition)
                {
                    return enemyOnMap.GetComponent<MapHero>();
                }
            }
        }
        return null;
    }

    MapHero GetEnemyByProtectedTile(Vector2Int tilePosition)
    {
        Vector2Int[] surroundingTilesPositions = GetAllSurroundingTilesPositions(tilePosition);
        foreach (EnemyPartyOnMap enemyOnMap in transform.GetComponentsInChildren<EnemyPartyOnMap>())
        {
            // verify if not null
            if (enemyOnMap)
            {
                Vector2Int enemyTilePos = GetTilePosition(enemyOnMap.transform);
                foreach (Vector2Int tilePos in surroundingTilesPositions)
                {
                    if (enemyTilePos == tilePos)
                    {
                        return enemyOnMap.GetComponent<MapHero>();
                    }
                }
            }
        }
        return null;
    }

    void EnterBattleOnMove(NesScripts.Controls.PathFind.Point pathPoint)
    {
        Debug.Log("Enter battle");
        MapHero enemyOnMap = null;
        TileState tileState = GetTileOccupationState(pathPoint);
        switch (tileState)
        {
            case TileState.EnemyParty:
                // Get Enemy party
                enemyOnMap = GetEnemyByTile(new Vector2Int(pathPoint.x, pathPoint.y));
                break;
            case TileState.Protected:
                // get random enemy which is protecting this tile
                enemyOnMap = GetEnemyByProtectedTile(new Vector2Int(pathPoint.x, pathPoint.y));
                break;
            default:
                Debug.LogError("Unhandled condition " + tileState.ToString());
                break;
        }
        // initialize battle
        if (enemyOnMap)
        {
            transform.root.Find("BattleScreen").GetComponent<BattleScreen>().EnterBattle(selectedHero, enemyOnMap);
        }
        else
        {
            Debug.LogError("Enemy not found");
        }
    }

    void EndMoveTransition()
    {
        // transition to required state based on the type of the last occupied cell
        switch (lastTileState)
        {
            case TileState.None:
                // Nothing special to do here
                break;
            case TileState.PlayerCity:
                // enter city
                EnterCityAfterMove();
                break;
            case TileState.EnemyParty:
            case TileState.Protected:
                Debug.LogError("This state should not be here, but we should enter this state during move, state: " + lastTileState.ToString());
                break;
            case TileState.SelectedParty:
            case TileState.PlayerParty:
                // this should not be possible, because move path in this case is 0
                Debug.LogError("Not possible condition " + lastTileState.ToString());
                break;
            default:
                Debug.LogError("Unknown tile state " + lastTileState.ToString());
                break;
        }
    }

    IEnumerator Move()
    {
        // exit exit browse mode and enter animation mode
        SetMode(Mode.Animation);
        // Block mouse input
        InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        inputBlocker.SetActive(true);
        //Debug.Log("Move");
        // Verify if hero was in city
        if (selectedHero.linkedCityOnMapTr)
        {
            // Unlink city from hero and hero from city if they were linked before
            MapCity linkedCity = selectedHero.linkedCityOnMapTr.GetComponent<MapCity>();
            linkedCity.linkedPartyTr = null;
            selectedHero.linkedCityOnMapTr = null;
            // Get current party city
            HeroParty heroParty = selectedHero.linkedPartyTr.GetComponent<HeroParty>();
            City currentCity = heroParty.transform.parent.GetComponent<City>();
            // Enable hire hero panel in city
            currentCity.ReturnToNomalState();
            currentCity.ActOnHeroLeavingCity();
            // Move party from city to PartiesOnMap container
            Transform partiesOnMap = transform.root.Find("PartiesOnMap");
            heroParty.transform.SetParent(partiesOnMap);
            // Update hero party place
            heroParty.SetPlace(HeroParty.PartyPlace.Map);
        }
        // Move
        float deltaTime;
        float previousTime = Time.time;
        if (movePath != null)
        {
            for (int i = 0; i < movePath.Count; i++)
            {
                var pathPoint = movePath[i];
                Vector2 dst = GetDestination(pathPoint);
                while (GetRemainingDistance(pathPoint) > 0.5f)
                {
                    // Debug.Log("Path point is [" + pathPoint.x + "]:[" + pathPoint.y + "]");
                    // move hero
                    deltaTime = Time.time - previousTime;
                    selectedHero.transform.position = Vector2.MoveTowards(selectedHero.transform.position, dst, deltaTime * heroMoveSpeed);
                    // wait until next move
                    previousTime = Time.time;
                    yield return new WaitForSeconds(heroMoveSpeedDelay);
                }
                // get tile state on current path point
                TileState tileState = GetTileOccupationState(pathPoint);
                // verify if state requires hero to enter battle
                if ((TileState.EnemyParty == tileState) || (TileState.Protected == tileState))
                {
                    // enter battle
                    EnterBattleOnMove(pathPoint);
                    // break move
                    break;
                }
                // Verify if we have reached last tile in path here
                if (movePath.Count - 1 == i)
                {
                    // We finished our move
                    // Transition to the next state based on the destination tile state
                    EndMoveTransition();
                }
            }
            //foreach (var pathPoint in movePath)
            //{
            //}
        }
        // Remove path highlight
        HighlightMovePath(false);
        // exit animation mode and enter browse mode
        SetMode(Mode.Browse);
        // Unblock mouse input
        inputBlocker.SetActive(false);
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
        ActOnClick(gameObject, pointerEventData);
    }

    public void OnPointerEnterChildObject(GameObject childGameObject, PointerEventData eventData)
    {
        // predefine variables
        MapHero mapHero = childGameObject.GetComponent<MapHero>();
        MapCity mapCity = childGameObject.GetComponent<MapCity>();
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
                            HeroParty heroParty = mapHero.linkedPartyTr.GetComponent<HeroParty>();
                            if (player.Faction == heroParty.GetFaction())
                            {
                                // highlighted hero belongs to player
                                // change cursor to selection hand
                                transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                // do nothing
                            }
                        }
                        if (mapCity)
                        {
                            // verify if this is player's city
                            City city = mapCity.linkedCityTr.GetComponent<City>();
                            if (player.Faction == city.GetFaction())
                            {
                                // highlighted city belongs to player
                                // change cursor to selection hand
                                transform.root.Find("CursorController").GetComponent<CursorController>().SetSelectionHandCursor();
                            }
                            else
                            {
                                // highlighted city is from other faction
                                // do nothing
                            }
                        }
                        break;
                    case Selection.PlayerHero:
                        break;
                    case Selection.PlayerCity:
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
    }

    public void OnPointerExitChildObject(GameObject childGameObject, PointerEventData eventData)
    {
        switch (mode)
        {
            case Mode.Browse:
                // change cursor
                transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
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

    public void ActOnClick(GameObject childGameObject, PointerEventData pointerEventData)
    {
        // predefine variables
        MapHero mapHero = childGameObject.GetComponent<MapHero>();
        MapCity mapCity = childGameObject.GetComponent<MapCity>();
        MapManager mapMgr = childGameObject.GetComponent<MapManager>();
        Debug.Log("Mode " + mode.ToString());
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
                Debug.Log("Selection " + selection.ToString());
                switch (selection)
                {
                    case Selection.None:
                        // act based on the object over which mouse is now
                        if (mapHero)
                        {
                            Debug.Log("Clicked on hero party on map");
                            // verify if this is player's hero
                            HeroParty heroParty = mapHero.linkedPartyTr.GetComponent<HeroParty>();
                            if (player.Faction == heroParty.GetFaction())
                            {
                                // highlighted hero belongs to player
                                // deselect previous hero if it was present
                                if (selectedHero)
                                {
                                    selectedHero.SetSelectedState(false);
                                }
                                // select this hero
                                selection = Selection.PlayerHero;
                                SetSelectedHero(mapHero);
                                mapHero.SetSelectedState(true);
                            }
                            else
                            {
                                // highlighted hero is from other faction
                                // do nothing, because we did not select our hero which can attack it
                            }
                        }
                        if (mapCity)
                        {
                            Debug.Log("Clicked on city on map");
                            // verify if this is player's city
                            City city = mapCity.linkedCityTr.GetComponent<City>();
                            if (player.Faction == city.GetFaction())
                            {
                                // highlighted city belongs to player
                                // deselect previous city if it was selected
                                if (selectedCity)
                                {
                                    selectedCity.SetSelectedState(false);
                                }
                                // select this city
                                SetSelectedCity(mapCity);
                                selection = Selection.PlayerCity;
                                mapCity.SetSelectedState(true);
                            }
                            else
                            {
                                // highlighted city is from other faction
                                // do nothing
                            }
                        }
                        break;
                    case Selection.PlayerHero:
                        // verify if this hero was already selected
                        StartCoroutine(Move());
                        break;
                    case Selection.PlayerCity:
                        if (mapCity)
                        {
                            // verify if this is player's city
                            City city = mapCity.linkedCityTr.GetComponent<City>();
                            if (player.Faction == city.GetFaction())
                            {
                                Debug.LogWarning("1");
                                // highlighted city belongs to player
                                // verify it it is the same city as already selected
                                if (selectedCity.GetInstanceID() == mapCity.GetInstanceID())
                                {
                                    Debug.LogWarning("2");
                                    // same city as selected
                                    // Enter city edit mode
                                    mapCity.EnterCityEditMode();
                                }
                                else
                                {
                                    Debug.LogWarning("3");
                                    // other city
                                    // select other city
                                    // deselect previous city if it was selected
                                    if (selectedCity)
                                    {
                                        selectedCity.SetSelectedState(false);
                                    }
                                    // select this city
                                    SetSelectedCity(selectedCity);
                                    selection = Selection.PlayerCity;
                                    mapCity.SetSelectedState(true);
                                }
                            }
                            else
                            {
                                // highlighted city is from other faction
                                // remove selection
                                selection = Selection.None;
                                if (selectedCity)
                                {
                                    selectedCity.SetSelectedState(false);
                                }
                                if (selectedHero)
                                {
                                    selectedHero.SetSelectedState(false);
                                }
                            }
                        }
                        if (mapMgr)
                        {
                            // remove selection
                            selection = Selection.None;
                            if (selectedCity)
                            {
                                selectedCity.SetSelectedState(false);
                            }
                            if (selectedHero)
                            {
                                selectedHero.SetSelectedState(false);
                            }
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

}
