using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public GameObject minimapSquarePrefab; 
    public Transform minimapParent; 
    public Sprite unexploredSprite, exploredSprite, currentSprite;
    public Sprite bossIcon, keyIcon, heartIcon, chestIcon, crownIcon;

    private Dictionary<Vector2Int, GameObject> minimapSquares = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentRoomIndex = Vector2Int.zero;
    public static MinimapManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void GenerateMinimap(Dictionary<Vector2Int, RoomType> allRooms, Vector2Int centerRoom)
    {
        foreach (Transform child in minimapParent) Destroy(child.gameObject);
        minimapSquares.Clear();

        // 1. Find bounds
        int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
        foreach (var kvp in allRooms)
        {
            Vector2Int pos = kvp.Key;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }
        int countX = maxX - minX + 1;
        int countY = maxY - minY + 1;

        RectTransform panelRect = minimapParent.GetComponent<RectTransform>();
        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        float margin = 25f;
        float cellSizeX = (panelWidth - margin * 2) / countX;
        float cellSizeY = (panelHeight - margin * 2) / countY;
        float cellSize = Mathf.Min(cellSizeX, cellSizeY);

        // 2. ***CENTER the minimap grid!***
        float mapWidth = countX * cellSize;
        float mapHeight = countY * cellSize;

        float centerOffsetX = -panelWidth / 2 + (panelWidth - mapWidth) / 2 + cellSize / 2;
        float centerOffsetY = -panelHeight / 2 + (panelHeight - mapHeight) / 2 + cellSize / 2;

        // 3. Draw each square
        foreach (var kvp in allRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomType type = kvp.Value;
            GameObject square = Instantiate(minimapSquarePrefab, minimapParent);

            RectTransform rect = square.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(cellSize, cellSize);

            // Relative position from minX/minY
            int gridX = pos.x - minX;
            int gridY = pos.y - minY;

            // Now, center the whole block
            rect.anchoredPosition = new Vector2(
                centerOffsetX + gridX * cellSize,
                centerOffsetY + gridY * cellSize
            );

            // (Your sprite/icon code here)
            square.GetComponent<Image>().sprite = unexploredSprite;

            Transform iconHolder = square.transform.Find("Item");
            if (iconHolder != null)
            {
                Image iconImage = iconHolder.GetComponent<Image>();
                iconImage.enabled = false;
                if (type == RoomType.Boss)
                {
                    iconImage.sprite = bossIcon; iconImage.enabled = true; 
                }
                else if (type == RoomType.Treasure) 
                {
                    iconImage.sprite = chestIcon; iconImage.enabled = true;
                }
                else if (type == RoomType.PressurePlatePuzzle) 
                { 
                    iconImage.sprite = keyIcon; iconImage.enabled = true;
                }
                else if (type == RoomType.Shop)
                {
                    iconImage.sprite = heartIcon; iconImage.enabled = true; 
                }
                else if (type == RoomType.TorchPuzzle)
                {
                    iconImage.sprite = keyIcon; iconImage.enabled = true;
                }
            }
            minimapSquares.Add(pos, square);
        }

        UpdateMinimap(centerRoom, new List<Vector2Int>() { centerRoom });
    }


    public void UpdateMinimap(Vector2Int currentRoom, List<Vector2Int> exploredRooms)
    {
        // Update room color/state for each room
        foreach (var kvp in minimapSquares)
        {
            Image img = kvp.Value.GetComponent<Image>();
            if (currentRoom == kvp.Key)
            {
                img.sprite = currentSprite;
            }
            else if (exploredRooms.Contains(kvp.Key))
            {
                img.sprite = exploredSprite;
            }
            else
            {
                img.sprite = unexploredSprite;
            }
        }
    }
}
