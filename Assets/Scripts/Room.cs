using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject topDoor1;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject bottomDoor1;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject leftDoor1;
    [SerializeField] GameObject rightDoor;
    [SerializeField] GameObject rightDoor1;

    public Vector2Int RoomIndex { get; set; }
  
    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            topDoor.SetActive(true);
            topDoor1.SetActive(true);
        }

        if (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
            bottomDoor1.SetActive(true);
        }

        if (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
            leftDoor1.SetActive(true);
        }

        if (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
            rightDoor1.SetActive(true);
        }
    }
}
