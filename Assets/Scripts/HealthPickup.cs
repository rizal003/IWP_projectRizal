using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 2;
    public Sprite pickupIcon;

    private bool pickedUp = false;

    public void TryPickup(GameObject player)
    {
        if (pickedUp) return;
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null && health.currentHealth < health.maxHealth)
        {
            pickedUp = true;
            health.ChangeHealth(healAmount);
            // Show popup if desired
            var popup = FindObjectOfType<PickupPopup>();
            if (popup != null)
            {
                popup.Show(
                    pickupIcon,
                    "Healed!",
                    $"+{healAmount / 2} Heart" + (healAmount > 2 ? "s!" : "!")
                );
            }
            Destroy(gameObject);
        }
    }

}
