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

            if (!currentRoom.IsExplored)
            {
                currentRoom.IsExplored = true;

                if (currentRoom.RoomType == RoomType.PressurePlatePuzzle)
                {
                    currentRoom.LockAllDoors(); // Lock until puzzle solved
                }
                else
                {
                    currentRoom.UnlockConnectedDoors(); // Open connected ones immediately
                }
            }

            // ✅ Use the serialized field here
            RoomManager.Instance.MovePlayerToRoom(currentRoom.RoomIndex, entryDirection);
        }
    }
}
