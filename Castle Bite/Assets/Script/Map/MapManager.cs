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
        Selection,
        HighlightMovePath,
        Move,
        EnterHeroEdit,
        EnterCity,
        EnterBattle,
        EnterCastSpell
    };
    Mode mode;
    Vector3 startPosition;
    // Transform startParent;
    [SerializeField]
    int tileSize = 16;
    Transform tileHighlighterTr;
    TileHighlighter tileHighlighter;
    // for pathfinding
    int tileMapWidth = 60;
    int tileMapHeight = 60;
    MapHero selectedHero;
    NesScripts.Controls.PathFind.Grid grid;
    List<NesScripts.Controls.PathFind.Point> movePath;
    GameObject[,] tileHighlighters;
    // for path highlighting
    public enum TileOccupiedBy {
        None,
        ImpassableTerrain,
        SelectedParty,
        PlayerParty,
        EnemyParty,
        AlliedParty,
        Treasure,
        PlayerCity,
        EnemyCity
    }
    // for hero moving
    public float heroMoveSpeed = 10.1f;
    public float heroMoveSpeedDelay = 0.1f;


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

    //
    // Vector3 mouseOnDragStartPosition;
    Vector3 mouseOnDownStartPosition;
    float xCorrectionOnDragStart;
    float yCorrectionOnDragStart;

    public Mode GetMode()
    {
        return mode;
    }

    public void SetSelectedHero(MapHero sltdHero)
    {
        selectedHero = sltdHero;
        // enter HighlightMovePath mode
        mode = Mode.HighlightMovePath;
    }

    void Start()
    {
        // Init map mode
        mode = Mode.Browse;
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
        tileHighlighter.OnChange(); // set highlighter according to the map mode;
        // Initialize path finder
        InitPathFinder();
        InitializePathHighligters();
    }

    void InitPathFinder()
    {
        // create the tiles map
        bool[,] tilesmap = new bool[tileMapWidth, tileMapHeight];
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
                    break;
                case Mode.HighlightMovePath:
                    // update tile highliter position
                    UpdateTileHighlighterToMousePoistion();
                    FindAndHighlightPath();
                    break;
                case Mode.Move:
                    // Move();
                    // do nothing, wait for move to finish
                    break;
                default:
                    Debug.LogError("Unknown mode");
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

    TileOccupiedBy GetTileOccupationState(NesScripts.Controls.PathFind.Point pathPoint)
    {
        TileOccupiedBy result = TileOccupiedBy.None;
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
                if (GetTilePosition(enemyParty.transform) == tilePosition)
                {
                    return TileOccupiedBy.EnemyParty;
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
                    return TileOccupiedBy.PlayerParty;
                }
            }
        }
        foreach (PlayerCityOnMap enemyParty in transform.GetComponentsInChildren<PlayerCityOnMap>())
        {
            // verify if not null
            if (enemyParty)
            {
                if (GetTilePosition(enemyParty.transform) == tilePosition)
                {
                    return TileOccupiedBy.PlayerCity;
                }
            }
        }
        return result;
    }

    void HighlightLastPathTile()
    {
        // highligh the last tile depending on what is under cursor
        // the might be another hero or enemy party or treasure chest or ally party
        NesScripts.Controls.PathFind.Point lastPathTile = movePath[movePath.Count - 1];
        switch (GetTileOccupationState(lastPathTile))
        {
            case TileOccupiedBy.None:
                // nothing to do
                break;
            case TileOccupiedBy.SelectedParty:
                // this should not be possible, because move path in this case is 0
                Debug.LogError("Not possible condition");
                break;
            case TileOccupiedBy.PlayerParty:
                // select other party
                // highlight blue
                tileHighlighters[lastPathTile.x, lastPathTile.y].GetComponentInChildren<Text>().color = Color.blue;
                break;
            case TileOccupiedBy.PlayerCity:
                // select city
                // highlight yellow
                tileHighlighters[lastPathTile.x, lastPathTile.y].GetComponentInChildren<Text>().color = Color.yellow;
                break;
            case TileOccupiedBy.EnemyParty:
                // highlight red
                tileHighlighters[lastPathTile.x, lastPathTile.y].GetComponentInChildren<Text>().color = Color.red;
                break;
        }
    }

    void HighlightMovePath(bool doHighlight)
    {
        // todo: it is better to make them transparant, then instantiate new and destroy each time
        if (movePath != null && movePath.Count > 0)
        {
            foreach (var pathPoint in movePath)
            {
                // output path to debug
                // Debug.Log("Path point is [" + pathPoint.x + "]:[" + pathPoint.y + "]");
                tileHighlighters[pathPoint.x, pathPoint.y].SetActive(doHighlight);
            }
            HighlightLastPathTile();
        }
    }

    void FindAndHighlightPath()
    {
        // First remove highlight from previous path
        HighlightMovePath(false);
        // create source and target points
        NesScripts.Controls.PathFind.Point _from = new NesScripts.Controls.PathFind.Point(GetHeroTilePosition().x, GetHeroTilePosition().y);
        // Debug.Log("From [" + _from.x + "]:[" + _from.y + "]");
        NesScripts.Controls.PathFind.Point _to = new NesScripts.Controls.PathFind.Point(GetTileHighlighterPosition().x, GetTileHighlighterPosition().y);
        // Debug.Log("To [" + _to.x + "]:[" + _to.y + "]");
        // get path
        // path will either be a list of Points (x, y), or an empty list if no path is found.
        movePath = NesScripts.Controls.PathFind.Pathfinding.FindPath(grid, _from, _to);
        // for Manhattan distance
        // List<NesScripts.Controls.PathFind.Point> path = NesScripts.Controls.PathFind.Pathfinding.FindPath(grid, _from, _to, NesScripts.Controls.Pathfinding.DistanceType.Manhattan);
        // highlight new path
        HighlightMovePath(true);
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
        mode = Mode.Drag;
        // update tileHighlighter
        tileHighlighter.OnChange();
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
        mode = Mode.Browse;
        // update tile highlighter
        tileHighlighter.OnChange();
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
        Debug.Log("Remaining distance is [" + result.ToString() + "]");
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

    IEnumerator Move()
    {
        Debug.Log("Move");
        float deltaTime;
        float previousTime = Time.time;
        if (movePath != null)
        {
            foreach (var pathPoint in movePath)
            {
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
            }
        }
        // exit move state and enter HighlightMovePath
        mode = Mode.HighlightMovePath;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // act based on current mode
        switch (mode)
        {
            case Mode.Browse:
                // verify if we can enter selection mode
                // if we can, then update tile highlighter
                mode = Mode.Selection;
                tileHighlighter.OnChange();
                break;
            case Mode.Selection:
                // verify if we should
                //  - highlight move path for selected unit 
                //  - select other unit
                //  - ...
                // if we can, then update tile highlighter
                mode = Mode.Selection;
                // tileHighlighter.OnChange();
                break;
            case Mode.HighlightMovePath:
                // enter move mode
                mode = Mode.Move;
                StartCoroutine(Move());
                break;
            default:
                Debug.LogError("Unknown mode");
                break;
        }
        

    }

}
