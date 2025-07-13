using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private Room room;
    [SerializeField] private Vector2Int entryDirection;  // This is correctly declared

    private void Awake()
    {
        room = GetComponent<Room>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Room currentRoom = GetComponentInParent<Room>();
            RoomManager.Instance.SetCurrentRoom(currentRoom);

            if (!currentRoom.IsExplored)
            {
                currentRoom.IsExplored = true;

                if (currentRoom.RoomType == RoomType.Boss)
                {
                    currentRoom.ActivateBoss();
                    currentRoom.LockAllDoors();
                    Debug.Log("Player entered boss room, activated boss.");
                }

                else if (currentRoom.RoomType == RoomType.PressurePlatePuzzle)
                {
                    currentRoom.LockAllDoors();
                }
                else
                {
                    currentRoom.UnlockConnectedDoors();
                }
            }

            RoomManager.Instance.MovePlayerToRoom(currentRoom.RoomIndex, entryDirection);
        }
    }
}
