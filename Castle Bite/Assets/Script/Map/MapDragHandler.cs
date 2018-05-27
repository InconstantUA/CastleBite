using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

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
    Vector3 mouseOnDragStartPosition;
    float xCorrectionOnDragStart;
    float yCorrectionOnDragStart;

    void Start()
    {
        // get map width and height
        mapWidth = gameObject.GetComponentInChildren<SpriteRenderer>().size.x;
        mapHeight = gameObject.GetComponentInChildren<SpriteRenderer>().size.y;
        // calculate screen borders
        // get maximum possible offset
        float xDeltaMax = (mapWidth - Screen.width) / 2;
        float yDeltaMax = (mapHeight - Screen.height) / 2;
        // border depend on the center position
        // because canvas is positioned in the lower left corner
        // mouse's 0:0 coordinates are located at the same position
        // center is screen width and height divided by 2
        xMinDef = (Screen.width / 2) - xDeltaMax;
        yMinDef = (Screen.height / 2) - yDeltaMax;
        xMaxDef = (Screen.width / 2) + xDeltaMax;
        yMaxDef = (Screen.height / 2) + yDeltaMax;
        // convert it to screen to world coordinates
        //Vector3 bordersMax = new Vector3(xMax, yMax, 100);
        //Vector3 bordersMin = new Vector3(xMin, yMin, 100);
        // xMax = Camera.main.WorldToScreenPoint(bordersMax).x;
        // yMax = Camera.main.WorldToScreenPoint(bordersMax).y;
        // Debug.Log("min x:" + xMinDef + " y:" + yMinDef);
        // Debug.Log("max x:" + xMaxDef + " y:" + yMaxDef);
        //xMax = Camera.main.ScreenToWorldPoint(bordersMax).x;
        //yMax = Camera.main.ScreenToWorldPoint(bordersMax).y;
        //xMin = Camera.main.ScreenToWorldPoint(bordersMin).x;
        //yMin = Camera.main.ScreenToWorldPoint(bordersMin).y;
        //Debug.Log("after translation");
        //Debug.Log("min x:" + xMin + " y:" + yMin);
        //Debug.Log("max x:" + xMax + " y:" + yMax);
    }

    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        //  itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        // get mouse start possition and calculate which offset to apply to final transform
        // this is because I would like to avoid that map center is also centered under mouse
        // this create strange map jumps
        mouseOnDragStartPosition = Input.mousePosition;
        // apply corrections depeding on location from the canvas center
        // this actually depends one the current position of the map
        mapPosiiton = Camera.main.WorldToScreenPoint(transform.position);
        // xCorrectionOnDragStart = (Screen.width / 2) - mouseOnDragStartPosition.x;
        // yCorrectionOnDragStart = (Screen.height / 2) - mouseOnDragStartPosition.y;
        xCorrectionOnDragStart = mapPosiiton.x - mouseOnDragStartPosition.x;
        yCorrectionOnDragStart = mapPosiiton.y - mouseOnDragStartPosition.y;
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
        transform.position = Camera.main.ScreenToWorldPoint(newPosition);
        //    Debug.Log("transform " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
    }
    #endregion
    #region IEndDragHandler implementation
    public void OnEndDrag(PointerEventData eventData)
    {
        // itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == startParent)
        {
            // transform.position = startPosition;
        }
    }
    #endregion
}
