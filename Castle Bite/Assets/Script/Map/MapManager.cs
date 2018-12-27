using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[Serializable]
//public struct PositionOnMap
//{
//    public float offsetMinX;
//    public float offsetMinY;
//    public float offsetMaxX;
//    public float offsetMaxY;

//    public static bool operator ==(PositionOnMap p1, PositionOnMap p2)
//    {
//        return (p1.offsetMinX.Equals(p2.offsetMinX)
//             && p1.offsetMinY.Equals(p2.offsetMinY)
//             && p1.offsetMaxX.Equals(p2.offsetMaxX)
//             && p1.offsetMaxY.Equals(p2.offsetMaxY));
//    }

//    public static bool operator !=(PositionOnMap p1, PositionOnMap p2)
//    {
//        return (!p1.offsetMinX.Equals(p2.offsetMinX)
//             || !p1.offsetMinY.Equals(p2.offsetMinY)
//             || !p1.offsetMaxX.Equals(p2.offsetMaxX)
//             || !p1.offsetMaxY.Equals(p2.offsetMaxY));
//    }
//}

[Serializable]
public struct MapCoordinates
{
    public int x;
    public int y;
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

    public static MapManager Instance { get; private set; }

    [SerializeField]
    bool keepEnabledAfterStart;
    //[SerializeField]
    //Transform rootUITr;
    //[SerializeField]
    //MapFocusPanel mapFocusPanel;
    //[SerializeField]
    //Transform mapOptions;
    //[SerializeField]
    //GameMap lMap;
    [SerializeField]
    Transform fogOfWarTransform;
    [SerializeField]
    Transform mapSlicesTransform;
    [SerializeField]
    TileHighlighter tileHighlighter;
    [SerializeField]
    Transform mapHeroesParentTransformOnMap;
    [SerializeField]
    Transform mapCitiesParentTransformOnMap;
    [SerializeField]
    Transform mapItemsContainersParentTransformOnMap;
    [SerializeField]
    Transform mapLabelsParentTransformOnMap;
    [SerializeField]
    Mode mode;
    [SerializeField]
    Selection selection;
    [SerializeField]
    float yAdjustmentConstant;
    //Vector3 startPosition;
    // Transform startParent;
    [SerializeField]
    int tileSize = 16;
    [SerializeField]
    Color fogOfWarColor;
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
    // Transform tileHighlighterTr;
    Color tileHighlighterColor;
    // for pathfinding
    [SerializeField]
    GameObject movePathHighlighterTemplate;
    float[,] tilesmap;
    int tileMapWidth = 60;
    int tileMapHeight = 60;
    MapHero selectedMapHero;
    MapCity selectedCity;
    NesScripts.Controls.PathFind.Point selectedTargetPathPoint;
    NesScripts.Controls.PathFind.Grid grid;
    List<NesScripts.Controls.PathFind.Point> movePath;
    MapTile[,] mapTiles;
    Transform[,] fogOfWarTiles;
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
    //float xMinDef;
    //float xMaxDef;
    //float yMinDef;
    //float yMaxDef;
    // those are variables depending on the mouse onDragStart position
    //float xMin;
    //float xMax;
    //float yMin;
    //float yMax;
    // modifier for position based on slices rotations done
    // int rotationPositionModifier = 0;
    public enum Shift { Left, Right };
    // for debug
    Vector3 mousePosition;
    // Vector3 mapPosition;
    // Vector3 mouseOnDragStartPosition;
    // Vector3 mouseOnDownStartPosition;
    // float xCorrectionOnDragStart;
    //float yCorrectionOnDragStart;
    // for animation and transition between states
    CoroutineQueue queue;
    public float scrollSpeed = 0.5f;
    // for tiles creation
    [SerializeField]
    GameObject tileBackgroundTemplate;
    [SerializeField]
    GameObject tileTextTemplate;
    [SerializeField]
    string cityTerraText;
    [SerializeField]
    Color cityTerraColor;
    [SerializeField]
    string forestTerraText;
    [SerializeField]
    Color forestTerraColor;
    [SerializeField]
    string hillTerraText;
    [SerializeField]
    Color hillTerraColor;
    [SerializeField]
    string iceTerraText;
    [SerializeField]
    Color iceTerraColor;
    [SerializeField]
    string jungleTerraText;
    [SerializeField]
    Color jungleTerraColor;
    [SerializeField]
    string lakeTerraText;
    [SerializeField]
    Color lakeTerraColor;
    [SerializeField]
    string lavaTerraText;
    [SerializeField]
    Color lavaTerraColor;
    [SerializeField]
    string mountainTerraText;
    [SerializeField]
    Color mountainTerraColor;
    [SerializeField]
    string oceanTerraText;
    [SerializeField]
    Color oceanTerraColor;
    [SerializeField]
    string plainTerraText;
    [SerializeField]
    Color plainTerraColor;
    [SerializeField]
    string riverTerraText;
    [SerializeField]
    Color riverTerraColor;
    [SerializeField]
    string roadTerraText;
    [SerializeField]
    Color roadTerraColor;
    [SerializeField]
    string sandTerraText;
    [SerializeField]
    Color sandTerraColor;
    [SerializeField]
    string seaTerraText;
    [SerializeField]
    Color seaTerraColor;
    [SerializeField]
    string shoreTerraText;
    [SerializeField]
    Color shoreTerraColor;
    [SerializeField]
    string snowTerraText;
    [SerializeField]
    Color snowTerraColor;
    [SerializeField]
    string tundraTerraText;
    [SerializeField]
    Color tundraTerraColor;
    [SerializeField]
    string valleyTerraText;
    [SerializeField]
    Color valleyTerraColor;
    [SerializeField]
    string volcanoTerraText;
    [SerializeField]
    Color volcanoTerraColor;

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

    public int TileMapWidth
    {
        get
        {
            return tileMapWidth;
        }
    }

    public int TileMapHeight
    {
        get
        {
            return tileMapHeight;
        }
    }

    public int TileSize
    {
        get
        {
            return tileSize;
        }

        set
        {
            tileSize = value;
        }
    }

    public float YAdjustmentConstant
    {
        get
        {
            return yAdjustmentConstant;
        }

        set
        {
            yAdjustmentConstant = value;
        }
    }

    public TileHighlighter TileHighlighter
    {
        get
        {
            return tileHighlighter;
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
        // verify if instance is null = not set
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // verify if instance not is equal this = this is the new instance spawned
            if (Instance.GetInstanceID() != this.GetInstanceID())
            {
                Debug.LogError("Another instance should be destoryed before spawning this instance.");
                // destroy previous instance
                Destroy(Instance.gameObject);
                // init new instance
                Instance = this;
            }
        }
        // Create a coroutine queue that can run max 1 coroutine at once
        queue = new CoroutineQueue(1, StartCoroutine);
        // modify note: tile highliter is not referenced via serialized field
        // tileHighlighterTr = transform.Find("TileHighlighter");
        // tileHighlighter = tileHighlighterTr.GetComponent<TileHighlighter>();
        // disable it on startup
        gameObject.SetActive(keepEnabledAfterStart);
        // disable player income information on the top info panel
        // SetPlayerIncomeVisible(false);
    }

