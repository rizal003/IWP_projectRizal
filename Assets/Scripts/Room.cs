using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [Header("Enemy Spawning")]
    [SerializeField] private GameObject enemyPrefab; // assign your enemy prefab here
    [SerializeField] private Transform patrolPointsParent; // assign the PatrolPoints object here

    private RoomType roomType;
    public RoomType RoomType { get { return roomType; } private set { roomType = value; } }
    public bool IsExplored { get; set; } = false;
    public Vector2Int RoomIndex { get; set; }

    private void Start()
    {
        if (roomType == RoomType.Normal)
        {
            SpawnEnemyWithPatrol();
        }
    }

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
            case RoomType.Enemy:
                Debug.Log("This is an Enemy Room");
                break;
            case RoomType.Normal:
            default:
                Debug.Log("This is a Normal Room");
                break;
        }
    }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) topDoor.SetActive(true);
        if (direction == Vector2Int.down) bottomDoor.SetActive(true);
        if (direction == Vector2Int.left) leftDoor.SetActive(true);
        if (direction == Vector2Int.right) rightDoor.SetActive(true);
    }

    private void SpawnEnemyWithPatrol()
    {
        if (enemyPrefab == null || patrolPointsParent == null) return;

        // Collect patrol points
        Transform[] patrolPoints = new Transform[patrolPointsParent.childCount];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPoints[i] = patrolPointsParent.GetChild(i);
        }

        // Shuffle patrol points to avoid overlap
        System.Random rng = new System.Random();
        for (int i = patrolPoints.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (patrolPoints[i], patrolPoints[j]) = (patrolPoints[j], patrolPoints[i]);
        }

        // Spawn 3 enemies or up to the number of patrol points
        int spawnCount = Mathf.Min(4, patrolPoints.Length);
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, patrolPoints[i].position, Quaternion.identity, transform);

            Enemy_Movement em = enemy.GetComponent<Enemy_Movement>();
            if (em != null)
            {
                em.patrolPoints = patrolPoints;
            }
        }
    }

    public void LockAllDoors()
    {
        if (topDoor != null) topDoor.SetActive(false);
        if (bottomDoor != null) bottomDoor.SetActive(false);
        if (leftDoor != null) leftDoor.SetActive(false);
        if (rightDoor != null) rightDoor.SetActive(false);
    }
    public void UnlockConnectedDoors()
    {
        RoomManager rm = RoomManager.Instance;

        if (rm.GetRoomScriptAt(RoomIndex + Vector2Int.left) != null)
            leftDoor.SetActive(true);
        if (rm.GetRoomScriptAt(RoomIndex + Vector2Int.right) != null)
            rightDoor.SetActive(true);
        if (rm.GetRoomScriptAt(RoomIndex + Vector2Int.up) != null)
            topDoor.SetActive(true);
        if (rm.GetRoomScriptAt(RoomIndex + Vector2Int.down) != null)
            bottomDoor.SetActive(true);
    }



}
