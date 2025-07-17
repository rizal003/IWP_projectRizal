using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    public int damage = 2;
    public float damageInterval = 0.5f; // seconds
    private Dictionary<GameObject, float> lastDamageTime = new();

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float now = Time.time;
            if (!lastDamageTime.ContainsKey(collision.gameObject) || now - lastDamageTime[collision.gameObject] > damageInterval)
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.ChangeHealth(-damage);
                    lastDamageTime[collision.gameObject] = now;
                }
            }
        }
    }

    void OnDisable()
    {
        lastDamageTime.Clear();
    }
}
