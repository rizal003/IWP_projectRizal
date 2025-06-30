using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyCombat : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireCooldown = 2f;
    public float fireRange = 8f;
    public LayerMask playerLayer;

    private float fireTimer;
    private Transform player;
    private Room enemyRoom;

    void Start()
    {
        enemyRoom = GetComponentInParent<Room>();
    }

    void Update()
    {
        // Only fire if player is in the same room
        if (RoomManager.Instance.CurrentRoom != enemyRoom) return;

        fireTimer -= Time.deltaTime;

        if (player == null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, fireRange, playerLayer);
            if (hits.Length > 0) player = hits[0].transform;
        }

        if (player != null && fireTimer <= 0f)
        {
            FireAtPlayer();
            fireTimer = fireCooldown;
        }
    }

    void FireAtPlayer()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile projectile = proj.GetComponent<EnemyProjectile>();
        projectile.Initialize(dir, projectileSpeed);
    }

    public void TryFireAtPlayer(Transform player)
    {
        if (fireTimer <= 0f && player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            FireProjectile(dir);
            fireTimer = fireCooldown;
        }
    }

    private void FireProjectile(Vector2 dir)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile projectile = proj.GetComponent<EnemyProjectile>();
        projectile.Initialize(dir, projectileSpeed);
    }

}

