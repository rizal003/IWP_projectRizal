using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5f;
    private Vector2 direction;
    private float speed;

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
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.ChangeHealth(-damage);
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}

