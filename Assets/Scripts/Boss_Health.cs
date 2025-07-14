using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Health : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 20;
    public int currentHealth;
    public Slider healthBar;
    private float hitCooldown = 0.1f; // seconds
    private float lastHitTime = -1f;
    //[Header("Phases")]
    //public float phase2Threshold = 0.5f; // 50% HP
    //public bool isPhase2 = false;

    [Header("Effects")]
    public Animator animator;
    public GameObject deathEffect;
    public SpriteRenderer spriteRenderer; 
    public Color hitColor = Color.red;    
    public float flashDuration = 0.1f;    
    private CameraShake _cameraShake;     
    private bool isDead = false;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log("Awake called, hiding slider " + healthBar);

        if (healthBar)
            healthBar.gameObject.SetActive(false);  
        currentHealth = maxHealth;
        if (healthBar)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        currentHealth -= damage;
        if (healthBar) healthBar.value = currentHealth;

        if (currentHealth > 0)
        {
            DemonSlimeBoss bossAI = GetComponent<DemonSlimeBoss>();
            if (bossAI != null)
            {
                if (bossAI.isDemon)
                    bossAI.OnDemonHit();
                else
                    bossAI.OnSlimeHit();
            }
            else
            {
                StartCoroutine(FlashRed());
            }
        }
        else
        {
            Die(); // Don't play hit, just die!
        }
    }

    public void SetHealthBar(Slider s)
    {
        healthBar = s;
        if (healthBar)
        {
            // Get the CanvasGroup attached to the slider
            canvasGroup = healthBar.GetComponent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.alpha = 0f; // Hide instantly
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                // fallback in case no CanvasGroup (shouldn't happen)
                healthBar.gameObject.SetActive(false);
            }
        }
    }


    public void EnableHealthBar()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 1f; // Show instantly
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else if (healthBar)
        {
            healthBar.gameObject.SetActive(true);
        }
    }


    IEnumerator FlashRed()
    {
        if (!spriteRenderer) yield break;
        Color original = spriteRenderer.color;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = original;
    }

    void Die()
    {
        if (canvasGroup)
            canvasGroup.alpha = 0f;
        else if (healthBar)
            healthBar.gameObject.SetActive(false);

        isDead = true;
        // Reset other triggers so Die always plays
        animator.ResetTrigger("DemonHit");
        animator.ResetTrigger("SlimeHit");
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Die");
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, 1.5f); // Delay for death animation
    }

}
