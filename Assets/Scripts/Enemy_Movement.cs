using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    public float attackRange = 2;
    public float attackCooldown = 2.0f;
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer;
    private int facingDirection = -1;

    private EnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    public Transform[] patrolPoints;
    private int patrolIndex = 0;
    public float patrolPauseTime = 1f; 
    private float patrolPauseTimer = 0f;
    private bool isPausingAtPoint = false;
    private bool overrideAnimation = false;

    private RangedEnemyCombat rangedCombat;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Patrol);
        rangedCombat = GetComponent<RangedEnemyCombat>(); 

    }
    void Update()
    {
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.velocity = Vector2.zero;

         
            if (rangedCombat != null && player != null)
            {
                rangedCombat.TryFireAtPlayer(player);
            }
        }
        else if (enemyState == EnemyState.Patrol)
        {
            Patrol();
        }

        CheckForPlayer();
    }


    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange && attackCooldownTimer <= 0f)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (distance > attackRange && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            ChangeState(EnemyState.Patrol);
        }
    }


    void Chase()
    {

        if (player.position.x > transform.position.x && facingDirection == 1 ||
                player.position.x < transform.position.x && facingDirection == -1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        Transform target = patrolPoints[patrolIndex];

        if (isPausingAtPoint)
        {
            rb.velocity = Vector2.zero;

            overrideAnimation = true;
            anim.SetBool("isChasing", false);
            anim.SetBool("isIdle", true);

            patrolPauseTimer += Time.deltaTime;

            if (patrolPauseTimer >= patrolPauseTime)
            {
                patrolPauseTimer = 0f;
                isPausingAtPoint = false;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;

                anim.SetBool("isIdle", false);
                anim.SetBool("isChasing", true);
                overrideAnimation = false;
            }

            return;
        }

        // Moving to patrol point
        Vector2 direction = (target.position - transform.position).normalized;

        // Flip based on X direction
        if ((direction.x > 0 && facingDirection == 1) || (direction.x < 0 && facingDirection == -1))
        {
            Flip();
        }

        rb.velocity = direction * speed;

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            isPausingAtPoint = true;
            rb.velocity = Vector2.zero;
        }
    }


    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void EndAttack()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
            ChangeState(EnemyState.Idle);
        else
            ChangeState(EnemyState.Chasing);
    }


    void ChangeState(EnemyState newState)
    {
        if (overrideAnimation) return; // prevent animation reset while overriding

        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing || enemyState == EnemyState.Patrol)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);

        enemyState = newState;

        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing || enemyState == EnemyState.Patrol)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);
    }


}

public enum EnemyState
{
    Idle,
    Patrol,
    Chasing,
    Attacking,
}