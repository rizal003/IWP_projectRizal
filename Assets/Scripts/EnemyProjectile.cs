using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;
    private Vector2 direction;
    private float speed;

    void Start()
    {
        // Ignore collision with all enemies
        Collider2D projCollider = GetComponent<Collider2D>();
        if (projCollider != null)
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
                if (enemyCollider != null)
                {
                    Physics2D.IgnoreCollision(projCollider, enemyCollider);
                }
            }
        }
    }

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall")) // Only check for Wall tag
        {
            Destroy(gameObject);
        }
    }
}