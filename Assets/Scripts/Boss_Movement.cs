using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Movement : MonoBehaviour
{
    public float speed =10f;
    public float attackRange = 4f;
    public Transform[] patrolPoints;

    [Header("Debug")]
    public bool playerDetected;
    public bool inAttackRange;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private int patrolIndex = 0;
    private bool isPausing = false;
    private float patrolPauseTimer = 0f;
    public float patrolPauseTime = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        playerDetected = (player != null);
        inAttackRange = playerDetected && Vector2.Distance(transform.position, player.position) <= attackRange;

        if (playerDetected && !inAttackRange)
        {
            ChasePlayer(); // always moves toward player
        }
        else if (!playerDetected)
        {
            Patrol();
        }
        else if (inAttackRange)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isMoving", false);
        }
    }


    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
        anim.SetBool("isMoving", true);

        if (direction.x > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }


    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (isPausing)
        {
            rb.velocity = Vector2.zero;
            patrolPauseTimer += Time.deltaTime;
            anim.SetBool("isMoving", false);
            if (patrolPauseTimer >= patrolPauseTime)
            {
                isPausing = false;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            }
            return;
        }

        Transform target = patrolPoints[patrolIndex];
        Vector2 newPos = Vector2.MoveTowards(rb.position, target.position, speed * Time.deltaTime);
        rb.MovePosition(newPos);
        anim.SetBool("isMoving", true);

        // Flip to face patrol direction (optional)
        if (target.position.x < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            isPausing = true;
            patrolPauseTimer = 0f;
            anim.SetBool("isMoving", false);
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 pushDir = (collision.transform.position - transform.position).normalized;
                float pushStrength = 50f; //
                playerRb.AddForce(pushDir * pushStrength, ForceMode2D.Force);
              
            }
        }
    }

    void Idle()
    {
        anim.SetBool("isMoving", false);
        rb.velocity = Vector2.zero;
    }

    public bool InAttackRange() => inAttackRange;
}
