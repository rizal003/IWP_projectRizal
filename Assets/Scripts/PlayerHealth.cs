using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;

    public void ChangeHealth(int amouunt)
    {
        currentHealth += amouunt;

        if (currentHealth <= 0)
        {
            playerSr.enabled = false;
            playerMovement.enabled = false;
        }
    }
}
