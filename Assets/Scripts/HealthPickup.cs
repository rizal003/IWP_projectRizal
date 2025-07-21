using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 2; 
    public Sprite pickupIcon; 

    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        if (other.CompareTag("Player"))
        {
            pickedUp = true;

            // Heal the player
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ChangeHealth(healAmount);
            }

            // Optional: Show pickup popup!
            var popup = FindObjectOfType<PickupPopup>();
            if (popup != null)
            {
                popup.Show(
                    pickupIcon,
                    "Healed!",
                    $"+{healAmount / 2} Heart" + (healAmount > 2 ? "s!" : "!")
                );
            }

            // Destroy this pickup
            Destroy(gameObject);
        }
    }
}
