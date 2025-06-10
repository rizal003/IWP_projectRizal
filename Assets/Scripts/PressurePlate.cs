using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite unpressedSprite;
    [SerializeField] private Sprite pressedSprite;

    public bool IsPressed => pressingObjects > 0;

    private int pressingObjects = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Pushable"))
        {
            pressingObjects++;
            Debug.Log($"{name} pressed by {other.name} (Count: {pressingObjects})");

            if (spriteRenderer != null && pressedSprite != null)
            {
                spriteRenderer.sprite = pressedSprite;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Pushable"))
        {
            pressingObjects = Mathf.Max(0, pressingObjects - 1);
            Debug.Log($"{name} released by {other.name} (Count: {pressingObjects})");

            if (pressingObjects == 0 && spriteRenderer != null && unpressedSprite != null)
            {
                spriteRenderer.sprite = unpressedSprite;
            }
        }
    }

}
