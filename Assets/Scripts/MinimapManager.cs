using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public GameObject minimapSquarePrefab; // Assign in Inspector (must have Image component)
    public Transform minimapParent; // The panel in your Canvas
    public Sprite unexploredSprite, exploredSprite, currentSprite;
    public Sprite bossIcon, keyIcon, heartIcon, chestIcon, crownIcon;

    private Dictionary<Vector2Int, GameObject> minimapSquares = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentRoomIndex = Vector2Int.zero;

    public void GenerateMinimap(Dictionary<Vector2Int, RoomType> allRooms, Vector2Int startRoom)
    {
        // Clear previous minimap
        foreach (Transform child in minimapParent) Destroy(child.gameObject);
        minimapSquares.Clear();

        foreach (var kvp in allRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomType type = kvp.Value;
            GameObject square = Instantiate(minimapSquarePrefab, minimapParent);
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x * 24, pos.y * 24); // 24 = icon size/spacing

            // Set default unexplored sprite
            square.GetComponent<Image>().sprite = unexploredSprite;

            // Add overlay icon if needed
            Transform iconHolder = square.transform.Find("Icon");
            if (iconHolder != null)
            {
                Image iconImage = iconHolder.GetComponent<Image>();
                iconImage.enabled = false;

                // Show correct icon based on room type
                if (type == RoomType.Boss)
                {
                    iconImage.sprite = bossIcon;
                    iconImage.enabled = true;
                }
                else if (type == RoomType.Treasure)
                {
                    iconImage.sprite = crownIcon;
                    iconImage.enabled = true;
                }
                else if (type == RoomType.Shop)
                {
                    iconImage.sprite = keyIcon;
                    iconImage.enabled = true;
                }
                else if (type == RoomType.Enemy)
                {
                    iconImage.sprite = heartIcon;
                    iconImage.enabled = true;
                }
                // Add more types as needed...
            }
            minimapSquares.Add(pos, square);
        }

        UpdateMinimap(startRoom, new List<Vector2Int>()); // At start, only current room is explored
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
