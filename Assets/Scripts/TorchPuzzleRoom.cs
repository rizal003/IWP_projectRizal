using System.Collections.Generic;
using UnityEngine;

public class TorchPuzzleRoom : MonoBehaviour
{
    public List<Torch> torches = new List<Torch>();
    public GameObject keyPrefab;
    public Transform keySpawnPoint;
    private bool puzzleSolved = false;

    public Room room; // Assign in inspector, or auto-find

    void Awake()
    {
        // Auto-populate torches if not assigned in inspector
        if (torches.Count == 0)
        {
            torches.Clear();
            foreach (Torch t in GetComponentsInChildren<Torch>())
                torches.Add(t);
        }
        if (room == null)
            room = GetComponentInParent<Room>();
    }


    // Called by Torch when it is lit
    public void RegisterTorchLit(Torch litTorch)
    {
        CheckPuzzleComplete();
    }

    private void CheckPuzzleComplete()
    {
        if (puzzleSolved) return;


        int litCount = 0;
        foreach (Torch t in torches)
        {
            if (!t.isLit)
                return; // Puzzle not solved yet
            litCount++;
        }


        puzzleSolved = true;
        AudioManager.I?.PlayOneShot(AudioManager.I?.puzzleSolved, 1f);
        Room room = GetComponentInParent<Room>();
        if (room != null)
        {
            room.UnlockConnectedDoors();
        }
        else
        {
            Debug.LogWarning("Room component not found on parent.");
        }
        // Spawn the key at the designated location
        if (keyPrefab != null && keySpawnPoint != null)
            Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);
    }

}
