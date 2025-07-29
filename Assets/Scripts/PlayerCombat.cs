using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    private Vector2 lastDirection = Vector2.down;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.5f;
    private float attackCooldownTimer = 0f;
    public GameObject slashPrefab;

    public float hitStopDuration = 0.05f;
    private CameraShake _cameraShake;
    public PlayerStats playerStats; 

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        _cameraShake = Camera.main?.GetComponent<CameraShake>(); 

    }
    void Update()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        UpdateAttackPoint(playerMovement.GetLastDirection());
    }


    void UpdateAttackPoint(Vector2 direction)
    {
        direction = direction.normalized;
        float distance = 0.5f; 
        attackPoint.localPosition = new Vector3(direction.x, direction.y, 0) * distance;
        Debug.DrawRay(attackPoint.position, lastDirection * attackRange, Color.red, 1f);

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || attackCooldownTimer > 0f) return;

        attackCooldownTimer = attackCooldown;
        lastDirection = playerMovement.GetLastDirection();

  

        int direction = 3; // default = down
        if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            direction = lastDirection.x > 0 ? 0 : 1; // 0 = right, 1 = left
        else
            direction = lastDirection.y > 0 ? 2 : 3; // 2 = up, 3 = down

        // Trigger the correct attack animation through the Animator
        animator.SetInteger("AttackDirection", direction);
        animator.SetTrigger("AttackTrigger");

     
    }


    public void DealDamage()
    {
        _cameraShake?.Shake(0.1f, 0.15f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Check if the enemy is in front of the player
            Vector2 enemyDirection = (enemy.transform.position - transform.position).normalized;
            float dotProduct = Vector2.Dot(lastDirection.normalized, enemyDirection);

            if (dotProduct > 0.7f) 
            {
                // First check for Enemy_Health
                Enemy_Health eh = enemy.GetComponentInParent<Enemy_Health>();
                if (eh != null)
                {
                    eh.TakeDamage(Mathf.RoundToInt(playerStats.attackDamage));
                    continue;
                }

                // Now check for Boss_Health
                Boss_Health bh = enemy.GetComponentInParent<Boss_Health>();
                if (bh != null)
                {
                    bh.TakeDamage(Mathf.RoundToInt(playerStats.attackDamage));
                    continue;
                }
            }
        }
    }



    public void SpawnSlash()
    {
        GameObject slash = Instantiate(slashPrefab, attackPoint.position, Quaternion.identity);
        var slashProj = slash.GetComponent<SlashProjectile>();
        slashProj.direction = lastDirection.normalized;

        // Set slash speed from player stats
        slashProj.speed = playerStats.slashSpeed;
    }



}
