using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Vector2 lastDirection = Vector2.down;
    private bool isKnockedBack;
    [SerializeField] private LayerMask pushableLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();    
    }

    void Update()
    {
        if (isKnockedBack) return;

        Vector2 targetVelocity = moveInput * moveSpeed;

        if (moveInput != Vector2.zero)
        {
            // Try to detect a pushable object in front
            RaycastHit2D hit = Physics2D.Raycast(rb.position, moveInput, 0.6f, pushableLayer);
            if (hit.collider != null && hit.collider.CompareTag("Pushable"))
            {
                Rigidbody2D pushRb = hit.collider.attachedRigidbody;
                if (pushRb != null)
                {
                    float pushSpeed = 2f;

                    // Apply force instead of MovePosition for smoother response
                    Vector2 pushDir = moveInput.normalized;
                    pushRb.velocity = pushDir * pushSpeed;

                    // Let player keep moving while pushing
                    rb.velocity = targetVelocity * 0.9f;
                }
            }
            else
            {
                rb.velocity = targetVelocity;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }



    public void Move(InputAction.CallbackContext context)
    {
        if (isKnockedBack == false)
        {
            animator.SetBool("isWalking", true);

            if (context.canceled)
            {
                animator.SetBool("isWalking", false);
                animator.SetFloat("LastInputX", moveInput.x);
                animator.SetFloat("LastInputY", moveInput.y);
            }

            moveInput = context.ReadValue<Vector2>();
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);

            if (moveInput != Vector2.zero)
            {
                lastDirection = moveInput;
            }
        }
    }
    public Vector2 GetLastDirection()
    {
        return lastDirection;
    }
    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        Debug.Log("Knockback direction: " + direction);
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}
