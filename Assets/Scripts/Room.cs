using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    

    private RoomType roomType;
    public RoomType RoomType { get { return roomType; } private set { roomType = value; } }
    public bool IsExplored { get; set; } = false;

    public Vector2Int RoomIndex { get; set; }

    public void SetRoomType(RoomType type)
    {
        RoomType = type;

        switch (RoomType)
        {
            case RoomType.Shop:
                Debug.Log("This is a Shop Room");
                break;
            case RoomType.Boss:
                Debug.Log("This is a Boss Room");
                break;
            case RoomType.Normal:
            default:
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
