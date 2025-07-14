using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSlimeBoss : MonoBehaviour
{
    public enum BossState { Slime, Transforming, DemonPatrol, DemonChase, DemonAttack }
    public BossState currentState;

    [Header("Phases")]
    public int slimeFormHitsNeeded = 3;
    public int demonFormMaxHealth = 20;
    public float transformDuration = 2f;
    public Collider2D slimeCollider;
    public Collider2D demonCollider;
    [Header("Attacks")]
    public float[] attackCooldowns = { 1f, 2f, 3f }; 
    private float attackTimer;

    [Header("References")]
    private Boss_Health health;
    private Boss_Movement movement;
    private Animator animator;
    public GameObject cleaveHitbox;
    public GameObject breathHitbox;
    public GameObject smashHitbox;
    public bool isDemon = false;
   public int hitCount;
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    private float hitAnimCooldown = 0.3f;   
    private float lastFlinchTime = -1f;
    void Awake()
    {
        health = GetComponent<Boss_Health>();
        movement = GetComponent<Boss_Movement>();
        animator = GetComponent<Animator>();
        currentState = BossState.Slime;
        slimeCollider.enabled = true;
        demonCollider.enabled = false;
        cleaveHitbox.SetActive(false);
        breathHitbox.SetActive(false);
        smashHitbox.SetActive(false);
    }

    void Update()
    {
        if (health.currentHealth <= 0) return;

        switch (currentState)
        {
            case BossState.Slime:
                // Wait for hits (handled in OnSlimeHit())
                break;

            case BossState.Transforming:
                // Handled by coroutine
                break;

            case BossState.DemonPatrol:
                if (movement.playerDetected)
                    currentState = BossState.DemonChase;
                break;

            case BossState.DemonChase:
                if (movement.InAttackRange())
                    currentState = BossState.DemonAttack;
                    attackTimer = 0f;
                break;

            case BossState.DemonAttack:
                if (attackTimer <= 0)
                {
                    PerformAttack();
                    attackTimer = attackCooldowns[Random.Range(0, attackCooldowns.Length)];
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                    if (!movement.InAttackRange())
                        currentState = BossState.DemonChase;
                }
                break;

        }
    }

    void PerformAttack()
    {
        int attackType = Random.Range(0, 3); // 0=Cleave, 1=Smash, 2=Breath
        animator.SetInteger("AttackType", attackType);
        animator.SetTrigger("Attack");
    }

    public void OnSlimeHit()
    {
        if (isDemon || currentState == BossState.Transforming) return;

        hitCount++;
        animator.SetInteger("HitCount", hitCount);

        if (!animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack") && Time.time - lastFlinchTime > hitAnimCooldown)
        {
            animator.SetTrigger("SlimeHit");
            StartCoroutine(FlashRed());
            lastFlinchTime = Time.time;
        }
        else
        {
            StartCoroutine(FlashRed());
        }

        if (hitCount >= slimeFormHitsNeeded && !isDemon)
        {
            animator.SetTrigger("Transform");
            health.maxHealth = demonFormMaxHealth;
            health.currentHealth = demonFormMaxHealth;
        }
    }

    public void FinishTransformation()
    {
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(0, 0);

        isDemon = true;
        health.maxHealth = demonFormMaxHealth;
        health.currentHealth = demonFormMaxHealth;
        currentState = BossState.DemonPatrol;
        health.EnableHealthBar();

        // Update the slider value as well!
        if (health.healthBar)
        {
            health.healthBar.maxValue = demonFormMaxHealth;
            health.healthBar.value = demonFormMaxHealth;
        }

        // COLLIDER SWITCH!
        slimeCollider.enabled = false;
        demonCollider.enabled = true;
    }


    public void EnableCleaveHitbox()
    {
        cleaveHitbox.SetActive(true);
        Invoke("DisableCleaveHitbox", 0.18f); 
    }
    void DisableCleaveHitbox()
    {
        cleaveHitbox.SetActive(false);
    }
    public void EnableBreathHitbox()
    {
        breathHitbox.SetActive(true);
        Invoke("DisableBreathHitbox", 0.25f);
    }
    void DisableBreathHitbox()
    {
        breathHitbox.SetActive(false);
    }
    public void EnableSmashHitbox()
    {
        smashHitbox.SetActive(true);
        Invoke("DisableSmashHitbox", 0.12f);
    }
    void DisableSmashHitbox()
    {
        smashHitbox.SetActive(false);
    }
    public void OnDemonHit()
    {
        animator.SetTrigger("DemonHit");      // Trigger demon hit anim
        StartCoroutine(FlashRed());           // Also flash red
    }

    IEnumerator FlashRed()
    {
        if (!spriteRenderer) yield break;
        Color original = spriteRenderer.color;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = original;
    }

}