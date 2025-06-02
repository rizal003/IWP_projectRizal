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
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);

    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.velocity = Vector2.zero;
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

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
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


            else if (distance > attackRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }
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
        //exit the current anim
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);

        //update current state
        enemyState = newState;

        //update new anim
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);
    }

}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
}