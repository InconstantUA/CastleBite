using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlighter : MonoBehaviour {
    Color hiddenColor = new Color32(0, 0, 0, 0);
    Color visibleColor = new Color32(255, 255, 255, 255);

    public void EnterBrowseMode()
    {
        // make TileHighligter visible
        // enable txt
        //GetComponentInChildren<Text>(true).gameObject.SetActive(true);
        SetColor(visibleColor);
    }

    public void EnterDragMode()
    {
        // make TileHighligter invisible
        // disable txt
        //GetComponentInChildren<Text>(true).gameObject.SetActive(false);
        SetColor(hiddenColor);
    }

    public void EnterAnimationMode()
    {
        // make TileHighligter invisible
        // disable txt
        //GetComponentInChildren<Text>(true).gameObject.SetActive(false);
        SetColor(hiddenColor);
    }

    public void SetColor(Color color)
    {
        // GetComponentInChildren<Text>().color = color;
        // GetComponent<Text>().color = color;
        GetComponent<TextMeshPro>().color = color;
    }

    public void SetToMousePoistion()
    {
        // get Y coords adjustments
        float yAdjustment = Camera.main.transform.position.y;
        // get the remaining adjustment
        while (yAdjustment >= MapManager.Instance.TileSize)
        {
            yAdjustment -= MapManager.Instance.TileSize;
        }
        // Debug.Log("y adjustment " + yAdjustment);
        // Update position
        int x = Mathf.FloorToInt(Input.mousePosition.x / MapManager.Instance.TileSize);
        int y = Mathf.FloorToInt((Input.mousePosition.y + yAdjustment + MapManager.Instance.YAdjustmentConstant) / MapManager.Instance.TileSize);
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0) * MapManager.Instance.TileSize 
            - new Vector3(0, yAdjustment + MapManager.Instance.YAdjustmentConstant, 0)) 
            + new Vector3(MapManager.Instance.TileSize / 2, MapManager.Instance.TileSize / 2, 0);
            // + new Vector3(MapManager.Instance.TileSize / 2 + 0.5f, MapManager.Instance.TileSize / 2 + 1, 0);
        // Debug.Log("Tile: " + x + ";" + y);
    }
}
