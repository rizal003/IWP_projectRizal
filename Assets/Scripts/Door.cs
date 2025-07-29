using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int Direction;  // e.g. (0,1) for up, (0,-1) for down
    public Room CurrentRoom;
    public bool isExitDoor = false; // Set to true for top door in boss room

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isExitDoor && CurrentRoom.isBossRoom && CurrentRoom.bossDefeated)
            {
                Debug.Log("Player entered boss exit, going to next floor...");
                GameManager.Instance.NextFloor();
                return;
            }


            Vector2Int nextRoomIndex = CurrentRoom.RoomIndex + Direction;
            Room nextRoom = RoomManager.Instance.GetRoomScriptAt(nextRoomIndex);

            if (nextRoom != null)
            {
                // Pass -Direction here!
                RoomManager.Instance.MovePlayerToRoom(nextRoomIndex, -Direction);
            }
            else
            {
                Debug.LogWarning("Next room not found!");
            }
        }
    }

}
