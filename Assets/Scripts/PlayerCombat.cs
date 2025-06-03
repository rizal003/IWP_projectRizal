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


    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
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

        attackCooldownTimer = attackCooldown; // Reset cooldown

        // Get last direction from movement script
        lastDirection = playerMovement.GetLastDirection();

        int direction = 3; // default = down

        if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
        {
            direction = lastDirection.x > 0 ? 0 : 1; // 0 = right, 1 = left
        }
        else
        {
            direction = lastDirection.y > 0 ? 2 : 3; // 2 = up, 3 = down
        }

        animator.SetInteger("AttackDirection", direction);
        animator.SetTrigger("AttackTrigger");
        animator.Play("Attack" + DirectionToString(direction));
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            // You need to have a script like EnemyHealth with TakeDamage(int dmg)
            enemy.GetComponent<Enemy_Health>().TakeDamage(1);
        }
    }

    private string DirectionToString(int dir)
    {
        return dir switch
        {
            0 => "Right",
            1 => "Left",
            2 => "Up",
            3 => "Down",
            _ => "Down"
        };
    }
}
