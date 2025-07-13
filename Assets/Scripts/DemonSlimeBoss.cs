using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSlimeBoss : MonoBehaviour
{
    [Header("Combat Settings")]
    public int slimeFormHitsNeeded = 3;
    public int demonFormMaxHealth = 10;
    public float transformDuration = 2f;
    public float attackCooldown = 2f;

    [Header("References")]
    private Enemy_Health health;
    private Enemy_Movement movement;
    private Animator animator;
    private Room bossRoom;

    private bool isDemon = false;
    private bool isTransforming = false;
    private float attackTimer;
    private int hitCount;

    void Awake()
    {
        health = GetComponent<Enemy_Health>();
        movement = GetComponent<Enemy_Movement>();
        animator = GetComponent<Animator>();
        bossRoom = GetComponentInParent<Room>();

        movement.patrolPoints = new Transform[0]; // Force chase-only mode
        movement.playerDetectRange = 999f; // Detect player anywhere
    }

    void Start()
    {
        movement.enabled = false;
    }

    public void OnTakeDamage()
    {
        if (health.currentHealth <= 0 || isTransforming) return;

        if (!isDemon)
        {
            hitCount++;
            animator.SetTrigger("SlimeHit");

            if (hitCount >= slimeFormHitsNeeded)
            {
                StartCoroutine(TransformIntoDemon());
            }
        }
        else
        {
            animator.SetTrigger("DemonHit");
        }
    }

    IEnumerator TransformIntoDemon()
    {
        isTransforming = true;
        animator.SetTrigger("Transform");
        movement.enabled = false;

        yield return new WaitForSeconds(transformDuration);

        animator.ResetTrigger("Transform");
        isDemon = true;
        isTransforming = false;

        health.maxHealth = demonFormMaxHealth;
        health.currentHealth = demonFormMaxHealth;
        movement.enabled = true;
        attackTimer = 0f;
    }

    void Update()
    {
        if (!isDemon || health.currentHealth <= 0) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            PerformRandomAttack();
            attackTimer = attackCooldown;
        }
    }

    void PerformRandomAttack()
    {
        if (isTransforming) return;

        // Randomly select between all 3 attacks
        int attackType = Random.Range(0, 3); // 0=Cleave, 1=Smash, 2=Breath

        animator.SetInteger("AttackType", attackType);
        animator.SetTrigger("Attack");
    }

    public void OnDeath()
    {
        if (bossRoom != null) bossRoom.UnlockConnectedDoors();
    }
}