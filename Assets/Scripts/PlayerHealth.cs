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
    private const int DEATH_RIGHT = 0;
    private const int DEATH_LEFT = 1;
    private const int DEATH_UP = 2;
    private const int DEATH_DOWN = 3;
    private CameraShake _cameraShake;
    public PlayerStats playerStats; 

    void Start()
    {
        _cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();
        maxHealth = playerStats.maxHealth; 
        currentHealth = maxHealth;         
    }
    public void ChangeHealth(int amount)
    {
        if (amount < 0 && _cameraShake != null)
        {
            _cameraShake.Shake(0.3f, 0.35f); 
        }
        currentHealth += amount;
        maxHealth = playerStats.maxHealth; 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Vector2 lastDir = playerMovement.GetLastDirection();

        int deathDir = DEATH_DOWN; 

        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
        {
            deathDir = lastDir.x > 0 ? DEATH_RIGHT : DEATH_LEFT;
        }
        else
        {
            deathDir = lastDir.y > 0 ? DEATH_UP : DEATH_DOWN;
        }

        // Set the death direction parameter
        animator.SetInteger("DeathDirection", deathDir);
        // Trigger the death animation
        animator.SetTrigger("Die");

        playerMovement.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false; 
    }

}
