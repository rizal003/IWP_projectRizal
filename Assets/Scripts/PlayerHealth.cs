using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;
    public Animator animator;

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Vector2 lastDir = playerMovement.GetLastDirection();

        int deathDir = 3; // Default: down

        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
        {
            deathDir = lastDir.x > 0 ? 0 : 1; // right : left
        }
        else
        {
            deathDir = lastDir.y > 0 ? 2 : 3; // up : down
        }

        animator.SetInteger("DeathDirection", deathDir);
        animator.SetTrigger("DeathTrigger");

        playerMovement.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        // Optionally disable SpriteRenderer here if needed later
    }
}
