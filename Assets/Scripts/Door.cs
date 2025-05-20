using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int Direction;  // e.g. (0,1) for up, (0,-1) for down
    public Room CurrentRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell RoomManager or GameManager to move player to next room
            RoomManager.Instance.MovePlayerToRoom(CurrentRoom.RoomIndex + Direction, Direction);
        }
    }
}
