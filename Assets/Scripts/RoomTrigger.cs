using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private Room room;

    private void Awake()
    {
        room = GetComponent<Room>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            room.IsExplored = true;
            Debug.Log($"Room {room.RoomIndex} marked as explored.");
        }
    }

}

