using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityInteractable : MonoBehaviour
{
    public enum InteractableType { Chest, Health, Key }
    public InteractableType type;

    private Chest chest;
    private HealthPickup healthPickup;
    private KeyPickup keyPickup;


    void Awake()
    {
        if (type == InteractableType.Chest)
            chest = GetComponentInParent<Chest>();
        else if (type == InteractableType.Health)
            healthPickup = GetComponentInParent<HealthPickup>();
        else if (type == InteractableType.Key)
            keyPickup = GetComponentInParent<KeyPickup>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player == null) return;

            if (type == InteractableType.Chest)
                player.SetNearbyChest(chest.transform, chest);
            else if (type == InteractableType.Health)
                player.SetNearbyHealth(healthPickup.transform, healthPickup);
            else if (type == InteractableType.Key)
                player.SetNearbyKey(keyPickup.transform, keyPickup);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player == null) return;

            if (type == InteractableType.Chest)
                player.SetNearbyChest(null, null);
            else if (type == InteractableType.Health)
                player.SetNearbyHealth(null, null);
            else if (type == InteractableType.Key)
                player.SetNearbyKey(null, null);
        }
    }

}
