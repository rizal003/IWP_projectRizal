using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    public RoomType roomType;
    public Vector2Int RoomIndex { get; set; }
    public void SetRoomType(RoomType type)
    {
        roomType = type;

        switch (roomType)
        {
            case RoomType.Shop:
                // You can modify the room's behavior to open a shop, show items, etc.
                Debug.Log("This is a Shop Room");
                break;
            case RoomType.Boss:
                // Boss room setup: large room, boss spawns, etc.
                Debug.Log("This is a Boss Room");
                break;
            case RoomType.Normal:
            default:
                // Normal room setup: enemy spawns, regular room behavior
                Debug.Log("This is a Normal Room");
                break;
        }
    }
    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            topDoor.SetActive(true);
        }

        if (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
        }

        if (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
        }

        if (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
        }
    }
}
