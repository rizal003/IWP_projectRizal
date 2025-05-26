using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject bossRoomPrefab;
    [SerializeField] GameObject shopRoomPrefab;
    [SerializeField] private int maxRooms = 11;
    [SerializeField] private int minRooms = 7;

    int roomWidth = 20;
    int roomHeight = 12;

    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    public static RoomManager Instance;  
    public GameObject player;  

    private Camera mainCamera;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        mainCamera = Camera.main;

        //initialize the 2D grid to track room placement
        roomGrid = new int[gridSizeX, gridSizeY];

        //initialise the queue that stores which rooms to proceed
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeX / 2);

        //generate room from the first/initial room
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        //check if there are rooms to process, room count is below max, and generation hasnt finished
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            // Dequeue the next room to process
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            //to generate rooms for all four directions
            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        //if generating room dosent meet the max, retry again
        else if(roomCount < minRooms)
        {
            Debug.Log("Roomcount was less than min amt of rooms, try again");
            RegenerateRooms();
        }
        //if generating room hits max or min, good
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            generationComplete = true;
        }
    }

    // Moves the player and camera to the next room and position the player infront of the door
    public void MovePlayerToRoom(Vector2Int newRoomIndex, Vector2Int enteredFromDirection)
    {
        // find the room then the next target room that player enters
        GameObject newRoom = roomObjects.Find(room => room.GetComponent<Room>().RoomIndex == newRoomIndex);
        if (newRoom == null)
        {
            Debug.LogWarning("Room not found at " + newRoomIndex);
            return;
        }

        // Move the camera instantly to the new room center
        mainCamera.transform.position = new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, mainCamera.transform.position.z);

        //start at centre
        Vector3 newPos = newRoom.transform.position;

        
        float halfRoomWidth = roomWidth * 0.5f;
        float halfRoomHeight = roomHeight * 0.5f;
        float offset = 3.5f;

        //adjust player position based on which direction they came from
        if (enteredFromDirection == Vector2Int.up)
            newPos += new Vector3(0, -halfRoomHeight + offset, 0);
        else if (enteredFromDirection == Vector2Int.down)
            newPos += new Vector3(0, halfRoomHeight - offset, 0);
        else if (enteredFromDirection == Vector2Int.left)
            newPos += new Vector3(halfRoomWidth - offset, 0, 0);
        else if (enteredFromDirection == Vector2Int.right)
            newPos += new Vector3(-halfRoomWidth + offset, 0, 0);

        player.transform.position = newPos;
    }

    //starts the room generation process from a given starting index
    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;
        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);
    }

    // attemps to generate a room at the specified index
    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms)
            return false;

        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
            return false;

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        GameObject newRoom;

        // Assign a random room type based on a single random value
        float rand = Random.value;
        RoomType randomType = RoomType.Normal;

        if (rand > 0.9f)  // 10% chance Boss room
        {
            randomType = RoomType.Boss;
            newRoom = Instantiate(bossRoomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        }
        else if (rand > 0.6f)  // 30% chance Shop room
        {
            randomType = RoomType.Shop;
            newRoom = Instantiate(shopRoomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        }
        else
        {
            randomType = RoomType.Normal;
            newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        }

        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        newRoom.GetComponent<Room>().SetRoomType(randomType);

        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        return true;
    }


    // reset the room generation, destroy and redo, use to retry for the regeneration
    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        // Neighbours
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // see which doors to open based on the direction
        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            // Neighbouring room to the left
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            // Neighbouring room to the right
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            // Neighbouring room below
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            // Neighbouring room above
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
        }

    }
    //rooom script
    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }

    //count how many adjacent room around the initial room
    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++;   // Left neighbour
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // Right neighbour
        if (y > 0 && roomGrid[x, y - 1] != 0) count++;   // Bottom neighbour
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // Top neighbour

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2),
                              roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }
}
