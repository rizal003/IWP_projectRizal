﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private PressurePlate[] plates;
    private bool puzzleSolved = false;

    private void Update()
    {
        if (puzzleSolved) return;

        bool allPressed = true;
        foreach (var plate in plates)
        {

            if (!plate.IsPressed)
            {
                allPressed = false;
                break;
            }
        }

        if (allPressed)
        {
            puzzleSolved = true;
            Debug.Log("Puzzle Solved!");

            Room room = GetComponentInParent<Room>(); 
            if (room != null)
            {
                room.UnlockConnectedDoors();
            }
            else
            {
                Debug.LogWarning("Room component not found on parent.");
            }
        }
    }
}
