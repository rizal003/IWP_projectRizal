using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LavaTileMapZone : MonoBehaviour
{
    [Header("Damage (half-hearts)")]
    public int damagePerTick = 1;      // 1 = half heart
    public float tickInterval = 0.9f;  // same cadence as LavaTile

    [Header("Visuals (optional)")]
    public Color lavaTint = new Color(1f, 0.35f, 0.25f, 1f);

    private Collider2D _col;
    private readonly Dictionary<Collider2D, float> _lastTick = new();

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>(true))
            sr.color = lavaTint;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var ph = other.GetComponent<PlayerHealth>();
        if (ph == null) return;

        // Prefer a shared per-player hazard cooldown if you added it to PlayerHealth
        if (ph.TryApplyHazardTick(damagePerTick, tickInterval)) return;

        // Fallback: local cooldown per player collider
        float last; _lastTick.TryGetValue(other, out last);
        if (Time.time - last >= tickInterval)
        {
            ph.ChangeHealth(-damagePerTick);
            _lastTick[other] = Time.time;
        }
    }

    void OnTriggerExit2D(Collider2D other) => _lastTick.Remove(other);
}