    public IEnumerator SetActive()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
    }

    void Start()
    {
        Debug.LogWarning("Starting Map Manager");
        // For map drag
        //  get map width and height
        // mapWidth = gameObject.GetComponentInChildren<SpriteRenderer>().size.x;
        // mapHeight = gameObject.GetComponentInChildren<SpriteRenderer>().size.y;
        mapWidth = 960;
        mapHeight = 960;
        //  calculate screen borders
        //  get maximum possible offset
        //float xDeltaMax = (mapWidth - Screen.width) / 2;
        //float yDeltaMax = (mapHeight - Screen.height) / 2;
        //  border depend on the center position
        //  because canvas is positioned in the lower left corner
        //  mouse's 0:0 coordinates are located at the same position
        //  center is screen width and height divided by 2
        //xMinDef = (Screen.width / 2) - xDeltaMax;
        //yMinDef = (Screen.height / 2) - yDeltaMax;
        //xMaxDef = (Screen.width / 2) + xDeltaMax;
        //yMaxDef = (Screen.height / 2) + yDeltaMax;
        //xMinDef = -mapWidth + Screen.width;
        //xMaxDef = 0;
        //yMinDef = -mapHeight + Screen.height + tileSize;
        //yMaxDef = -tileSize;
        // For map tile highligter in selection mode
        // Init map mode
        SetMode(Mode.Browse);
        // Initialize path finder
        InitializeMapTiles();
        InitFogOfWarTiles();
        InitTilesMap();
        InitPathFinderGrid();
    }

    void SetMapObjectAlwaysOn(MapObject mapObject, bool doShow)
    {
        // mapObject.SetAlwaysOn(doShow);
        // verify if we need to enable it or disable
        if (doShow)
        {
            // get tile coordinates
            Vector2Int tileCoordinates = GetTileByWorldPosition(mapObject.transform.position);
            // verify if it is not under fog of war = verify if this tile is not discovered
            if (TurnsManager.Instance.GetActivePlayer().TilesDiscoveryState[tileCoordinates.x, tileCoordinates.y] == 0)
            {
                // under the for of war
                // set only flag, but don't change color
                mapObject.LabelAlwaysOn = doShow;
            }
            else
            {
                // tile was discovered
                // Set always on flag and make label visible
                mapObject.SetAlwaysOn(doShow);
            }
        }
        else
        {
            // disable always on and hide label
            mapObject.SetAlwaysOn(doShow);
        }
    }

    // called via Unity Editor
    public void SetCitiesNamesVisible(bool doShow)
    {
        Debug.Log("Show all cities names: " + doShow.ToString());
        //MapObject mapObject;
        foreach (MapCity mapCity in transform.GetComponentsInChildren<MapCity>(true))
        {
            // Set always on
            SetMapObjectAlwaysOn(mapCity.GetComponent<MapObject>(), doShow);
        }
        // Save menu options
        MapMenuManager.Instance.SaveCitiesNamesToggleOptions();
    }

    // called via Unity Editor
    public void SetHeroesNamesVisible(bool doShow)
    {
        Debug.Log("Show all heroes names: " + doShow.ToString());
        //MapObject mapObject;
        foreach (MapHero mapHero in transform.GetComponentsInChildren<MapHero>(true))
        {
            SetMapObjectAlwaysOn(mapHero.GetComponent<MapObject>(), doShow);
        }
        // Save menu options
        MapMenuManager.Instance.SaveHeroesNamesToggleOptions();
    }

    // called via Unity Editor
    public void SetManaSourcesVisible(bool doShow)
    {
        Debug.Log("Show All Mana sources names visible: " + doShow.ToString());
        // Save menu options
        MapMenuManager.Instance.SaveManaSourcesToggleOptions();
    }

    // called via Unity Editor
    public void SetTreasureChestsVisible(bool doShow)
    {
        Debug.Log("Show All Treasure chests names visible: " + doShow.ToString());
        foreach (MapItemsContainer mapItem in transform.GetComponentsInChildren<MapItemsContainer>(true))
        {
            // Set always on
            SetMapObjectAlwaysOn(mapItem.GetComponent<MapObject>(), doShow);
        }
        // Save menu options
        MapMenuManager.Instance.SaveTreasureChestsToggleOptions();
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

    List<Vector2Int> DiscoverTilesAround(Vector2Int centerTileCoords, int discoveryRange)
    {
        // init discovered tiles list
        List<Vector2Int> discoveredTiles = new List<Vector2Int>();
        // get active player tiles discovery state array
        int[,] activePlayerTilesDiscoveryStateArray;
        if (TurnsManager.Instance.GetActivePlayer().TilesDiscoveryState != null)
        {
            activePlayerTilesDiscoveryStateArray = TurnsManager.Instance.GetActivePlayer().TilesDiscoveryState;
        }
        else
        {
            TurnsManager.Instance.GetActivePlayer().TilesDiscoveryState = new int[TileMapWidth, TileMapHeight];
            activePlayerTilesDiscoveryStateArray = TurnsManager.Instance.GetActivePlayer().TilesDiscoveryState;
        }

        // initialize tiles positions
        int checkX, checkY;
        // get all tiles around
        for (int x = -discoveryRange; x <= discoveryRange; x++)
        {
            for (int y = -discoveryRange; y <= discoveryRange; y++)
            {
                // verify if we are within discovery range
                // if ((Mathf.Abs(x - y) < discoveryRange + 2) && (Mathf.Abs(y + x) < discoveryRange + 2))
                // if (Math.Sqrt(x*x + y*y) <= discoveryRange)
                if (Mathf.Sqrt(x*x + y*y) <= (float)(discoveryRange+1))
                {
                    // calculate checked tiles coordinates
                    checkX = centerTileCoords.x + x;
                    checkY = centerTileCoords.y + y;
                    // verify if we are over edges
                    if (checkX < 0)
                    {
                        checkX = tileMapWidth - (-checkX);
                    }
                    else if (checkX == tileMapWidth)
                    {
                        checkX = 0;
                    }
                    else if (checkX > tileMapWidth)
                    {
                        checkX -= tileMapWidth;
                    }
                    if (checkY < 0)
                    {
                        checkY = tileMapHeight - (-checkY);
                    }
                    else if (checkY == tileMapHeight)
                    {
                        checkY = 0;
                    }
                    //Debug.Log(checkX + ":" + checkY);
                    // verify if this tile is not discovered
                    if (activePlayerTilesDiscoveryStateArray[checkX, checkY] == 0)
                    {
                        // mark tile as discovered (not 0)
                        activePlayerTilesDiscoveryStateArray[checkX, checkY] = 1;
                        // add discovered tile position to the list
                        discoveredTiles.Add(new Vector2Int(checkX, checkY));
                    }
                }
            }
        }
        // return list with all discovered tiles
        return discoveredTiles;
    }

    List<Vector2Int> DiscoverTilesAround(MapCity mapCity)
    {
        // get city discovery range
        int cityDiscoveryRange = 3 + mapCity.LCity.CityLevelCurrent;
        // discover tiles around
        return DiscoverTilesAround(GetTileByWorldPosition(mapCity.transform.position), cityDiscoveryRange);
    }

    List<Vector2Int> DiscoverTilesAround(MapHero mapHero)
    {
        // get hero discovery range
        int heroDiscoveryRange = mapHero.LHeroParty.GetPartyLeader().ScoutingRange;
        // discover tiles around
        return DiscoverTilesAround(GetTileByWorldPosition(mapHero.transform.position), heroDiscoveryRange);
    }

    void DiscoverTile(int x, int y)
    {
        // enable tile
        mapTiles[x, y].gameObject.SetActive(true);
        // disable fog of war tile
        fogOfWarTiles[x, y].gameObject.SetActive(false);
        //// get map object label on tile
        //MapObjectLabel mapObjectLabel = GetMapObjectLabelOnTile(new Vector2Int(x, y));
        //// verify if map object label is not null
        //if (mapObjectLabel != null)
        //{
        //    // verify if map object has always on flag
        //    if (mapObjectLabel.MapObject.LabelAlwaysOn)
        //    {
        //        mapObjectLabel.SetAlwaysOnLabelColor();
        //    }
        //}
    }

    void HideTile(int x, int y)
    {
        // disable tile
        mapTiles[x, y].gameObject.SetActive(false);
        // enable fog of war tile
        fogOfWarTiles[x, y].gameObject.SetActive(true);
        //// get map object label on tile
        //MapObjectLabel mapObjectLabel = GetMapObjectLabelOnTile(new Vector2Int(x, y));
        //// verify if map object label is not null
        //if (mapObjectLabel != null)
        //{
        //    mapObjectLabel.HideLabel();
        //}
    }

    MapObject GetMapObjectOnTile(Vector2Int tileCoords)
    {
        // get object on tile
        GameObject objectOnTile = GetObjectOnTile(tileCoords);
        // verify if object is not null
        if (objectOnTile != null)
        {
            // return map object
            return objectOnTile.GetComponent<MapObject>();
        }
        // default
        return null;
    }

    MapObjectLabel GetMapObjectLabelOnTile(Vector2Int tileCoords)
    {
        // get map object
        MapObject mapObject = GetMapObjectOnTile(tileCoords);
        // verify if object is not null
        if (mapObject != null)
        {
            // return map object label
            return mapObject.Label;
        }
        // default
        return null;
    }

    void InitFogOfWarTiles()
    {
        // verify if fog of war does not exist yet
        if (fogOfWarTiles == null)
        {
            // Create fog of war
            fogOfWarTiles = new Transform[tileMapWidth, tileMapHeight];
            // activate fog of war
            fogOfWarTransform.gameObject.SetActive(true);
            // init slice index
            int sliceIndex = 0;
            // loop though all slices in fog of war
            foreach (Transform slice in fogOfWarTransform)
            {
                // init tile index
                int tileIndex = 0;
                // loop through all tiles in the fog
                foreach (Transform tileTransform in slice)
                {
                    // set fog of war image color
                    tileTransform.GetComponent<RawImage>().color = fogOfWarColor;
                    // save fog of war tile reference
                    fogOfWarTiles[sliceIndex, tileIndex] = tileTransform;
                    // increment tile index
                    tileIndex++;
                }
                // increment index
                sliceIndex++;
            }
        }
    }

    public void InitTilesMap()
    {
        // create the tiles map
        tilesmap = new float[tileMapWidth, tileMapHeight];
        // init tiles map based on the Terra info
        // true = walkable, false = blocking
        for (int x = 0; x < tilesmap.GetLength(0); x += 1)
        {
            for (int y = 0; y < tilesmap.GetLength(1); y += 1)
            {
                // Debug.Log(x.ToString() + ":" + y.ToString());
                // verify if tile is passable
                if (mapTiles[x, y].Terra.TerraIsPassable)
                {
                    tilesmap[x, y] = (float)mapTiles[x, y].Terra.TerraMoveCost;
                }
                else
                {
                    // mark is as non-passable
                    tilesmap[x, y] = 0;
                }
            }
        }
        // Get active player faction
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        // Adjust map to active player if it exists (it may not exist during game load)
        if (activePlayer != null)
        {
            // Loop over all map objects
            foreach (MapObject mapObject in transform.GetComponentsInChildren<MapObject>())
            {
                // get map object position with tile coordinates
                Vector2Int pos = GetTilePosition(mapObject.transform);
                // verify if object is located within tile map border
                if (PositionIsWithinTilesMap(pos))
                {
                    // verify if map object is city
                    if (mapObject.GetComponent<MapCity>() != null)
                    {
                        // verify if city has the same faction as the active player (belongs to active player)
                        if (mapObject.GetComponent<MapCity>().LCity.CityFaction == activePlayer.Faction)
                        // verify if city is not occupied
                        // Todo: fix bug when trying to pass through occupied city
                        {
                            // mark tile as passable
                            tilesmap[pos.x, pos.y] = 1;
                            // discover tiles around
                            DiscoverTilesAround(mapObject.GetComponent<MapCity>());
                        }
                        else
                        {
                            // mark tile as non-passable
                            tilesmap[pos.x, pos.y] = 0;
                        }
                    }
                    else
                    {
                        // mark tile as non-passable
                        tilesmap[pos.x, pos.y] = 0;
                        // verify if it hero on map
                        if (mapObject.GetComponent<MapHero>() != null)
                        {
                            // verify if hero has the same faction as the active player (belongs to active player)
                            if (mapObject.GetComponent<MapHero>().LHeroParty.Faction == activePlayer.Faction)
                            {
                                // discover tiles around
                                DiscoverTilesAround(mapObject.GetComponent<MapHero>());
                            }
                        }
                    }
                }
                // Activate/deactivate labels for discovered and hidden tiles
                // verify if map object has always on flag
                if (mapObject.LabelAlwaysOn)
                {
                    // get map object label on tile
                    MapObjectLabel mapObjectLabel = mapObject.Label;
                    // verify if map object label is not null
                    if (mapObjectLabel != null)
                    {
                        // verify if it is under fog of war = verify if this tile is discovered
                        if (TurnsManager.Instance.GetActivePlayer().TilesDiscoveryState[pos.x, pos.y] != 0)
                        {
                            // verify if mouse is not over the map object or its label at this moment
                            if (!mapObject.IsMouseOver && !mapObjectLabel.IsMouseOver)
                            {
                                // tile was discovered
                                // make label visible
                                mapObjectLabel.SetAlwaysOnLabelColor();
                            }
                        }
                    }
                }
            }
            // get active player tiles discovery state array
            int[,] activePlayerTilesDiscoveryStateArray = activePlayer.TilesDiscoveryState;
            // Activate discovered tiles
            for (int x = 0; x < tilesmap.GetLength(0); x += 1)
            {
                for (int y = 0; y < tilesmap.GetLength(1); y += 1)
                {
                    // Debug.Log(x.ToString() + ":" + y.ToString());
                    // verify if tile has been discovered
                    if (activePlayerTilesDiscoveryStateArray[x, y] != 0)
                    {
                        DiscoverTile(x, y);
                    }
                    else
                    {
                        HideTile(x, y);
                    }
                }
            }
        }
    }

    void InitPathFinderGrid()
    {
        // create a grid
        grid = new NesScripts.Controls.PathFind.Grid(tilesmap);
    }

    bool ValidateOutOfScreenMouse(Vector2Int tileCoords)
    {
        if ((tileCoords.x > grid.nodes.GetLength(0) - 1)
            || (tileCoords.y > grid.nodes.GetLength(1) - 1)
            || (tileCoords.x < 0)
            || (tileCoords.y < 0))
        {
            // nothing to do, mouse is over the screen
            return false;
        }
        return true;
    }

    bool ValidateOutOfScreenMouse(NesScripts.Controls.PathFind.Point pathPoint)
    {
        return ValidateOutOfScreenMouse(new Vector2Int(pathPoint.x, pathPoint.y));
    }

    GameObject GetFogOnTile(Vector2Int tilePosition)
    {
        // loop over all active fog of war tiles
        foreach (MapFogOfWar fogObject in fogOfWarTransform.GetComponentsInChildren<MapFogOfWar>(false))
        {
            // verify if there is a fog on tile which we are searching
            // if fog is not active (disabled), then it will not be checked
            if (GetTilePosition(fogObject.transform) == tilePosition)
            {
                return fogObject.gameObject;
            }
        }
        return null;
    }

    GameObject GetFogOnTile(NesScripts.Controls.PathFind.Point pathPoint)
    {
        Vector2Int tilePosition = new Vector2Int
        {
            x = pathPoint.x,
            y = pathPoint.y
        };
        return GetFogOnTile(tilePosition);
    }

    void UpdateGridBasedOnHighlightedTile()
    {
        // reinitialize tile map to reset all previous changes
        InitTilesMap();
        // Get highligted tile state
        Vector2Int highlighterPosition = GetTileByWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)); // GetTileHighlighterPosition();
        NesScripts.Controls.PathFind.Point highlightedPoint = new NesScripts.Controls.PathFind.Point(highlighterPosition.x, highlighterPosition.y);
        if (!ValidateOutOfScreenMouse(highlightedPoint))
        {
            // nothing to do, mouse is over the screen
        }
        else
        {
            if (GetFogOnTile(highlightedPoint))
            {
                // make tile impassable
                tilesmap[highlightedPoint.x, highlightedPoint.y] = 0;
            }
            else if (GetObjectOnTile(highlightedPoint))
            {
                // some object is present
                // adjust grid to make this tile passable (highlightable)
                // this is needed to get move path if user clicked on a occupied tile
                // but this is not needed to be passable if user clicked on other tile
                // make tile passable
                tilesmap[highlightedPoint.x, highlightedPoint.y] = 1;
            }
            else
            {
                // no object on tile or fog, just terrain
            }
        }
        // Upgrade grid
        grid.UpdateGrid(tilesmap);
    }


    //void SetTileHighlighterToMousePoistion()
    //{
    //    // get mouse position
    //    // Vector3 mousePosition = Camera.main.WorldToViewportPoint(Input.mousePosition);
    //    // get Y coords adjustments
    //    float yAdjustment = Camera.main.transform.position.y;
    //    // get the remaining adjustment
    //    while (yAdjustment >= tileSize)
    //    {
    //        yAdjustment -= tileSize;
    //    }
    //    // Debug.Log("y adjustment " + yAdjustment);
    //    // Update position
    //    int x = Mathf.FloorToInt(Input.mousePosition.x / tileSize);
    //    int y = Mathf.FloorToInt((Input.mousePosition.y + yAdjustment + yAdjustmentConstant) / tileSize);
    //    // tileHighlighterTr.position = new Vector3(x, y, 0) * tileSize;
    //    tileHighlighter.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0) * tileSize - new Vector3(0, yAdjustment + yAdjustmentConstant, 0));
    //    // Debug.Log("Tile: " + x + ";" + y);
    //}

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
                MapObjectLabel label = mapHero.GetComponent<MapObject>().Label;
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
                MapObjectLabel label = mapCity.GetComponent<MapObject>().Label;
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
        // Debug.Log("mouse: " + Input.mousePosition);
        // Vector3 pointerPositionInWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Debug.Log("trans: " + pointerPositionInWorldSpace);
        // Vector2Int highlighterPosition = GetTileByPosition(pointerPositionInWorldSpace);
        Vector2Int highlighterPosition = GetTileByWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
                    // verify if mouse is over fog of war
                    if (GetFogOnTile(highlightedPoint))
                    {
                        // hide tile highligter
                        tileHighlighterColor = new Color32(0, 0, 0, 0);
                    }
                    else
                    {
                        // predefine colors
                        Color32 darkBlue = new Color32(0, 0, 128, 255);
                        Color32 darkCyan = new Color32(0, 128, 128, 255);
                        Color32 darkYellow = new Color32(128, 122, 0, 255);
                        Color32 darkRed = new Color32(128, 0, 0, 255);
                        // Update tile highlighter position
                        tileHighlighter.SetToMousePoistion();
                        // set default tile highlighter color
                        tileHighlighterColor = Color.white;
                        // get game object below the tile highlighter
                        GameObject childGameObject = GetObjectOnTile(highlightedPoint); // return map's objects: hero, city, other..
                        MapHero mapHero = null;
                        MapCity mapCity = null;
                        MapItemsContainer mapItem = null;
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
                            mapItem = childGameObject.GetComponent<MapItemsContainer>();
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
                            if (IsTileProtected(GetTileByWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition))))
                            {
                                // CursorController.Instance.SetAttackCursor();
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
                                //CursorController.Instance.SetNormalCursor();
                                //tileHighlighterColor = Color.white;
                                // get tile coordinates
                                Vector2Int tileCoords = GetTileByWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                                // Verify if tile coords are correct
                                if (ValidateOutOfScreenMouse(tileCoords))
                                {
                                    // Get tile
                                    MapTile mapTile = mapTiles[tileCoords.x, tileCoords.y];
                                    // verify if tile is not passable
                                    if (!mapTile.Terra.TerraIsPassable)
                                    {
                                        // make it gray, indicating that this is not passable
                                        tileHighlighterColor = Color.gray;
                                    }
                                }
                                else
                                {
                                    // Mouse is not over screen
                                    Debug.LogWarning("Mouse not over screen. Hide tile highlighter");
                                    // Hide tile highlighter in this mode
                                    tileHighlighterColor = new Color32(0, 0, 0, 0);
                                }
                            }
                        }
                    }
                    // Update color of tile highlighter
                    tileHighlighter.SetColor(tileHighlighterColor);
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
        //// verify if mouse is moving
        //if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
        //{
            switch (mode)
            {
                case Mode.Browse:
                    // update tile highliter position
                    UpdateGridBasedOnHighlightedTile();
                    UpdateTileHighighterBasedOnMousePosition();
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
        //}
    }

    int GetShiftedTilePositionX(int defaultPositionX)
    {
        int shiftedTilePositionX = defaultPositionX - mapShift;
        // verify if we are not ouside of the grid borders
        if (shiftedTilePositionX >= tileMapWidth)
        {
            // reset it
            shiftedTilePositionX -= tileMapWidth;
        }
        // verify if it is not less than 0
        else if (shiftedTilePositionX < 0)
        {
            // reset it
            shiftedTilePositionX += tileMapWidth;
        }
        return shiftedTilePositionX;
    }

    public Vector2Int GetTileByWorldPosition(Vector3 position)
    {
        // calculate tile position base on object position and map position
        return new Vector2Int
        {
            x = GetShiftedTilePositionX(Mathf.FloorToInt((position.x - transform.position.x) / tileSize)),
            y = Mathf.FloorToInt((position.y - transform.position.y) / tileSize)
        };
    }

    public Vector3 GetWorldPositionByCoordinates(MapCoordinates coordinates)
    {
        // get tile position by its coordinates
        return mapTiles[coordinates.x, coordinates.y].transform.position;
    }

    public MapCoordinates GetCoordinatesByWorldPosition(Vector3 position)
    {
        // calculate tile position base on object position and map position
        Vector2Int tileID = GetTileByWorldPosition(position);
        return new MapCoordinates
        {
            x = tileID.x,
            y = tileID.y
        };
    }

    //public Vector3 GetPositionByTile(int tileX, int tileY)
    //{
    //    return new Vector3
    //    {
    //        x = tileX * tileSize,
    //        y = tileY * tileSize,
    //        z = 0
    //    };
    //}

    Vector2Int GetTilePosition(Transform tr)
    {
        return GetTileByWorldPosition(tr.position);
        //return new Vector2Int
        //{
        //    x = Mathf.FloorToInt(tr.position.x / tileSize),
        //    y = Mathf.FloorToInt(tr.position.y / tileSize)
        //};
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

    string GetTileTextByTerra(TerraType terraType)
    {
        switch (terraType)
        {
            case TerraType.City:
                return cityTerraText;
            case TerraType.Forest:
                return forestTerraText;
            case TerraType.Hill:
                return hillTerraText;
            case TerraType.Ice:
                return iceTerraText;
            case TerraType.Jungle:
                return jungleTerraText;
            case TerraType.Lake:
                return lakeTerraText;
            case TerraType.Lava:
                return lavaTerraText;
            case TerraType.Mounain:
                return mountainTerraText;
            case TerraType.Ocean:
                return oceanTerraText;
            case TerraType.Plain:
                return plainTerraText;
            case TerraType.River:
                return riverTerraText;
            case TerraType.Road:
                return roadTerraText;
            case TerraType.Sand:
                return sandTerraText;
            case TerraType.Sea:
                return seaTerraText;
            case TerraType.Shore:
                return shoreTerraText;
            case TerraType.Snow:
                return snowTerraText;
            case TerraType.Tundra:
                return tundraTerraText;
            case TerraType.Valley:
                return valleyTerraText;
            case TerraType.Volcano:
                return volcanoTerraText;
            default:
                Debug.LogError("Unknown terrain type");
                return "e";
        }
    }

    Color GetTileColorByTerra(TerraType terraType)
    {
        switch (terraType)
        {
            case TerraType.City:
                return cityTerraColor;
            case TerraType.Forest:
                return forestTerraColor;
            case TerraType.Hill:
                return hillTerraColor;
            case TerraType.Ice:
                return iceTerraColor;
            case TerraType.Jungle:
                return jungleTerraColor;
            case TerraType.Lake:
                return lakeTerraColor;
            case TerraType.Lava:
                return lavaTerraColor;
            case TerraType.Mounain:
                return mountainTerraColor;
            case TerraType.Ocean:
                return oceanTerraColor;
            case TerraType.Plain:
                return plainTerraColor;
            case TerraType.River:
                return riverTerraColor;
            case TerraType.Road:
                return roadTerraColor;
            case TerraType.Sand:
                return sandTerraColor;
            case TerraType.Sea:
                return seaTerraColor;
            case TerraType.Shore:
                return shoreTerraColor;
            case TerraType.Snow:
                return snowTerraColor;
            case TerraType.Tundra:
                return tundraTerraColor;
            case TerraType.Valley:
                return valleyTerraColor;
            case TerraType.Volcano:
                return volcanoTerraColor;
            default:
                Debug.LogError("Unknown terrain type");
                return Color.red;
        }
    }

    Color GetTileBackgroundColorByTerra(TerraType terraType)
    {
        switch (terraType)
        {
            case TerraType.Ice:
                return new Color(0.1f,0.1f,0.1f);
            case TerraType.City:
            case TerraType.Forest:
            case TerraType.Hill:
            case TerraType.Jungle:
            case TerraType.Lake:
            case TerraType.Lava:
            case TerraType.Mounain:
            case TerraType.Ocean:
            case TerraType.Plain:
            case TerraType.River:
            case TerraType.Road:
            case TerraType.Sand:
            case TerraType.Sea:
            case TerraType.Shore:
            case TerraType.Snow:
            case TerraType.Tundra:
            case TerraType.Valley:
                return Color.black;
            case TerraType.Volcano:
                return new Color(0.9f,0.13f,0,1f);
            default:
                Debug.LogError("Unknown terrain type");
                return Color.red;
        }
    }

    void InitMapTile(MapTile mapTile)
    {
        // create background
        RawImage tileBackgroundRawImage = Instantiate(tileBackgroundTemplate, mapTile.transform).GetComponent<RawImage>();
        // Set background color
        tileBackgroundRawImage.color = GetTileBackgroundColorByTerra(mapTile.Terra.TerraType);
        // send background to back
        tileBackgroundRawImage.transform.SetAsFirstSibling();
        // enable background
        tileBackgroundRawImage.gameObject.SetActive(true);
        // create tile text
        Text tileText = Instantiate(tileTextTemplate, mapTile.transform).GetComponent<Text>();
        // set text
        tileText.text = GetTileTextByTerra(mapTile.Terra.TerraType);
        // set text color
        tileText.color = GetTileColorByTerra(mapTile.Terra.TerraType);
        // enable text
        tileText.gameObject.SetActive(true);
        // disable tile highlighter
        mapTile.transform.Find("TileHighliter").gameObject.SetActive(false);
        // get turns info text
        Text turnsInfoText = mapTile.transform.Find("TurnsInfo").GetComponent<Text>();
        // clear text
        turnsInfoText.text = "";
        // set turns info text on top
        turnsInfoText.transform.SetAsLastSibling();
        // disable tile
        mapTile.gameObject.SetActive(false);
    }

    void InitializeMapTiles()
    {
        // todo: better keep them as string and highlight specific elements
        // Get map slices container
        // Transform mapSlicesContainer = transform.Find("MapSlices");
        // Transform mapTilesTransform = transform.Find("MapTiles");
        // Create map tiles array
        mapTiles = new MapTile[tileMapWidth, tileMapHeight];
        // init row and colum index variables
        // Vector2Int tileCoordinates;
        // convert mapTiles to 2dimentional array
        int i = 0;
        foreach(Transform childTransform in mapSlicesTransform)
        {
            MapTile[] _mapTiles = childTransform.GetComponentsInChildren<MapTile>(true);
            for (int j = _mapTiles.Length - 1; j >= 0; j--)
            {
                // assign tile object to mapTiles array
                mapTiles[i, j] = _mapTiles[j];
                // init map tile
                InitMapTile(_mapTiles[j]);
            }
            i++;
        }
        //foreach (MapTile mapTile in transform.Find("MapTiles").GetComponentsInChildren<MapTile>(true))
        //foreach (MapTile mapTile in mapSlicesContainer.GetComponentsInChildren<MapTile>(true))
        //{
        //    // get tile coordintates by tile object position
        //    tileCoordinates = GetTileByPosition(mapTile.transform.position);
        //    Debug.Log(tileCoordinates.x + ":" + tileCoordinates.y);
        //    // assign tile object to mapTiles array
        //    mapTiles[tileCoordinates.x, tileCoordinates.y] = mapTile;
        //    // move tile to respective map slice
        //    //mapTile.transform.SetParent(mapSlicesContainer.GetChild(tileCoordinates.x), false);
        //    // reset position x to -8
        //    //mapTile.transform.position = new Vector3(-8, mapTile.transform.position.y, 0);
        //    // rename tile
        //    //mapTile.name = "MapTile (" + tileCoordinates.y + ")";
        //    // disable game object
        //    // mapTile.gameObject.SetActive(false);
        //}
        //for (int x = 0; x < tileHighlighters.GetLength(0); x += 1)
        //{
        //    for (int y = 0; y < tileHighlighters.GetLength(1); y += 1)
        //    {
        //        // Debug.Log(tilesmap[x, y].ToString());
        //        // create new highlight object
        //        tileHighlighters[x, y] = Instantiate(movePathHighlighterTemplate, mapTilesTransform);
        //        tileHighlighters[x, y].transform.position = new Vector3(x, y, 0) * tileSize;
        //        // tileHighlighters[x, y].gameObject.SetActive(true);
        //    }
        //}
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

    GameObject GetObjectOnTile(Vector2Int tilePosition)
    {
        // loop over all map objects and verify if it is on the tile
        foreach (MapObject mapObject in transform.GetComponentsInChildren<MapObject>())
        {
            // verify if object is located on tile which we are searching
            if (GetTilePosition(mapObject.transform) == tilePosition)
            {
                return mapObject.gameObject;
            }
        }
        //// go over all objects and verify if they are on the tile
        //foreach (MapHero party in transform.GetComponentsInChildren<MapHero>())
        //{
        //    // verify if not null
        //    if (party)
        //    {
        //        Vector2Int partyTilePosition = GetTilePosition(party.transform);
        //        if (partyTilePosition == tilePosition)
        //        {
        //            return party.gameObject;
        //        }
        //    }
        //}
        //foreach (MapCity city in transform.GetComponentsInChildren<MapCity>())
        //{
        //    // verify if not null
        //    if (city)
        //    {
        //        Vector2Int cityTilePosition = GetTilePosition(city.transform);
        //        if (cityTilePosition == tilePosition)
        //        {
        //            return city.gameObject;
        //        }
        //    }
        //}
        //foreach (MapItemsContainer mapItem in transform.GetComponentsInChildren<MapItemsContainer>())
        //{
        //    // verify if not null
        //    if (mapItem)
        //    {
        //        Vector2Int itemTilePosition = GetTilePosition(mapItem.transform);
        //        if (itemTilePosition == tilePosition)
        //        {
        //            return mapItem.gameObject;
        //        }
        //    }
        //}
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
            MapItemsContainer mapItem = gameObjectOnTile.GetComponent<MapItemsContainer>();
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
            // initialize number of days to reach path point variable
            int numberOfDaysToReachThisPathPoint = 0;
            // Loop through each path point
            foreach (NesScripts.Controls.PathFind.Point pathPoint in movePath)
            {
                // output path to debug
                // Debug.Log("Path point is [" + pathPoint.x + "]:[" + pathPoint.y + "]");
                // mapTiles[pathPoint.x, pathPoint.y].gameObject.SetActive(doHighlight);
                mapTiles[pathPoint.x, pathPoint.y].transform.Find("TileHighliter").gameObject.SetActive(doHighlight);
                // clear turn info text (because is not reset automatically and may be still present)
                mapTiles[pathPoint.x, pathPoint.y].transform.Find("TurnsInfo").GetComponent<Text>().text = "";
                // verify if we need to highlight
                if (doHighlight == true)
                {
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
                            MapItemsContainer mapItem = gameObjectOnTile.GetComponent<MapItemsContainer>();
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
                        //// get the rest of division
                        //float restOfDivision = movePointsLeft % partyLeader.GetEffectiveMaxMovePoints();
                        //Debug.Log("Rest of division is " + restOfDivision);
                        //// verify if we has reached next day limit
                        //if (restOfDivision == 0)
                        //{
                        int newNumberOfDaysToReachThisPathPoint = Math.Abs(movePointsLeft / partyLeader.GetEffectiveMaxMovePoints()) + 1;
                        // verify if number of days to reach this path point has changed compare to previous value
                        if (newNumberOfDaysToReachThisPathPoint != numberOfDaysToReachThisPathPoint)
                        {
                            // Get number of days needed to reach this path point
                            numberOfDaysToReachThisPathPoint = Math.Abs(movePointsLeft / partyLeader.GetEffectiveMaxMovePoints()) + 1;
                            Debug.Log("numberOfDaysToReachThisPathPoint = " + numberOfDaysToReachThisPathPoint);
                            // add day indicator to the move path highter
                            mapTiles[pathPoint.x, pathPoint.y].transform.Find("TurnsInfo").GetComponent<Text>().text = numberOfDaysToReachThisPathPoint.ToString();
                        }
                        //}
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
                    mapTiles[pathPoint.x, pathPoint.y].transform.Find("TileHighliter").GetComponent<Text>().color = tileHighlighColor;
                    // reduce number of move points left based on the tile move cost
                    movePointsLeft -= mapTiles[pathPoint.x, pathPoint.y].Terra.TerraMoveCost;
                }
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
        SetMovePath(GetTileByWorldPosition(from), GetTileByWorldPosition(to));
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
                    tilesmap[_to.x, _to.y] = 1;
                    // Upgrade grid
                    grid.UpdateGrid(tilesmap);
                }
            }
            // because we are using borderless screen we need to find the best possible path
            // we have an object for we the path is needed to be found
            // create 3 grids
            // get the column ID where our object is located
            //int columnId = from.x;
            // init new tiles map, which later will be used for grid generation
            //float[,] newTileMap = new float[tileMapWidth, tileMapHeight];
            // - 1st grid where the object is located on the left edge of the grid
            //// fill in tile map starting from the columnt where the object is now and finishing with the colum to the left from the object
            //// set new tile map column index
            //int nc = 0;
            //for (int c = columnId; c < tileMapWidth; c++)
            //{
            //    // loop though all tiles in a column
            //    for (int i = 0; i < tileMapHeight; i++)
            //    {
            //        // add tile to new tilemap from current tile map
            //        newTileMap[nc, i] = tilesmap[c, i];
            //    }
            //    // increment new tile map column identifier
            //    nc++;
            //}
            //// fill in the rest
            //for (int c = 0; c < columnId; c++)
            //{
            //    // loop though all tiles in a column
            //    for (int i = 0; i < tileMapHeight; i++)
            //    {
            //        // add tile to new tilemap from current tile map
            //        newTileMap[nc, i] = tilesmap[c, i];
            //    }
            //    // increment new tile map column identifier
            //    nc++;
            //}
            //// create new grid
            //NesScripts.Controls.PathFind.Grid newGrid = new NesScripts.Controls.PathFind.Grid(newTileMap);
            //// init new to x position
            //int newTo;
            //// get new to x position
            //if (_to.x < _from.x)
            //{
            //    newTo = 1 + _to.x;
            //}
            //else if (_to.x == _from.x)
            //{
            //    newTo = 0;
            //}
            //else
            //{
            //    newTo = _to.x - _from.x;
            //}
            //// find path on a new grid
            //movePath = NesScripts.Controls.PathFind.Pathfinding.FindPath(newGrid, new NesScripts.Controls.PathFind.Point(0, _from.y), new NesScripts.Controls.PathFind.Point(newTo, to.y));
            // - 2nd grid where the object is located on the right edge of the grid
            // ..
            // - 3rd grid where the object is located in the middle of the grid
            // ..
            // default Find path
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
        SetMovePath(GetTileByWorldPosition(selectedMapHero.transform.position), GetTileByWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        // highlight new path
        HighlightMovePath(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // save mouse position, it may be required for OnBeginDrag
        // mouseOnDownStartPosition = Input.mousePosition;
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
            // Vector3 mapNewPosiiton = Camera.main.WorldToScreenPoint(transform.position);
            // keep z position otherwise map will disappear
            // mapPosition = new Vector3(mapNewPosiiton.x, mapNewPosiiton.y, 0);
            // xCorrectionOnDragStart = (Screen.width / 2) - mouseOnDragStartPosition.x;
            // yCorrectionOnDragStart = (Screen.height / 2) - mouseOnDragStartPosition.y;
            //Debug.Log("Map   [" + mapPosition.x + "," + mapPosition.y + "]");
            //Debug.Log("Mouse [" + mouseOnDownStartPosition.x + "," + mouseOnDownStartPosition.y + "]");
            // xCorrectionOnDragStart = mapPosition.x - mouseOnDownStartPosition.x;
            //yCorrectionOnDragStart = mapPosition.y - mouseOnDownStartPosition.y;
            // this corrections should also be applied to x and y min and max
            //xMin = xMinDef - xCorrectionOnDragStart;
            //xMax = xMaxDef - xCorrectionOnDragStart;
            //xMin = mouseOnDownStartPosition.x - mapPosition.x + Screen.width - mapWidth + tileSize;
            //xMax = mouseOnDownStartPosition.x - mapPosition.x - tileSize;
            //yMin = yMinDef - yCorrectionOnDragStart;
            //yMax = yMaxDef - yCorrectionOnDragStart;
            // activate camera focus on a map
            Camera.main.GetComponent<CameraController>().SetCameraFocus(this, true);
        }
        else if (Input.GetMouseButton(1))
        {
            // on right mouse down
        }
    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    // verify if user clicked left or right mouse button
    //    if (Input.GetMouseButton(0))
    //    {
    //        // on left mouse held down
    //        mousePosition = Input.mousePosition;
    //        mapPosiiton = Camera.main.WorldToScreenPoint(transform.position);
    //        // make sure that new position is within the borders of the map
    //        float newPositionX = mousePosition.x;
    //        Debug.Log("Before:" + GetIndexByPosition(mousePosition.x) / tileSize + "-" + GetIndexByPosition(xMax) / tileSize);
    //        int xCurrent = GetIndexByPosition(transform.position.x) / tileSize;
    //        //if (mousePosition.x <= xMin)
    //        if (mousePosition.x < xMin)
    //        {
    //            newPositionX = xMin;
    //            //// set first map slice as last
    //            //transform.Find("MapSlices").GetChild(0).SetAsLastSibling();
    //            //// reset position
    //            //newPositionX = xMin + tileSize;
    //            //// adjust xMin
    //            //xMin -= tileSize;
    //            //// adjust xMax
    //            //xMax -= tileSize;
    //        }
    //        //else if (mousePosition.x >= xMax)
    //        else if (mousePosition.x > xMax)
    //        {
    //            newPositionX = xMax;
    //            //// set last slice as first
    //            //transform.Find("MapSlices").GetChild(59).SetAsFirstSibling();
    //            //// reset position
    //            //// newPositionX = xMax - tileSize;
    //            //// adjust xMax
    //            //xMax += tileSize;
    //            //// adjust xMin
    //            //xMin += tileSize;
    //        }
    //        float newPositionY = mousePosition.y;
    //        if (mousePosition.y <= yMin)
    //        {
    //            newPositionY = yMin;
    //        }
    //        else if (mousePosition.y >= yMax)
    //        {
    //            newPositionY = yMax;
    //        }
    //        // for unknown reason z is set to -30000 on drag, that is why I use original value
    //        Vector3 newPosition = new Vector3(newPositionX + xCorrectionOnDragStart, newPositionY + yCorrectionOnDragStart, 0);
    //        // make drag to allign with tile size
    //        float x = Mathf.RoundToInt(newPosition.x / tileSize) * tileSize;
    //        float y = Mathf.RoundToInt(newPosition.y / tileSize) * tileSize;
    //        newPosition = new Vector3(x, y, newPosition.z);
    //        Vector3 newPositionTransformed = Camera.main.ScreenToWorldPoint(newPosition);
    //        transform.position = new Vector3(newPositionTransformed.x, newPositionTransformed.y, 0);
    //    }
    //    else if (Input.GetMouseButton(1))
    //    {
    //        // on right mouse held down
    //    }
    //}

    public int RoundToTileSize(float position)
    {
        return Mathf.RoundToInt(position / tileSize) * tileSize;
    }

    void ShiftMapObjects(Shift shiftDirection, int count)
    {
        int shiftPositionModifier = 0;
        switch (shiftDirection)
        {
            case Shift.Left:
                shiftPositionModifier = -count * tileSize;
                break;
            case Shift.Right:
                shiftPositionModifier = count * tileSize;
                break;
            default:
                Debug.LogError("Unknown shift direction: " + shiftDirection.ToString());
                break;
        }
        // init map world corners positions array
        Vector3[] mapWorldCorners = new Vector3[4];
        // Get map world corners
        // structure:
        // 12
        // 03
        GetComponent<RectTransform>().GetWorldCorners(mapWorldCorners);
        // init buffer to prevent false positives
        float buffer = tileSize / 3;
        // init transform world corners positions array
        Vector3[] mapObjectWorldCorners = new Vector3[4];
        // shift objects on map
        foreach (MapObject mapObject in transform.GetComponentsInChildren<MapObject>())
        {
            float newPositionX = mapObject.transform.position.x + shiftPositionModifier;
            // set object position
            mapObject.transform.position = new Vector3(newPositionX, mapObject.transform.position.y, 0);
            // get objects GetWorldCorners
            mapObject.GetComponent<RectTransform>().GetWorldCorners(mapObjectWorldCorners);
            // verify if object is moving off screen
            // Debug.Log(mapObject.name + " " + Mathf.RoundToInt(newPositionX / tileSize) + ":" + Mathf.RoundToInt((transform.position.x) / tileSize));
            // object tile is lower or equal than map tile position with 1 tile shift
            //if (Mathf.RoundToInt(mapObject.transform.position.x / tileSize) <= Mathf.RoundToInt((transform.position.x + tileSize) / tileSize))
            //if (Mathf.RoundToInt(newPositionX / tileSize) < Mathf.RoundToInt(transform.position.x / tileSize))
            // Debug.Log(mapObjectWorldCorners[0].x + ":" + mapWorldCorners[0].x);
            // verify if objects right border position is less than the map left border position
            if (mapObjectWorldCorners[3].x - buffer < mapWorldCorners[0].x)
            {
                // move object to the other side of the map
                // or shift it left or rotate
                newPositionX += mapWidth;
            }
            //else if (Mathf.RoundToInt(mapObject.transform.position.x / tileSize) >= Mathf.RoundToInt((transform.position.x + mapWidth - tileSize) / tileSize))
            //else if (Mathf.RoundToInt(mapObject.transform.position.x / tileSize) > tileMapWidth)
            // verify if objects left border position is more than the map right border position
            if (mapObjectWorldCorners[0].x + buffer > mapWorldCorners[3].x)
            {
                // move object to the other side of the map
                // or shift it right or rotate
                newPositionX -= mapWidth;
            }
            // set object position
            mapObject.transform.position = new Vector3(newPositionX, mapObject.transform.position.y, 0);
            // Debug.Log(mapObject.name + " " + Mathf.RoundToInt(mapObject.transform.position.x / tileSize) + ":" + Mathf.RoundToInt((transform.position.x) / tileSize));
            //// set object label position
            //mapObject.Label.SetLabelByMapObjectPosition();
        }
    }

    int mapShift;

    void ShiftTiles(Shift shiftDirection, int count)
    {
        switch (shiftDirection)
        {
            case Shift.Left:
                mapShift -= count;
                break;
            case Shift.Right:
                mapShift += count;
                break;
            default:
                Debug.LogError("Unknown shift direction: " + shiftDirection.ToString());
                break;
        }
        // verify if shift made a full circle
        if (Math.Abs(mapShift) >= tileMapWidth)
        {
            // Debug.Log("Reset map shift");
            if (mapShift > 0)
            {
                mapShift -= tileMapWidth;
            }
            else
            {
                mapShift += tileMapWidth;
            }
        }
        // Debug.Log("Map Shift: " + mapShift);
    }

    public void RotateLeft(int numberOfRotationsRequired = 1)
    {
        for (int i = 0; i < numberOfRotationsRequired; i++)
        {
            // do rotation
            // Debug.Log("Set last slice as first");
            // set last slice as first
            mapSlicesTransform.GetChild(59).SetAsFirstSibling();
            fogOfWarTransform.GetChild(59).SetAsFirstSibling();
        }
        // Shift map objects
        ShiftMapObjects(Shift.Right, numberOfRotationsRequired);
        ShiftTiles(Shift.Right, numberOfRotationsRequired);
    }

    public void RotateRight(int numberOfRotationsRequired = 1)
    {
        // int numberOfRotationsRequired = 1;
        // Debug.Log("Number of rotaions required is " + numberOfRotationsRequired);
        for (int i = 0; i < numberOfRotationsRequired; i++)
        {
            // do rotation
            // Debug.Log("Set first map slice as last");
            // set first map slice as last
            mapSlicesTransform.GetChild(0).SetAsLastSibling();
            fogOfWarTransform.GetChild(0).SetAsLastSibling();
        }
        // Shift map objects
        ShiftMapObjects(Shift.Left, numberOfRotationsRequired);
        ShiftTiles(Shift.Left, numberOfRotationsRequired);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // verify if user clicked left or right mouse button
        if (Input.GetMouseButton(0))
        {
            // note: for up/down movement camera will be moved
            // on left mouse held down
            // float newPositionY = Input.mousePosition.y;
            //if (Input.mousePosition.y <= yMin)
            //{
            //    newPositionY = yMin;
            //}
            //else if (Input.mousePosition.y >= yMax)
            //{
            //    newPositionY = yMax;
            //}
            // Debug.Log("Mouse:Camera y " + (int)Input.mousePosition.y + ":" + (int)Camera.main.transform.position.y);
            // move camera up/down instead of map
            //Vector3 newCameraPosition = Camera.main.transform.position;
            //newCameraPosition.y = newPositionY;
            //Camera.main.transform.position = newCameraPosition;
            //
            //
            // for unknown reason z is set to -30000 on drag, that is why I reset it to 0
            // transform.position = new Vector3(Input.mousePosition.x + xCorrectionOnDragStart + rotationPositionModifier, newPositionY + yCorrectionOnDragStart, 0);
            // do not change map position, instead move camera
            // transform.position = new Vector3(Input.mousePosition.x + xCorrectionOnDragStart + rotationPositionModifier, 0, 0);
            // Debug.Log("New position: " + transform.position.x + ":" + transform.position.y + ":" + transform.position.z);
            // Debug.Log("Camera x:y " + (int)Camera.main.transform.position.x + ":" + (int)Camera.main.transform.position.y);
            // define border
            //float leftBorder = -tileSize;
            //float rightBorder = Screen.width - mapWidth + tileSize;
            //// verify if map has reached left border with tilesize buffer
            //if (transform.position.x >= leftBorder)
            //{
            //    // get number of rotaitons required based on the distance and tile size
            //    int numberOfRotationsRequired = Math.Abs(Mathf.RoundToInt(transform.position.x / tileSize)) + 1;
            //    for (int i = 0; i < numberOfRotationsRequired; i++)
            //    {
            //        // do rotation
            //        // Debug.Log("Set last slice as first");
            //        // set last slice as first
            //        transform.Find("MapSlices").GetChild(59).SetAsFirstSibling();
            //        fogOfWarTransform.GetChild(59).SetAsFirstSibling();
            //        // adjust position by one tile size to the left
            //        transform.position = new Vector3(transform.position.x - tileSize, transform.position.y, 0);
            //        // increment rotation position modifier by tile size
            //        rotationPositionModifier -= tileSize;
            //    }
            //    // Shift map objects
            //    ShiftMapObjects(Shift.Right, numberOfRotationsRequired);
            //    ShiftTiles(Shift.Right, numberOfRotationsRequired);
            //}
            //// verify if map has reached right border with tilesize buffer
            //else if (transform.position.x <= rightBorder)
            //{
            //    // get number of rotaitons required based on the distance and tile size
            //    int numberOfRotationsRequired = Mathf.Abs(Mathf.RoundToInt((rightBorder - transform.position.x) / tileSize)) + 1;
            //    // int numberOfRotationsRequired = 1;
            //    // Debug.Log("Number of rotaions required is " + numberOfRotationsRequired);
            //    for (int i = 0; i < numberOfRotationsRequired; i++)
            //    {
            //        // do rotation
            //        // Debug.Log("Set first map slice as last");
            //        // set first map slice as last
            //        transform.Find("MapSlices").GetChild(0).SetAsLastSibling();
            //        fogOfWarTransform.GetChild(0).SetAsLastSibling();
            //        // shift position by one tile size to the right
            //        transform.position = new Vector3(transform.position.x + tileSize, transform.position.y, 0);
            //        // increment rotation position modifier by tile size
            //        rotationPositionModifier += tileSize;
            //    }
            //    // Shift map objects
            //    ShiftMapObjects(Shift.Left, numberOfRotationsRequired);
            //    ShiftTiles(Shift.Left, numberOfRotationsRequired);
            //}
        }
        else if (Input.GetMouseButton(1))
        {
            // on right mouse held down
        }
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    // verify if user clicked left or right mouse button
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        // on left mouse up
    //        // enter back to Browse mode
    //        SetMode(Mode.Browse);
    //        // set tile highighter under mouse
    //        SetTileHighlighterToMousePoistion();
    //        // allow map to block raycasts
    //        GetComponent<CanvasGroup>().blocksRaycasts = true;
    //    }
    //    else if (Input.GetMouseButtonUp(1))
    //    {
    //        // on right mouse up
    //    }
    //}

    public void OnEndDrag(PointerEventData eventData)
    {
        // verify if user clicked left or right mouse button
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse up
            // align map with tile size
            transform.position = new Vector3(RoundToTileSize(transform.position.x), RoundToTileSize(transform.position.y), RoundToTileSize(transform.position.z));
            // reset rotation position modifier
            // rotationPositionModifier = 0;
            // enter back to Browse mode
            SetMode(Mode.Browse);
            // set tile highighter under mouse
            tileHighlighter.SetToMousePoistion();
            // allow map to block raycasts
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // deactivate camera focus on a map
            Camera.main.GetComponent<CameraController>().SetCameraFocus(this, false);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse up
        }
    }

    //void SaveCitiesNamesToggleOptions()
    //{
    //    // Always show city names toggle options 0 - disable, 1 - enable
    //    // verify if toggle is currently selected
    //    if (MapMenuManager.Instance.MapOptions.transform.Find("ToggleCitiesNames").GetComponent<TextToggle>().selected)
    //    {
    //        // set option
    //        GameOptions.options.mapUIOpt.toggleCitiesNames = 1;
    //    }
    //    else
    //    {
    //        // set option
    //        GameOptions.options.mapUIOpt.toggleCitiesNames = 0;
    //    }
    //    // save option
    //    PlayerPrefs.SetInt("MapUIShowCityNames", GameOptions.options.mapUIOpt.toggleCitiesNames); // 0 - disable, 1 - enable
    //}

    //void SaveHeroesNamesToggleOptions()
    //{
    //    // Always show city names toggle options 0 - disable, 1 - enable
    //    // verify if toggle is currently selected
    //    if (MapMenuManager.Instance.MapOptions.transform.Find("ToggleHeroesNames").GetComponent<TextToggle>().selected)
    //    {
    //        // set option
    //        GameOptions.options.mapUIOpt.toggleHeroesNames = 1;
    //    }
    //    else
    //    {
    //        // set option
    //        GameOptions.options.mapUIOpt.toggleHeroesNames = 0;
    //    }
    //    // save option
    //    PlayerPrefs.SetInt("MapUIShowHeroesNames", GameOptions.options.mapUIOpt.toggleHeroesNames); // 0 - disable, 1 - enable
    //}

    //void SavePlayerIncomeToggleOptions()
    //{
    //    // Always show city names toggle options 0 - disable, 1 - enable
    //    // verify if toggle is currently selected
    //    if (MapMenuManager.Instance.MapOptions.transform.Find("TogglePlayerIncome").GetComponent<TextToggle>().selected)
    //    {
    //        // set option
    //        GameOptions.options.mapUIOpt.togglePlayerIncome = 1;
    //    }
    //    else
    //    {
    //        // set option
    //        GameOptions.options.mapUIOpt.togglePlayerIncome = 0;
    //    }
    //    // save option
    //    PlayerPrefs.SetInt("MapUIShowPlayerIncome", GameOptions.options.mapUIOpt.togglePlayerIncome); // 0 - disable, 1 - enable
    //}

    //void SaveMapUIOptions()
    //{
    //    Debug.Log("Save Map UI Options");
    //    // verify if map options has not been destroyed
    //    if (MapMenuManager.Instance.MapOptions != null)
    //    {
    //        SaveCitiesNamesToggleOptions();
    //        SaveHeroesNamesToggleOptions();
    //        SavePlayerIncomeToggleOptions();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Cannot save map options. MapOptions transform has been destoyed");
    //    }
    //}

    //void LoadCitiesNamesToggleOptions()
    //{
    //    // Get map UI options
    //    GameOptions.options.mapUIOpt.toggleCitiesNames = PlayerPrefs.GetInt("MapUIShowCityNames", 0); // default 0 - disable "always show city names" toggle
    //    // Get City names toggle
    //    TextToggle textToggle = MapMenuManager.Instance.MapOptions.transform.Find("ToggleCitiesNames").GetComponent<TextToggle>();
    //    // verify if it was enabled before
    //    if (GameOptions.options.mapUIOpt.toggleCitiesNames == 0)
    //    {
    //        // disable toggle
    //        textToggle.selected = false;
    //        textToggle.SetNormalStatus();
    //        // hide cities names
    //        SetCitiesNamesVisible(false);
    //    }
    //    else
    //    {
    //        // enable toggle
    //        textToggle.selected = true;
    //        textToggle.SetPressedStatus();
    //        // always show city names
    //        SetCitiesNamesVisible(true);
    //    }
    //}

    //void LoadHeroesNamesToggleOptions()
    //{
    //    // Get map UI options
    //    GameOptions.options.mapUIOpt.toggleHeroesNames = PlayerPrefs.GetInt("MapUIShowHeroesNames", 0); // default 0 - disable "always show heroes names" toggle
    //    // Get toggle
    //    TextToggle textToggle = MapMenuManager.Instance.MapOptions.transform.Find("ToggleHeroesNames").GetComponent<TextToggle>();
    //    // verify if it was enabled before
    //    if (GameOptions.options.mapUIOpt.toggleHeroesNames == 0)
    //    {
    //        // disable toggle
    //        textToggle.selected = false;
    //        textToggle.SetNormalStatus();
    //        // hide cities names
    //        SetHeroesNamesVisible(false);
    //    }
    //    else
    //    {
    //        // enable toggle
    //        textToggle.selected = true;
    //        textToggle.SetPressedStatus();
    //        // always show city names
    //        SetHeroesNamesVisible(true);
    //    }
    //}

    //void LoadPlayerIncomeToggleOptions()
    //{
    //    // Get map UI options
    //    GameOptions.options.mapUIOpt.togglePlayerIncome = PlayerPrefs.GetInt("MapUIShowPlayerIncome", 0); // default 0 - disable "always show heroes names" toggle
    //    // Get toggle
    //    TextToggle textToggle = MapMenuManager.Instance.MapOptions.transform.Find("TogglePlayerIncome").GetComponent<TextToggle>();
    //    // verify if it was enabled before
    //    if (GameOptions.options.mapUIOpt.togglePlayerIncome == 0)
    //    {
    //        // disable toggle
    //        textToggle.selected = false;
    //        textToggle.SetNormalStatus();
    //        // hide player income
    //        SetPlayerIncomeVisible(false);
    //    }
    //    else
    //    {
    //        // enable toggle
    //        textToggle.selected = true;
    //        textToggle.SetPressedStatus();
    //        // show player income
    //        SetPlayerIncomeVisible(true);
    //    }
    //}

    //void LoadMapUIOptions()
    //{
    //    Debug.Log("Load Map UI Options");
    //    LoadCitiesNamesToggleOptions();
    //    LoadHeroesNamesToggleOptions();
    //    LoadPlayerIncomeToggleOptions();
    //}

    void OnEnable()
    {
        //LoadMapUIOptions();
        // GetComponentInChildren<TileHighlighter>(true).gameObject.SetActive(true);
        tileHighlighter.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        //SaveMapUIOptions();
        SetMode(Mode.Browse);
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

    int GetShiftedPathPointCoordinateX(int defaultCoordinateX)
    {
        int shiftedTilePositionX = defaultCoordinateX + mapShift;
        // verify if we are not ouside of the grid borders
        if (shiftedTilePositionX >= tileMapWidth)
        {
            // reset it
            shiftedTilePositionX -= tileMapWidth;
        }
        // verify if it is not less than 0
        else if (shiftedTilePositionX < 0)
        {
            // reset it
            shiftedTilePositionX += tileMapWidth;
        }
        return shiftedTilePositionX;
    }

    Vector2 GetDestination(NesScripts.Controls.PathFind.Point pathPoint)
    {
        // + tileSize/ is to place it in the center of the tile
        return new Vector2
        {
            x = (float)(GetShiftedPathPointCoordinateX(pathPoint.x)) * (float)tileSize + (float)tileSize / 2f + transform.position.x,
            y = (float)(pathPoint.y) * (float)tileSize + (float)tileSize / 2f + transform.position.y
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
        queue.Run(MapMenuManager.Instance.EnterCityEditMode(mapCity));
        // Trigger on hero entering city
        // ..
        // reset map state and selections, because hero can be removed while in city
        SetSelection(Selection.None);
    }

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
        // InputBlocker inputBlocker = gameRoot.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.Instance.SetActive(false);
        yield return null;
    }

    IEnumerator EnterBattleStep(MapHero mapHero)
    {
        Debug.Log("EnterBattleStep");
        UIRoot.Instance.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).EnterBattle(selectedMapHero, mapHero);
        yield return null;
    }

    IEnumerator EnterBattleStep(MapCity mapCity)
    {
        Debug.Log("EnterBattleStep");
        UIRoot.Instance.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).EnterBattle(selectedMapHero, mapCity);
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


    IEnumerator PickupItem(MapItemsContainer mapItem)
    {
        Debug.Log("Pick up item");
        // Activate inventory item pickup pop-up menu
        UIRoot.Instance.transform.Find("MiscUI/ItemsPickUpPopUp").GetComponent<InventoryItemPickUpPopUp>().SetActive(mapItem);
        // Loop through each item in the chest
        foreach (InventoryItem inventoryItem in mapItem.LInventoryItems)
        {
            // change parent to hero party
            inventoryItem.transform.SetParent(selectedMapHero.LHeroParty.transform);
        }
        // Destroy chest label
        Destroy(mapItem.GetComponent<MapObject>().Label.gameObject);
        // Destroy chest
        Destroy(mapItem.gameObject);
        // exit animation mode and enter browse mode
        SetMode(Mode.Browse);
        yield return new WaitForSeconds(0.5f);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // Exit coroutine
        yield return null;
    }

    Vector3 GetDirection(Vector2Int nodeA, Vector2Int nodeB)
    {
        // get direction vector
        Vector3 direction = new Vector3
        {
            x = ((nodeA.x - nodeB.x + 3 * tileMapWidth / 2) % tileMapWidth) - tileMapWidth / 2,
            y = ((nodeA.y - nodeB.y + 3 * tileMapHeight / 2) % tileMapHeight) - tileMapHeight / 2,
            z = 0
        };
        // normalize vector, so it can be used in move towards function with constant move speed
        return Vector3.Normalize(direction);
    }

    //float ToroidalDistance(float x1, float y1, float x2, float y2)
    //{
    //    float dx = Mathf.Abs(x2 - x1);
    //    float dy = Mathf.Abs(y2 - y1);

    //    if (dx > 0.5f)
    //        dx = 1.0f - dx;

    //    if (dy > 0.5f)
    //        dy = 1.0f - dy;

    //    return Mathf.Sqrt(dx * dx + dy * dy);
    //}

    public Vector3 GetDirection(Vector3 dst, Vector3 src)
    {
        // get direction vector
        Vector3 direction = new Vector3
        {
            x = ((dst.x - src.x + 3f * mapWidth / 2f) % mapWidth) - mapWidth / 2f,
            y = ((dst.y - src.y + 3f * mapHeight / 2f) % mapHeight) - mapHeight / 2f,
            z = 0
        };
        // normalize vector, so it can be used in move towards function with constant move speed
        return Vector3.Normalize(direction);
    }

    public float GetToroidalDistance(Vector3 src, Vector3 dst)
    {
        float dx = Mathf.Abs(src.x - dst.x);
        if (dx > mapWidth / 2f)
            dx = mapWidth - dx;
        float dy = Mathf.Abs(src.y - dst.y);
        if (dy > mapHeight / 2f)
            dy = mapHeight - dy;
        return Mathf.Sqrt(dx*dx + dy*dy);
    }

    //Vector3 MoveTowards(Vector3 src, Vector2Int nodeB, float maxDistance)
    public Vector3 MoveTowards(Vector3 src, Vector3 dst, float maxDistance, Vector3 direction)
    {

        // float distance = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
        // move towards the goal
        //src.x = (src.x - maxDistance * direction.x);
        //src.y = (src.y - maxDistance * direction.y);
        // get distance
        float distance = GetToroidalDistance(src, dst);
        // Debug.Log("d: " + distance.ToString() + " md: " + maxDistance.ToString());
        if (distance > maxDistance)
        {
            // move towards
            src.x = (src.x + maxDistance * direction.x);
            src.y = (src.y + maxDistance * direction.y);
            // Debug.Log("post-remaining distance " + GetToroidalDistance(src, dst));
        }
        else
        {
            // already there
            // Debug.Log("Already there");
            src.x = dst.x;
            src.y = dst.y;
        }
        return src;
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
            heroParty.transform.SetParent(ObjectsManager.Instance.GameMap.transform);
            // Update hero party place
            //heroParty.SetPlace(HeroParty.PartyPlace.Map);
        }
        // Verify if hero party was on hold
        if (selectedMapHero.LHeroParty.HoldPosition == true)
        {
            // trigger break hold event
            MapMenuManager.Instance.MapFocusPanel.BreakHoldPosition(selectedMapHero);
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
            // InputBlocker inputBlocker = gameRoot.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
            InputBlocker.Instance.SetActive(true);
            // initialize break move condition
            bool breakMove = false;
            MapCity enterCity = null;
            MapHero protectedTileEnemy = null;
            Faction selectedHeroFaction = selectedMapHero.LHeroParty.Faction;
            // Get map focus panel
            //MapFocusPanel mapFocusPanel = mapFocusPanel;
            // loop through path points
            Debug.Log("Move path count: " + movePath.Count.ToString());
            for (int i = 0; i < (movePath.Count) && (partyLeader.MovePointsCurrent >= 1) ; i++)
            {
                // consume move points from hero
                partyLeader.MovePointsCurrent -= mapTiles[movePath[i].x, movePath[i].y].Terra.TerraMoveCost;
                // verify if move points number is less than 0
                if (partyLeader.MovePointsCurrent < 0)
                {
                    // reset it to 0
                    partyLeader.MovePointsCurrent = 0;
                }
                // update focus panel info
                MapMenuManager.Instance.MapFocusPanel.UpdateMovePointsInfo();
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
                    MapItemsContainer mapItem = nextGameObjectOnPath.GetComponent<MapItemsContainer>();
                    if (mapHero)
                    {
                        Debug.Log("Map hero on path");
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
                                // verify if the city is our destination point
                                if (i == movePath.Count - 1)
                                {
                                    // continue move untill we are on the same tile
                                    enterCity = mapCity;
                                } else
                                {
                                    // city tile is intermediate point
                                    // nothing to do here
                                }
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
                        Debug.Log("Map item on path");
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
                    Debug.Log("No object on path");
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
                // Get direction
                Vector3 direction = GetDirection(new Vector2Int(pathPoint.x, pathPoint.y), GetTileByWorldPosition(selectedMapHero.transform.position));
                // Vector3 direction = GetDirection(dst, src);
                Debug.Log("dir: " + direction.x.ToString() + ":" + direction.y.ToString() + ":" + direction.z.ToString());
                while (GetRemainingDistance(pathPoint) > 0.5f)
                {
                    // Recalculate destination, because it may change due to map shift when camera follows hero
                    dst = GetDestination(pathPoint);
                    // Debug.Log("Path point is [" + pathPoint.x + "]:[" + pathPoint.y + "]");
                    // move hero
                    deltaTime = Time.time - previousTime;
                    // selectedMapHero.transform.position = Vector2.MoveTowards(selectedMapHero.transform.position, dst, deltaTime * heroMoveSpeed);
                    // selectedMapHero.transform.position = MoveTowards(selectedMapHero.transform.position, new Vector2Int(pathPoint.x, pathPoint.y), deltaTime * heroMoveSpeed);
                    selectedMapHero.transform.position = MoveTowards(selectedMapHero.transform.position, dst, deltaTime * heroMoveSpeed, direction);
                    //selectedMapHero.transform.Translate(direction * deltaTime * heroMoveSpeed, Space.World);
                    // wait until next move
                    previousTime = Time.time;
                    yield return new WaitForSeconds(heroMoveSpeedDelay);
                }
                // discover tiles around
                // reveal discovered tiles
                foreach(Vector2Int tileCoords in DiscoverTilesAround(selectedMapHero))
                {
                    DiscoverTile(tileCoords.x, tileCoords.y);
                }
                // 
                if (enterCity)
                {
                    EnterCityAfterMove(enterCity);
                }
                if (protectedTileEnemy)
                {
                    EnterBattle(protectedTileEnemy);
                    break;
                }
            }
            // verify if we reached this code without breaking move or entering city or protected enemy tile
            if (!(breakMove || enterCity || protectedTileEnemy))
            {
                // this was just move on the map without any additional challanges on the way
                // exit animation mode and enter browse mode
                SetMode(Mode.Browse);
                // Unblock mouse input
                InputBlocker.Instance.SetActive(false);
            }
        }
        // Remove path highlight
        HighlightMovePath(false);
        // Release camera focus
        MapHero nullMapHero = null;
        Camera.main.GetComponent<CameraController>().SetCameraFocus(nullMapHero);
    }

    public void SetMode(Mode value)
    {
        Debug.Log("Change map manager mode to : " + value);
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

    public void OnPointerEnterChildObject(GameObject childGameObject, PointerEventData eventData)
    {
        //// set default tile highlighter color
        //tileHighlighterColor = Color.yellow;
        // predefine variables
        MapHero mapHero = childGameObject.GetComponent<MapHero>();
        MapCity mapCity = childGameObject.GetComponent<MapCity>();
        MapItemsContainer mapItem = childGameObject.GetComponent<MapItemsContainer>();
        MapObjectLabel mapHeroViaLabel = null;
        MapObjectLabel mapCityViaLabel = null;
        MapObjectLabel label = childGameObject.GetComponent<MapObjectLabel>();
        if (label)
        {
            // set hero and city variables
            mapHero = label.MapObject.GetComponent<MapHero>();
            mapCity = label.MapObject.GetComponent<MapCity>();
            // find out on which label we clicked
            if (label.MapObject.GetComponent<MapHero>())
            {
                mapHeroViaLabel = label;
            }
            if (label.MapObject.GetComponent<MapCity>())
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
                                CursorController.Instance.SetSelectionHandCursor();
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
                                CursorController.Instance.SetSelectionHandCursor();
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
                                            CursorController.Instance.SetOpenDoorsCursor();
                                        }
                                        else
                                        {
                                            //Debug.Log("hero on map");
                                            // her os on map
                                            // change cursor to edit hero cursor
                                            CursorController.Instance.SetEditHeroCursor();
                                            //tileHighlighterColor = Color.blue;
                                        }
                                    }
                                    else
                                    {
                                        // change cursor to selection hand
                                        //CursorController.Instance.SetMoveArrowCursor();
                                        CursorController.Instance.SetSelectionHandCursor();
                                    }
                                    break;
                                case Relationships.State.Allies:
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    CursorController.Instance.SetAttackCursor();
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
                                            CursorController.Instance.SetOpenDoorsCursor();
                                        }
                                        else
                                        {
                                            Debug.Log("hero on map");
                                            // her os on map
                                            // change cursor to edit hero cursor
                                            CursorController.Instance.SetEditHeroCursor();
                                            //tileHighlighterColor = Color.blue;
                                        }
                                    }
                                    else
                                    {
                                        // change cursor to selection hand
                                        CursorController.Instance.SetSelectionHandCursor();
                                        //tileHighlighterColor = Color.blue;
                                    }
                                    break;
                                case Relationships.State.Allies:
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    CursorController.Instance.SetNormalCursor();
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
                                    CursorController.Instance.SetMoveArrowCursor();
                                    //tileHighlighterColor = Color.blue;
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    CursorController.Instance.SetAttackCursor();
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
                                    CursorController.Instance.SetSelectionHandCursor();
                                    //tileHighlighterColor = Color.blue;
                                    break;
                                case Relationships.State.Allies:
                                    break;
                                case Relationships.State.Neutral:
                                case Relationships.State.AtWar:
                                    CursorController.Instance.SetNormalCursor();
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
                            CursorController.Instance.SetGrabHandCursor();
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
                                CursorController.Instance.SetSelectionHandCursor();
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
                                    CursorController.Instance.SetOpenDoorsCursor();
                                }
                                else
                                {
                                    // change cursor to selection hand
                                    CursorController.Instance.SetSelectionHandCursor();
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
    }

    public void OnPointerExitChildObject(GameObject childGameObject, PointerEventData eventData)
    {
        switch (mode)
        {
            case Mode.Browse:
                // change cursor
                CursorController.Instance.SetNormalCursor();
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
        Vector2Int clickedTargetTile = GetTileByWorldPosition(Camera.main.ScreenToWorldPoint(eventData.position));
        NesScripts.Controls.PathFind.Point clickedTargetPathPoint = new NesScripts.Controls.PathFind.Point(clickedTargetTile.x, clickedTargetTile.y);
        // verify if we already highlighted path
        if (selectedTargetPathPoint != null)
        {
            // We already have path highlighted
            // verify if it is path to the same target
            if (selectedTargetPathPoint == clickedTargetPathPoint)
            {
                // we target the same path point
                // focus camera on a unit
                // Debug.Log("Focus camera on a map hero");
                Camera.main.GetComponent<CameraController>().SetCameraFocus(selectedMapHero);
                // Move to the point
                // StartCoroutine(Move());
                queue.Run(Move());
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
                CursorController.Instance.SetNormalCursor();
                // update focus panel
                MapMenuManager.Instance.MapFocusPanel.ReleaseFocus();
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
                MapMenuManager.Instance.MapFocusPanel.SetActive(mH);
                mH.SetSelectedState(true);
                // verify if hero is in city
                if (mH.lMapCity)
                {
                    //Debug.Log("SetSelection hero in city");
                    // hero is in city
                    // change cursor to open doors cursor -> enter city edit mode indicator
                    CursorController.Instance.SetOpenDoorsCursor();
                }
                else
                {
                    //Debug.Log("SetSelection hero on map");
                    // her os on map
                    // change cursor to edit hero cursor
                    CursorController.Instance.SetEditHeroCursor();
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
                MapMenuManager.Instance.MapFocusPanel.SetActive(mC);
                mC.SetSelectedState(true);
                // change cursor to open doors cursor
                CursorController.Instance.SetOpenDoorsCursor();
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
        queue.Run(MapMenuManager.Instance.EnterCityEditMode(mapCity));
    }

    public void ActOnClick(GameObject childGameObject, PointerEventData pointerEventData)
    {
        // predefine variables
        MapHero mapHero = childGameObject.GetComponent<MapHero>();
        MapCity mapCity = childGameObject.GetComponent<MapCity>();
        MapItemsContainer mapItem = childGameObject.GetComponent<MapItemsContainer>();
        MapObjectLabel mapHeroViaLabel = null;
        MapObjectLabel mapCityViaLabel = null;
        MapObjectLabel label = childGameObject.GetComponent<MapObjectLabel>();
        if (label)
        {
            // find out on which label we clicked
            if (label.MapObject.GetComponent<MapHero>())
            {
                mapHeroViaLabel = label;
            }
            if (label.MapObject.GetComponent<MapCity>())
            {
                mapCityViaLabel = label;
            }
        }
        // check if child game object (on which we clicked is Map == MapManager)
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
                                mapHero = mapHeroViaLabel.MapObject.GetComponent<MapHero>();
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
                                mapCity = mapCityViaLabel.MapObject.GetComponent<MapCity>();
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
                                    queue.Run(MapMenuManager.Instance.EnterCityEditMode(mapCity));
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
                            mapCity = mapCityViaLabel.MapObject.GetComponent<MapCity>();
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
                                    queue.Run(MapMenuManager.Instance.EnterHeroEditMode(mapHero));
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
                            mapHero = mapHeroViaLabel.MapObject.GetComponent<MapHero>();
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
                                        queue.Run(MapMenuManager.Instance.EnterCityEditMode(_mapCity));
                                    }
                                    else
                                    {
                                        // her os on map
                                        // enter hero edit mode
                                        queue.Run(MapMenuManager.Instance.EnterHeroEditMode(mapHero));
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
                                mapHero = mapHeroViaLabel.MapObject.GetComponent<MapHero>();
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
                                mapCity = mapCityViaLabel.MapObject.GetComponent<MapCity>();
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
        Vector2Int fleeingPartyPositionOnMap = GetTileByWorldPosition(fleeingPartyTransform.position);
        Vector2Int oppositeObjectPositionOnMap = GetTileByWorldPosition(oppositeObjectTransform.position);
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

    public void SetCitiesPassableByFaction(Faction faction)
    {
        // loop though all cities on the map
        foreach(MapCity mapCity in GetComponentsInChildren<MapCity>())
        {
            // get city tile coordintates
            Vector2Int cityTileCoords = GetTileByWorldPosition(mapCity.transform.position);
            // verify map city faction
            if (mapCity.LCity.CityFaction == faction)
            {
                // set city tile passable for active player
                mapTiles[cityTileCoords.x, cityTileCoords.y].Terra.TerraIsPassable = true;
            }
            else
            {
                // set city tile not passable for active player
                mapTiles[cityTileCoords.x, cityTileCoords.y].Terra.TerraIsPassable = false;
            }
        }
    }

    public Transform GetParentTransformByType(MapHero mapHero)
    {
        return mapHeroesParentTransformOnMap;
    }

    public Transform GetParentTransformByType(MapCity mapCity)
    {
        return mapCitiesParentTransformOnMap;
    }

    public Transform GetParentTransformByType(MapItemsContainer mapItemsContainer)
    {
        return mapItemsContainersParentTransformOnMap;
    }

    public Transform GetParentTransformByType(MapObjectLabel mapObjectLabel)
    {
        return mapLabelsParentTransformOnMap;
    }

    public void SetPlayerColor(Color color)
    {
        // change player color
        Debug.Log("Change player color to " + color);
        // Get active player
        GamePlayer activePlayer = TurnsManager.Instance.GetActivePlayer();
        // Set player color
        activePlayer.PlayerColor = color;
        // change colors for all objects on map which belong to the active player
        // loop through all heroes
        foreach(MapHero mapHero in mapHeroesParentTransformOnMap.GetComponentsInChildren<MapHero>())
        {
            // check if faction is the same as for the active player
            if (mapHero.LHeroParty.Faction == activePlayer.Faction)
            {
                // change text color
                mapHero.SetColor(color);
            }
        }
        // loop through all cities
        foreach (MapCity mapCity in mapCitiesParentTransformOnMap.GetComponentsInChildren<MapCity>())
        {
            // check if faction is the same as for the active player
            if (mapCity.LCity.CityFaction == activePlayer.Faction)
            {
                // change text color
                mapCity.SetColor(color);
            }
        }
    }

    public void ExecutePreTurnActions()
    {
        // Update map tiles data, because some friendly cities are passable and other cities are not passable unless conquerred.
        InitTilesMap();
    }
}
