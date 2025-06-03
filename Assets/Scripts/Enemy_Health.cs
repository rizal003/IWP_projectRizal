using System.Collections;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth = 3;
    private Animator animator;
    private bool isDead = false;
    public SpriteRenderer spriteRenderer; 
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    private Color originalColor;

    void Start()
    {
        animator = GetComponent<Animator>();
        originalColor = spriteRenderer.color;

    }
    public void FlashRed()
    {
        StopAllCoroutines(); 
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }


    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        animator.SetTrigger("Hit");
        FlashRed(); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }



    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        // Optional: Destroy after animation
        Destroy(gameObject, 1.5f); // adjust time to match your animation length
    }
}
