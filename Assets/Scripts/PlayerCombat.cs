using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    private Vector2 lastDirection = Vector2.down; // default facing down

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

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
