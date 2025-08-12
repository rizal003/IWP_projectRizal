using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LavaTile : MonoBehaviour
{
    [Header("Damage (half-hearts)")]
    public int damagePerTick = 1;       // 1 = half heart
    public float tickInterval = 0.9f;   // global cooldown per player

    [Header("Collider fit")]
    [Range(0f, 0.45f)] public float inset = 0.07f; // shrink trigger so edges don't tag you

    [Header("Visuals")]
    public Color lavaColor = new Color(1f, 0.35f, 0.25f, 1f);
    public GameObject smokeParticles;

    private BoxCollider2D col;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.usedByComposite = false;

        // Fit collider to sprite (in local space) then inset slightly
        var sr = GetComponent<SpriteRenderer>();
        if (sr && sr.sprite != null)
        {
            // sprite.bounds is world-space; convert to local
            Vector3 lossy = transform.lossyScale;
            Vector2 worldSize = sr.bounds.size;
            Vector2 localSize = new Vector2(
                lossy.x != 0 ? worldSize.x / lossy.x : 0.0f,
                lossy.y != 0 ? worldSize.y / lossy.y : 0.0f
            );

            // inset on each side
            float fx = Mathf.Max(0f, 1f - inset * 2f);
            float fy = Mathf.Max(0f, 1f - inset * 2f);
            col.size = new Vector2(localSize.x * fx, localSize.y * fy);
            col.offset = Vector2.zero;
        }

        if (sr) sr.color = lavaColor;
        if (smokeParticles) Instantiate(smokeParticles, transform.position, Quaternion.identity, transform);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other || !other.CompareTag("Player")) return;

        // Robust overlap + feet check -----------------------------
        // 1) quick reject: no actual overlap = no damage
        if (!other.IsTouching(col)) return;

        // 2) feet point must be inside (prevents “close but not on it” hits)
        Bounds pb = other.bounds;
        Vector2 feet = new Vector2(pb.center.x, pb.min.y + pb.size.y * 0.12f); // ~lowest 12%
        if (!col.OverlapPoint(feet)) return;
        // ---------------------------------------------------------

        var ph = other.GetComponent<PlayerHealth>();
        if (ph != null) ph.TryApplyHazardTick(damagePerTick, tickInterval);
    }


}

