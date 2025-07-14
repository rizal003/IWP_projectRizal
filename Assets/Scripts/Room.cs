using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [Header("Enemy Spawning")]
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private Transform patrolPointsParent; 
    [SerializeField] private GameObject vampirePrefab;
    [SerializeField] private GameObject bossPrefab;
    private RoomType roomType;
    public RoomType RoomType { get { return roomType; } private set { roomType = value; } }
    public bool IsExplored { get; set; } = false;
    public Vector2Int RoomIndex { get; set; }
    private static bool vampireSpawned = false;
    public bool spawnVampireHere = false;
    private void Start()
    {
        Debug.Log($"Room initialization - Type: {roomType}, VampirePrefab: {vampirePrefab != null}, PatrolPoints: {patrolPointsParent != null}");

        if (roomType == RoomType.Normal)
        {
            SpawnEnemyWithPatrol();
        }
        else if (roomType == RoomType.Vampire)
        {
            Debug.Log($"Attempting to spawn vampires in vampire room. Prefab: {vampirePrefab != null}, Points: {patrolPointsParent?.childCount ?? 0}");
            SpawnVampireWithSpecificPatrol();
        }
        else if (roomType == RoomType.Boss)  
        {
            SpawnBoss();
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
        int spawnCount = Mathf.Min(3, patrolPoints.Length);
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

    private void SpawnVampireWithSpecificPatrol()
    {
        Debug.Log("Attempting to spawn vampires...");

        if (vampirePrefab == null)
        {
            Debug.LogWarning("Cannot spawn vampire - vampirePrefab is not assigned!");
            return;
        }

        if (patrolPointsParent == null)
        {
            Debug.LogWarning("Cannot spawn vampire - patrolPointsParent is not assigned!");
            return;
        }

        Debug.Log($"Patrol points available: {patrolPointsParent.childCount}");

        // Collect patrol points for vampires - using first 4 points (0-3) for two vampires
        List<Transform> vampirePatrolList = new List<Transform>();
        for (int i = 0; i < Mathf.Min(4, patrolPointsParent.childCount); i++) // Get first 4 points
        {
            vampirePatrolList.Add(patrolPointsParent.GetChild(i));
            Debug.Log($"Added patrol point {i}");
        }

        // Need at least 2 patrol points per vampire
        if (vampirePatrolList.Count < 4)
        {
            Debug.LogWarning($"Not enough patrol points for vampires (needs 4, found {vampirePatrolList.Count})");
            return;
        }

        // Spawn two vampires
        for (int vampireCount = 0; vampireCount < 2; vampireCount++)
        {
            // Use points 0-1 for first vampire, 2-3 for second
            Transform[] vampirePoints = new Transform[2] {
            vampirePatrolList[vampireCount * 2],
            vampirePatrolList[vampireCount * 2 + 1]
        };

            Debug.Log($"Spawning vampire {vampireCount + 1} at position: {vampirePoints[0].position}");
            GameObject vampire = Instantiate(vampirePrefab, vampirePoints[0].position, Quaternion.identity, transform);

            if (vampire == null)
            {
                Debug.LogError("Failed to instantiate vampire prefab!");
                continue;
            }

            // Set up movement
            Enemy_Movement em = vampire.GetComponent<Enemy_Movement>();
            if (em != null)
            {
                em.patrolPoints = vampirePoints;
                em.attackRange = 5f;
                em.playerDetectRange = 7f;
                em.speed = 2f;
                Debug.Log($"Enemy_Movement configured for vampire {vampireCount + 1}");
            }
            else
            {
                Debug.LogWarning("Vampire prefab is missing Enemy_Movement component!");
            }

            // Set up combat
            RangedEnemyCombat rec = vampire.GetComponent<RangedEnemyCombat>();
            if (rec != null)
            {
                rec.fireRange = 5f;
                rec.projectileSpeed = 8f;
                rec.fireCooldown = 2f;
                Debug.Log($"RangedEnemyCombat configured for vampire {vampireCount + 1}");
            }
            else
            {
                Debug.LogWarning("Vampire prefab is missing RangedEnemyCombat component!");
            }
        }

        Debug.Log("2 Vampires spawned successfully!");
    }
    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError("Boss prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;
        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity, transform);
        boss.SetActive(false); // Boss starts inactive until triggered

        // ----> Find the health bar slider in the scene and assign it:
        Slider bossSlider = GameObject.Find("Slider 1").GetComponent<Slider>();
        Boss_Health bossHealth = boss.GetComponent<Boss_Health>();
        if (bossHealth != null && bossSlider != null)
        {
            bossHealth.SetHealthBar(bossSlider);
            Debug.Log("Assigned boss slider reference to boss health script.");
        }
        else
        {
            Debug.LogWarning("Couldn't find boss slider or boss health script!");
        }

        LockAllDoors();
        Debug.Log("Boss spawned but inactive.");
    }

    public void ActivateBoss()
    {
        DemonSlimeBoss boss = GetComponentInChildren<DemonSlimeBoss>(true);
        if (boss != null)
        {
            boss.gameObject.SetActive(true);  // Activate GameObject itself!
        
            Debug.Log("Boss GameObject and components activated!");
        }
        else
        {
            Debug.LogError("No boss found in room!");
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
