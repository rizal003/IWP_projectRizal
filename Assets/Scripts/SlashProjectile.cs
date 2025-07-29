using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 0.5f;
    public int damage = 1;
    public Vector2 direction;

    private Animator animator;
    public LayerMask targetLayers; 

    void Start()
    {
        animator = GetComponent<Animator>();
        Destroy(gameObject, lifetime);
        SetDirectionAnim(direction);
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    void SetDirectionAnim(Vector2 dir)
    {
        int dirIndex = 3;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dirIndex = dir.x > 0 ? 0 : 1;
        else
            dirIndex = dir.y > 0 ? 2 : 3;

        animator.SetInteger("Direction", dirIndex);
        animator.SetTrigger("PlaySlash");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Skip if not in target layers
        if ((targetLayers.value & (1 << collision.gameObject.layer)) == 0) return;

        Debug.Log("Slash hit: " + collision.name);

        // Rest of your damage logic...
        Enemy_Health eh = collision.GetComponentInParent<Enemy_Health>();
        if (eh != null) eh.TakeDamage(damage);

        Boss_Health bh = collision.GetComponentInParent<Boss_Health>();
        if (bh != null) bh.TakeDamage(damage);

        Destroy(gameObject);
    }

}

