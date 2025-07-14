using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
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
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || attackCooldownTimer > 0f) return;

        attackCooldownTimer = attackCooldown;

        // Get last direction
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
            Debug.Log("Hit: " + enemy.name);

            // First check for Enemy_Health
            Enemy_Health eh = enemy.GetComponentInParent<Enemy_Health>();
            if (eh != null)
            {
                eh.TakeDamage(1);
                continue; // Skip to next enemy after damaging
            }

            // Now check for Boss_Health
            Boss_Health bh = enemy.GetComponentInParent<Boss_Health>();
            if (bh != null)
            {
                bh.TakeDamage(1);
                continue;
            }
        }
    }



    public void SpawnSlash()
    {
        GameObject slash = Instantiate(slashPrefab, attackPoint.position, Quaternion.identity);
        slash.GetComponent<SlashProjectile>().direction = lastDirection.normalized;
    }

  
}
