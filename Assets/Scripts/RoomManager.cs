using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject bossRoomPrefab;
    [SerializeField] GameObject shopRoomPrefab;
    [SerializeField] GameObject treasureRoomPrefab;  
    [SerializeField] GameObject PressurePlatePuzzlePrefab;     
    [SerializeField] GameObject VampireRoomPrefab;
    [SerializeField] GameObject tutorialRoomPrefab;
    [SerializeField] GameObject torchPuzzleRoomPrefab;


    [SerializeField] private int maxRooms = 11;
    [SerializeField] private int minRooms = 7;
    [SerializeField] GameObject iceTilePrefab;
    [SerializeField] GameObject lavaTilePrefab;
    [SerializeField] GameObject underwaterTilePrefab;
    // Add more as needed

    int roomWidth = 20;
    int roomHeight = 12;

    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, RoomType> allRoomsDict = new Dictionary<Vector2Int, RoomType>();
    private List<Vector2Int> exploredRooms = new List<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    public static RoomManager Instance;  
    public GameObject player;
    public Room CurrentRoom { get; private set; }

    private Camera mainCamera;
    public bool IsExplored { get; set; } = false;
    public MinimapManager minimapManager;
    private bool spawnedTreasureRoom = false;
   
    private bool spawnedPressurePlatePuzzleRoom = false;
    private bool spawnedTorchPuzzleRoom = false;
 

    private int shopRoomCount = 0;

    private int minShopRooms = 2; // for minimum shops, if needed
    public EnvironmentType environmentType = EnvironmentType.Normal; 

    private void Awake()
    {
        Instance = this;
    }
    private EnvironmentType GetEnvironmentTypeForFloor(int floor)
    {
        if (floor == 1 || floor == 2)
            return EnvironmentType.Normal;
        else if 
            (floor == 3 || floor == 4)
            return EnvironmentType.Ice;
        else if 
            (floor == 5 || floor == 6)
            return EnvironmentType.Lava;
        else if 
            (floor == 7 || floor == 8)
            return EnvironmentType.Underwater;
        else
            return EnvironmentType.Normal; 
    }


    private void Start()
    {
        mainCamera = Camera.main;
        int floor = GameManager.Instance != null ? GameManager.Instance.floorNumber : 1;
        environmentType = GetEnvironmentTypeForFloor(floor);
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
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            generationComplete = true;
            AssignBossRoom();
            GenerateMinimapForPlayer();
            MovePlayerToStartRoom();
        }


    }
    public void MovePlayerToStartRoom()
    {
        Vector2Int startRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        GameObject startRoomObj = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == startRoomIndex);

        if (startRoomObj == null)
        {
            Debug.LogWarning("Start room not found!");
            return;
        }

        Transform spawn = startRoomObj.transform.Find("SpawnPoint");
        if (spawn != null && player != null)
        {
            player.transform.position = spawn.position;
            Debug.Log($"Player moved to SpawnPoint at position: {spawn.position}");
        }
        else if (player != null)
        {
            player.transform.position = startRoomObj.transform.position;
            Debug.Log($"Player moved to Room center at position: {startRoomObj.transform.position}");
        }
        else
        {
            Debug.LogWarning("Player GameObject is null.");
            return;
        }

        if (Camera.main != null && player != null)
        {
            Camera.main.transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                Camera.main.transform.position.z);
            Debug.Log($"Camera moved to position: {Camera.main.transform.position}");
        }
        else
        {
            Debug.LogWarning("Main Camera or Player is null, cannot move camera.");
        }
    }



    private void GenerateMinimapForPlayer()
    {
        // Collect all rooms and their types
        Dictionary<Vector2Int, RoomType> allRooms = new Dictionary<Vector2Int, RoomType>();
        foreach (GameObject roomObj in roomObjects)
        {
            Room room = roomObj.GetComponent<Room>();
            if (room != null)
            {
                allRooms.Add(room.RoomIndex, room.RoomType);
            }
        }
        Vector2Int startRoom = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        MinimapManager.Instance.GenerateMinimap(allRooms, startRoom);
    }

    IEnumerator SlideCamera(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Ease in-out (smoothstep)
            t = t * t * (3f - 2f * t);
            mainCamera.transform.position = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = to;
    }

    public void MovePlayerToRoom(Vector2Int newRoomIndex, Vector2Int enteredFromDirection)
    {
    
        // Find the new room
        GameObject newRoom = roomObjects.Find(room => room.GetComponent<Room>().RoomIndex == newRoomIndex);
        if (newRoom == null)
        {
            Debug.LogWarning("Room not found at " + newRoomIndex);
            return;
        }

        // Calculate new camera position
        Vector3 newCameraPos = new Vector3(
            newRoom.transform.position.x,
            newRoom.transform.position.y,
            mainCamera.transform.position.z
        );
      ;

        StopAllCoroutines(); // Stops any ongoing camera slide
        StartCoroutine(SlideCamera(mainCamera.transform.position, newCameraPos, 0.35f));
        // Update camera shake with new room position
        CameraShake cameraShake = mainCamera.GetComponent<CameraShake>();
        if (cameraShake != null)
        {
            cameraShake.SetRoomCenter(newCameraPos);
        }
        else
        {
            mainCamera.transform.position = newCameraPos;
        }

        Room roomScript = newRoom.GetComponent<Room>();
        Transform spawnPoint = roomScript.GetSpawnPoint(-enteredFromDirection);

        // Place the player at the correct spawn point if found, or fallback to room center
        if (spawnPoint != null)
            player.transform.position = spawnPoint.position;
        else
            player.transform.position = newRoom.transform.position;


        // Mark this room as explored
        exploredRooms.Add(newRoomIndex);

        // Update the minimap (after moving the player)
        MinimapManager.Instance.UpdateMinimap(newRoomIndex, new List<Vector2Int>(exploredRooms));

    }
    public void SetCurrentRoom(Room room)
    {
        CurrentRoom = room;
        // Track explored room if not already in the list
        if (!exploredRooms.Contains(room.RoomIndex))
            exploredRooms.Add(room.RoomIndex);

        if (MinimapManager.Instance != null)
            MinimapManager.Instance.UpdateMinimap(room.RoomIndex, exploredRooms);
    }
    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        if (roomGrid[roomIndex.x, roomIndex.y] != 0)
        {
            Debug.LogWarning("Tried to start generation from an already occupied room position.");
            return;
        }

        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;

        var initialRoom = Instantiate(tutorialRoomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        initialRoom.GetComponent<Room>().SetRoomType(RoomType.Tutorial); // Set as tutorial type
        roomObjects.Add(initialRoom);

        allRoomsDict[roomIndex] = RoomType.Tutorial;


    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomGrid[x, y] != 0 || roomCount >= maxRooms)
            return false;

        // Skip random checks for the origin room — already handled in StartRoomGenerationFromRoom
        if (roomIndex != Vector2Int.zero)
        {
            if (Random.value < 0.5f)
                return false;

            if (CountAdjacentRooms(roomIndex) > 1)
                return false;
        }

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        RoomType chosenType;

        // Spawn a PressurePlatePuzzle in the starting room
        if (roomIndex == Vector2Int.zero)
        {
            chosenType = RoomType.PressurePlatePuzzle;
        }
        else
        {
            int roomsLeft = maxRooms - roomCount;

            if (!spawnedTreasureRoom && roomsLeft <= 4)
                chosenType = RoomType.Treasure;
            else if (!spawnedPressurePlatePuzzleRoom && roomsLeft <= 3)
                chosenType = RoomType.PressurePlatePuzzle;
            else if (!spawnedTorchPuzzleRoom && roomsLeft <= 2)
                chosenType = RoomType.TorchPuzzle;
            else if (shopRoomCount < minShopRooms && roomsLeft <= (minShopRooms - shopRoomCount))
                chosenType = RoomType.Shop;
            else
            {
                float rand = Random.value;
                if (rand > 0.95f)
                    chosenType = RoomType.Treasure;               // 0.95–1.00: 5%
                else if (rand > 0.90f)
                    chosenType = RoomType.PressurePlatePuzzle;    // 0.90–0.95: 5%
                else if (rand > 0.85f)
                    chosenType = RoomType.TorchPuzzle;            // 0.85–0.90: 5%
                else if (rand > 0.70f)
                    chosenType = RoomType.Shop;                   // 0.70–0.85: 15%
                else if (rand > 0.35f)
                    chosenType = RoomType.Vampire;                // 0.35–0.70: 35%
                else
                    chosenType = RoomType.Normal;                 // 0.00–0.35: 35%
            }

        }

        GameObject prefabToSpawn;

        switch (chosenType)
        {
            case RoomType.Treasure:
                prefabToSpawn = treasureRoomPrefab;
                break;
            case RoomType.Vampire:
                prefabToSpawn = VampireRoomPrefab;
                break;
            case RoomType.Shop:
                prefabToSpawn = shopRoomPrefab;
                break;
            case RoomType.PressurePlatePuzzle:
                prefabToSpawn = PressurePlatePuzzlePrefab;
                break;
            case RoomType.TorchPuzzle:
                prefabToSpawn = torchPuzzleRoomPrefab;
                break;
            case RoomType.Normal:
            default:
                prefabToSpawn = roomPrefab;
                break;
        }

        allRoomsDict[roomIndex] = chosenType;
        GameObject newRoom = Instantiate(prefabToSpawn, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        newRoom.GetComponent<Room>().SetRoomType(chosenType);

        if (chosenType == RoomType.Treasure)
            spawnedTreasureRoom = true;
        if (chosenType == RoomType.PressurePlatePuzzle)
            spawnedPressurePlatePuzzleRoom = true;
        if (chosenType == RoomType.TorchPuzzle)
            spawnedTorchPuzzleRoom = true;
        if (chosenType == RoomType.Shop)
            shopRoomCount++;

        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y); // Always open doors on both sides!

        return true;
    }

    private void AssignBossRoom()
    {
        Vector2Int startRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        GameObject furthestRoom = null;
        int maxDistance = -1;

        foreach (var roomObj in roomObjects)
        {
            Vector2Int index = roomObj.GetComponent<Room>().RoomIndex;
            int dist = Mathf.Abs(index.x - startRoomIndex.x) + Mathf.Abs(index.y - startRoomIndex.y);

            if (dist > maxDistance)
            {
                maxDistance = dist;
                furthestRoom = roomObj;
            }
        }

        if (furthestRoom != null)
        {
            // Replace the room prefab with boss room prefab or update the type
            Vector2Int bossRoomIndex = furthestRoom.GetComponent<Room>().RoomIndex;

            // Destroy the current furthest room GameObject
            Destroy(furthestRoom);

            // Instantiate the boss room prefab at the same position
            var newBossRoom = Instantiate(bossRoomPrefab, GetPositionFromGridIndex(bossRoomIndex), Quaternion.identity);
            Room bossRoomScript = newBossRoom.GetComponent<Room>();
            bossRoomScript.RoomIndex = bossRoomIndex;
            bossRoomScript.name = "BossRoom";
            bossRoomScript.SetRoomType(RoomType.Boss);  


            roomObjects.Remove(furthestRoom);
            roomObjects.Add(newBossRoom);
        }
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
        spawnedTreasureRoom = false;
        spawnedPressurePlatePuzzleRoom = false;
        spawnedTorchPuzzleRoom = false;
        shopRoomCount = 0;


        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

        // Add this (after rooms have been re-created)
        if (MinimapManager.Instance != null)
        {
            // Create a dictionary of all room positions and their types
            Dictionary<Vector2Int, RoomType> allRooms = new Dictionary<Vector2Int, RoomType>();
            foreach (var obj in roomObjects)
            {
                Room r = obj.GetComponent<Room>();
                if (r != null)
                    allRooms[r.RoomIndex] = r.RoomType;
            }
            MinimapManager.Instance.GenerateMinimap(allRooms, initialRoomIndex);
        }
    }

    public void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        // Neighbours
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // LEFT
        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            if (newRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                newRoomScript.OpenDoor(Vector2Int.left);

            if (leftRoomScript != null && leftRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                leftRoomScript.OpenDoor(Vector2Int.right);
        }

        // RIGHT
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            if (newRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                newRoomScript.OpenDoor(Vector2Int.right);

            if (rightRoomScript != null && rightRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                rightRoomScript.OpenDoor(Vector2Int.left);
        }

        // BOTTOM
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            if (newRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                newRoomScript.OpenDoor(Vector2Int.down);

            if (bottomRoomScript != null && bottomRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                bottomRoomScript.OpenDoor(Vector2Int.up);
        }

        // TOP
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            if (newRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                newRoomScript.OpenDoor(Vector2Int.up);

            if (topRoomScript != null && topRoomScript.RoomType != RoomType.PressurePlatePuzzle)
                topRoomScript.OpenDoor(Vector2Int.down);
        }

    }
    //rooom script
   public Room GetRoomScriptAt(Vector2Int index)
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
public enum EnvironmentType
{
    Normal,
    Lava,
    Ice,
    Underwater,
    VisionCone
}
