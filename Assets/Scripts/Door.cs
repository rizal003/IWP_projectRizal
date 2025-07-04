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
            Vector2Int nextRoomIndex = CurrentRoom.RoomIndex + Direction;
            Room nextRoom = RoomManager.Instance.GetRoomScriptAt(nextRoomIndex);

            if (nextRoom != null)
            {

                RoomManager.Instance.MovePlayerToRoom(nextRoomIndex, Direction);
            }
            else
            {
                Debug.LogWarning("Next room not found!");
            }
        }
    }


}
