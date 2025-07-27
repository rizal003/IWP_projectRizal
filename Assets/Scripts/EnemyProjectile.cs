using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;
    private Vector2 direction;
    private float speed;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        IgnoreEnemyCollisions();
        Destroy(gameObject, lifetime);
    }
    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;

        if (spriteRenderer != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle); // Now works!
        }
    }
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void IgnoreEnemyCollisions()
    {
        Collider2D projCollider = GetComponent<Collider2D>();
        if (projCollider == null) return;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(projCollider, enemyCollider);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}