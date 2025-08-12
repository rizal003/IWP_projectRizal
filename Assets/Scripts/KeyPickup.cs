using System.Collections;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public Sprite keyIconSprite;
    private bool pickedUp = false;

    public void TryPickup(GameObject player)
    {
        if (pickedUp) return;
        pickedUp = true;
        AudioManager.I?.PlayOneShot(AudioManager.I?.keyPickup, 0.9f);

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddKey();
            var popup = FindObjectOfType<PickupPopup>();
            if (popup != null)
            {
                popup.Show(keyIconSprite, "Key Acquired!", "+1 Key");
            }
        }
        Destroy(gameObject); // Remove key from scene
    }
}
