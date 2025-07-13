using System.Collections;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 3;
    [SerializeField] public int currentHealth;

    [Header("Effects")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public GameObject deathEffect;

    private Animator animator;
    private bool isDead = false;
    private CameraShake _cameraShake;
    void Start()
    {
        _cameraShake = Camera.main?.GetComponent<CameraShake>();

    }
    void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void TakeDamage(int amount)
    {
        _cameraShake?.Shake(0.1f, 0.15f); // Null-conditional operator

        if (isDead) return;

        currentHealth -= amount;
        animator.SetTrigger("Hit");
        StartCoroutine(FlashRed());

        var boss = GetComponent<DemonSlimeBoss>();
        if (boss != null) boss.OnTakeDamage();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");

        // Disable components
        GetComponent<Collider2D>().enabled = false;
        if (TryGetComponent<Rigidbody2D>(out var rb)) rb.simulated = false;
        if (TryGetComponent<Enemy_Movement>(out var move)) move.enabled = false;

        // Effects
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Destroy after animation
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animLength);
    }

    IEnumerator FlashRed()
    {
        if (!spriteRenderer) yield break;

        Color original = spriteRenderer.color;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = original;
    }
}