using System.Collections;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public Sprite keyIconSprite; // <--- Add this line!

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey();
                var popup = FindObjectOfType<PickupPopup>();
                if (popup != null)
                {
                    // Now you can assign this in the inspector
                    popup.Show(keyIconSprite, "Key Acquired!", "+1 Key");
                }
                Destroy(gameObject); // Remove key from scene
            }
        }
    }
}
